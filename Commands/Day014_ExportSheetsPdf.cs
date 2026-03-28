using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitDayByDay.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class Day014_ExportSheetsPdf : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Collect all sheets
                List<ViewSheet> sheets = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSheet))
                    .Cast<ViewSheet>()
                    .Where(s => !s.IsPlaceholder)
                    .OrderBy(s => s.SheetNumber)
                    .ToList();

                if (sheets.Count == 0)
                {
                    TaskDialog.Show("Export PDF", "No sheets found in the project.");
                    return Result.Succeeded;
                }

                // Output folder on desktop
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string outputFolder = Path.Combine(desktopPath, "RevitPDF_Export");

                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // Check for locked files before starting export
                foreach (ViewSheet sheet in sheets)
                {
                    string safeName = SanitizeFileName($"{sheet.SheetNumber} - {sheet.Name}");
                    string existingFile = Path.Combine(outputFolder, safeName + ".pdf");
                    if (File.Exists(existingFile))
                    {
                        try
                        {
                            File.Delete(existingFile);
                        }
                        catch (IOException)
                        {
                            TaskDialog.Show("Export PDF",
                                $"Cannot overwrite \"{safeName}.pdf\" — the file is locked.\n" +
                                "Close it in your PDF viewer and try again.");
                            return Result.Cancelled;
                        }
                    }
                }

                // Prepare PDF export options
                PDFExportOptions pdfOptions = new PDFExportOptions();
                pdfOptions.Combine = false; // One PDF per sheet
                pdfOptions.ColorDepth = ColorDepthType.Color;
                pdfOptions.ExportQuality = PDFExportQualityType.DPI300;
                pdfOptions.ZoomType = ZoomType.Zoom;
                pdfOptions.ZoomPercentage = 100;
                // NamingRule uses sheet number + sheet name by default

                // Export all sheets in a single call — Revit handles progress internally
                IList<ElementId> allSheetIds = sheets.Select(s => s.Id).ToList();
                bool success = doc.Export(outputFolder, allSheetIds, pdfOptions);

                // Count actual exported files
                int exported = Directory.GetFiles(outputFolder, "*.pdf").Length;

                string resultMessage = success
                    ? $"Exported {exported} sheet(s) to PDF.\nOutput folder: {outputFolder}"
                    : $"Export completed with errors. {exported} file(s) created.\nOutput folder: {outputFolder}";

                TaskDialog.Show("Export PDF", resultMessage);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
    }
}
