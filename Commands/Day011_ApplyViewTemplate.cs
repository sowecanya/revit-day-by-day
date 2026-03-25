namespace RevitDayByDay.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]
    public class Day011_ApplyViewTemplate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            var templates = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => v.IsTemplate)
                .OrderBy(v => v.Name)
                .ToList();

            if (templates.Count == 0)
            {
                TaskDialog.Show("Day 11", "No view templates found in this project.");
                return Result.Failed;
            }

            var selected = ShowTemplateSelector(templates);
            if (selected == null)
                return Result.Cancelled;

            var floorPlans = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(v => !v.IsTemplate && v.ViewType == ViewType.FloorPlan)
                .ToList();

            if (floorPlans.Count == 0)
            {
                TaskDialog.Show("Day 11", "No floor plans found.");
                return Result.Failed;
            }

            using (var t = new Transaction(doc, "Apply View Template"))
            {
                t.Start();
                foreach (var plan in floorPlans)
                    plan.ViewTemplateId = selected.Id;
                t.Commit();
            }

            TaskDialog.Show("Day 11",
                $"Applied '{selected.Name}' to {floorPlans.Count} floor plans.");

            return Result.Succeeded;
        }

        private View ShowTemplateSelector(List<View> templates)
        {
            var window = new Window
            {
                Title = "Select View Template",
                Width = 400,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            var stack = new StackPanel { Margin = new Thickness(12) };

            stack.Children.Add(new TextBlock
            {
                Text = "Which template to apply to all floor plans?",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10)
            });

            var listBox = new ListBox { Height = 380 };
            foreach (var t in templates)
                listBox.Items.Add(t.Name);
            listBox.SelectedIndex = 0;
            listBox.MouseDoubleClick += (s, e) =>
            {
                if (listBox.SelectedIndex >= 0)
                    window.DialogResult = true;
            };
            stack.Children.Add(listBox);

            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var btnOk = new Button { Content = "Apply", Width = 80, IsDefault = true };
            btnOk.Click += (s, e) => window.DialogResult = true;
            btnPanel.Children.Add(btnOk);

            var btnCancel = new Button
            {
                Content = "Cancel",
                Width = 80,
                Margin = new Thickness(8, 0, 0, 0),
                IsCancel = true
            };
            btnPanel.Children.Add(btnCancel);

            stack.Children.Add(btnPanel);
            window.Content = stack;

            if (window.ShowDialog() == true && listBox.SelectedIndex >= 0)
                return templates[listBox.SelectedIndex];

            return null;
        }
    }
}
