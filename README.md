# Revit API — Day by Day

100 days. 100 commands. One plugin for Revit 2026.

Every day I post one Revit API technique on LinkedIn. All the code lives here. Clone, build, run.

Revit 2026 only. .NET 8 only.

## Installation

Clone the repo, open `RevitDayByDay.csproj` in Visual Studio 2022, hit Build. The DLL copies itself automatically. Drop `RevitDayByDay.addin` into `%AppData%\Autodesk\Revit\Addins\2026\` and start Revit.

A **Day by Day** tab will appear on the Ribbon.

## Project structure

`App.cs` registers the Ribbon tab and buttons. Each command is a separate file in `Commands`. To add a new day, create a file in Commands and add one line to App.cs.

## Days

| Day | Topic | Code | Post |
|-----|-------|------|------|
| 1 | Rename 500 views in 2 seconds | [Day001_RenameViews.cs](Commands/Day001_RenameViews.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7438982550162837504-GqFp?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 2 | Count elements by category | [Day002_CountByCategory.cs](Commands/Day002_CountByCategory.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7439301340310319105-FDtX?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 3 | Export rooms to CSV | [Day003_ExportRoomsToCsv.cs](Commands/Day003_ExportRoomsToCsv.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7439652702181163008-tsNP?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 4 | Find every warning in the model | [Day004_ListWarnings.cs](Commands/Day004_ListWarnings.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7440044717079171073-XREb?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 5 | Delete unplaced rooms | [Day005_DeleteUnplacedRooms.cs](Commands/Day005_DeleteUnplacedRooms.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7440384679049859072-guMN?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 6 | Bulk-update parameter values | [Day006_BulkSetParameter.cs](Commands/Day006_BulkSetParameter.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7440780052113408001-xDVw?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 7 | List all linked files | [Day007_ListLinkedFiles.cs](Commands/Day007_ListLinkedFiles.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7441194415504678913-hkL5?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 8 | Find elements without a workset | [Day008_NoWorksetElements.cs](Commands/Day008_NoWorksetElements.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7441389842392907776-zxi-?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 9 | Auto-number doors by room | [Day009_AutoNumberDoors.cs](Commands/Day009_AutoNumberDoors.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7441868374864838656-4_Hd?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 10 | Find the tallest wall | [Day010_TallestWall.cs](Commands/Day010_TallestWall.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7442239952244871169-zDea?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 11 | Apply view template to all plans | [Day011_ApplyViewTemplate.cs](Commands/Day011_ApplyViewTemplate.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7442602745808740352-ciPa?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 12 | Create a schedule from code | [Day012_CreateSchedule.cs](Commands/Day012_CreateSchedule.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-ugcPost-7443239962420006912-VZY9?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 13 | Section box around selection | [Day013_SectionBoxSelection.cs](Commands/Day013_SectionBoxSelection.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-ugcPost-7443281637116784641-pP5A?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 14 | Export all sheets to PDF | [Day014_ExportSheetsPdf.cs](Commands/Day014_ExportSheetsPdf.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-activity-7443707975749398528-AQyU?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 15 | Create sheets from a list | [Day015_CreateSheets.cs](Commands/Day015_CreateSheets.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-ugcPost-7443978316165988352-hyno?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 16 | Find unplaced views | [Day016_UnplacedViews.cs](Commands/Day016_UnplacedViews.cs) | [LinkedIn] (https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-ugcPost-7444409595617361921-x6gG?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |
| 17 | Duplicate a view | [Day017_DuplicateView.cs](Commands/Day017_DuplicateView.cs) | [LinkedIn](https://www.linkedin.com/posts/sowecanya_revitapi-bim-csharp-ugcPost-7444772809672740864-vOUS?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEOH9K4BmKdRXkgpxwcJ5io7vCI8SCCLEAw) |

## Author

Dinar Sharafutdinov. BIM developer, MEP.

## License

MIT
