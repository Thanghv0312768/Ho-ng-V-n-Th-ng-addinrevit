using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace ClassLibrary3
{
    [Transaction(TransactionMode.Manual)]
    public class CA004_SelectWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;

                // 1. Lấy các đối tượng đang được chọn
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
                TaskDialog.Show("Revit", "Số lượng đối tượng được chọn: " + selectedIds.Count.ToString());

                // 2. Lọc ra các đối tượng là Tường
                ICollection<ElementId> selectedWallIds = new List<ElementId>();
                foreach (ElementId id in selectedIds)
                {
                    Element elements1 = uidoc.Document.GetElement(id);
                    if (elements1 is Wall)
                    {
                        selectedWallIds.Add(id);
                    }
                }

                // 3. Đặt danh sách tường làm vùng chọn hiện tại
                uidoc.Selection.SetElementIds(selectedWallIds);

                // 4. Thông báo kết quả
                if (0 != selectedWallIds.Count)
                {
                    TaskDialog.Show("Revit", selectedWallIds.Count.ToString() + " Đối tượng Tường đã được chọn!");
                }
                else
                {
                    TaskDialog.Show("Revit", "Không có đối tượng tường nào được chọn!");
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}