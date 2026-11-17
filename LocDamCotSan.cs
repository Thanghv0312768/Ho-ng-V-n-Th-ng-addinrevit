using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace A005_TaoListElements
{
    [Transaction(TransactionMode.Manual)]
    public class LocDamCotSan : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                // 1. Lấy các đối tượng đang được chọn
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
                TaskDialog.Show("Revit", "Số lượng đối tượng được chọn: " + selectedIds.Count.ToString());

                // 2. Tạo 3 danh sách: dầm, cột, sàn
                List<ElementId> beamIds = new List<ElementId>();
                List<ElementId> columnIds = new List<ElementId>();
                List<ElementId> floorIds = new List<ElementId>();

                foreach (ElementId id in selectedIds)
                {
                    Element elem = doc.GetElement(id);
                    if (elem == null || elem.Category == null) continue;

                    BuiltInCategory cat = elem.Category.BuiltInCategory;

                    if (cat == BuiltInCategory.OST_StructuralFraming)        // Dầm
                    {
                        beamIds.Add(id);
                    }
                    else if (cat == BuiltInCategory.OST_StructuralColumns)   // Cột
                    {
                        columnIds.Add(id);
                    }
                    else if (elem is Floor)                                   // Sàn
                    {
                        floorIds.Add(id);
                    }
                }

                // 3. Gộp 3 danh sách và đặt làm vùng chọn hiện tại
                List<ElementId> allIds = new List<ElementId>();
                allIds.AddRange(beamIds);
                allIds.AddRange(columnIds);
                allIds.AddRange(floorIds);
                uidoc.Selection.SetElementIds(allIds);

                // 4. Thông báo kết quả
                if (allIds.Count != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Dầm: " + beamIds.Count.ToString());
                    sb.AppendLine("Cột: " + columnIds.Count.ToString());
                    sb.AppendLine("Sàn: " + floorIds.Count.ToString());
                    sb.AppendLine("Tổng: " + allIds.Count.ToString() + " đối tượng đã được chọn!");
                    TaskDialog.Show("Revit", sb.ToString());
                }
                else
                {
                    TaskDialog.Show("Revit", "Không có dầm, cột, sàn nào được chọn!");
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