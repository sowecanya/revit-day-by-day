# Revit Day By Day

Archived Revit API learning experiment: 25 small C# commands for Revit 2026.

This repository was originally planned as a 100-day LinkedIn/code series. I stopped the format after 25 days because it was not the right long-term direction. The code is kept public as a compact learning archive, not as an actively maintained product.

## Status

- Archived: no new days are planned.
- Target: Revit 2026, .NET 8, Windows.
- Scope: small isolated Revit API command examples.
- Support: no active support, releases, installer, or compatibility promises.

## What Is Here

The add-in registers a `Day by Day` ribbon tab and exposes one button per command. Each command lives in `Commands/DayXXX_*.cs`.

## Build

Prerequisites:

- Autodesk Revit 2026 installed at `C:\Program Files\Autodesk\Revit 2026`
- Visual Studio 2022 or .NET SDK with Windows desktop workload

Build:

```powershell
dotnet build RevitDayByDay.csproj
```

The project copies the compiled DLL to:

```text
%AppData%\Autodesk\Revit\Addins\2026\
```

To load the add-in, copy `RevitDayByDay.addin` to the same Revit add-ins folder and start Revit.

## Commands

| Day | Topic | Code |
|---:|---|---|
| 1 | Rename views | [Day001_RenameViews.cs](Commands/Day001_RenameViews.cs) |
| 2 | Count elements by category | [Day002_CountByCategory.cs](Commands/Day002_CountByCategory.cs) |
| 3 | Export rooms to CSV | [Day003_ExportRoomsToCsv.cs](Commands/Day003_ExportRoomsToCsv.cs) |
| 4 | List model warnings | [Day004_ListWarnings.cs](Commands/Day004_ListWarnings.cs) |
| 5 | Delete unplaced rooms | [Day005_DeleteUnplacedRooms.cs](Commands/Day005_DeleteUnplacedRooms.cs) |
| 6 | Bulk-set parameter values | [Day006_BulkSetParameter.cs](Commands/Day006_BulkSetParameter.cs) |
| 7 | List linked files | [Day007_ListLinkedFiles.cs](Commands/Day007_ListLinkedFiles.cs) |
| 8 | Find elements without a workset | [Day008_NoWorksetElements.cs](Commands/Day008_NoWorksetElements.cs) |
| 9 | Auto-number doors by room | [Day009_AutoNumberDoors.cs](Commands/Day009_AutoNumberDoors.cs) |
| 10 | Find the tallest wall | [Day010_TallestWall.cs](Commands/Day010_TallestWall.cs) |
| 11 | Apply view templates | [Day011_ApplyViewTemplate.cs](Commands/Day011_ApplyViewTemplate.cs) |
| 12 | Create a schedule | [Day012_CreateSchedule.cs](Commands/Day012_CreateSchedule.cs) |
| 13 | Create a section box around selection | [Day013_SectionBoxSelection.cs](Commands/Day013_SectionBoxSelection.cs) |
| 14 | Export sheets to PDF | [Day014_ExportSheetsPdf.cs](Commands/Day014_ExportSheetsPdf.cs) |
| 15 | Create sheets from a list | [Day015_CreateSheets.cs](Commands/Day015_CreateSheets.cs) |
| 16 | Find unplaced views | [Day016_UnplacedViews.cs](Commands/Day016_UnplacedViews.cs) |
| 17 | Duplicate a view | [Day017_DuplicateView.cs](Commands/Day017_DuplicateView.cs) |
| 18 | Color rooms by area | [Day018_ColorRoomsByArea.cs](Commands/Day018_ColorRoomsByArea.cs) |
| 19 | Tag doors | [Day019_TagAllDoors.cs](Commands/Day019_TagAllDoors.cs) |
| 20 | Place a legend on sheets | [Day020_LegendToSheets.cs](Commands/Day020_LegendToSheets.cs) |
| 21 | Read built-in parameters | [Day021_ReadBuiltInParam.cs](Commands/Day021_ReadBuiltInParam.cs) |
| 22 | Write shared parameters | [Day022_WriteSharedParam.cs](Commands/Day022_WriteSharedParam.cs) |
| 23 | Create a project parameter | [Day023_CreateProjectParam.cs](Commands/Day023_CreateProjectParam.cs) |
| 24 | Copy parameter values | [Day024_CopyParamValues.cs](Commands/Day024_CopyParamValues.cs) |
| 25 | Filter walls by thickness | [Day025_FilterByThickness.cs](Commands/Day025_FilterByThickness.cs) |

## License

MIT
