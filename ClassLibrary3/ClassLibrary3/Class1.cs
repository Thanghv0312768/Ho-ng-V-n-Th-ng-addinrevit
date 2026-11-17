using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace ClassLibrary3
{
    [Transaction(TransactionMode.ReadOnly)]
    public class CA003_ShowFamilyName : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // 1. Cho người dùng bấm chọn 1 vật thể (Esc để huỷ)
                Reference pickedRef = uidoc.Selection.PickObject(
                    ObjectType.Element,
                    "Bấm chọn một vật thể để xem tên Family");

                Element element = doc.GetElement(pickedRef);
                if (element == null)
                {
                    message = "Không lấy được vật thể đã chọn.";
                    return Result.Failed;
                }

                // 2. Lấy Type của vật thể, từ Type lấy ra tên Family
                string familyName = "(không có)";
                string typeName = "(không có)";

                if (doc.GetElement(element.GetTypeId()) is ElementType elementType)
                {
                    familyName = elementType.FamilyName;
                    typeName = elementType.Name;
                }

                string categoryName = element.Category != null ? element.Category.Name : "(không có)";

                // 3. Hiển thị kết quả
                TaskDialog dialog = new TaskDialog("Thông tin Family");
                dialog.MainInstruction = "Family: " + familyName;
                dialog.MainContent =
                    "Type: " + typeName + "\n" +
                    "Category: " + categoryName + "\n" +
                    "Element ID: " + element.Id.ToString();
                dialog.Show();

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // Người dùng nhấn Esc
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}