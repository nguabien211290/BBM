using BBM.Business;
using BBM.Business.Logic;
using BBM.Business.Models.Module;
using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BBM.Controllers
{
    public class ImportController : BaseController
    {
        private ImportBusiness _ImportBus;
        public ImportController(ImportBusiness ImportBus)
        {
            _ImportBus = ImportBus;
        }

        public async Task<JsonResult> Import_Product(string name, HttpPostedFileBase file)
        {
            var Messaging = new RenderMessaging();

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.EndsWith(".xls") || file.FileName.EndsWith(".xlsx"))
                    {
                        Stream stream = file.InputStream;

                        IExcelDataReader reader = null;

                        if (file.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (file.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }

                        reader.IsFirstRowAsColumnNames = true;

                        DataSet result = reader.AsDataSet(true);

                        reader.Close();

                        var rs = await _ImportBus.ImportData(result, User.UserId);

                        if (rs == null)
                        {
                            Messaging.isError = true;
                            Messaging.messaging = "Do sự cố mạng, vui lòng thử lại!";
                        }
                        else
                        {
                            if (rs.Count > 0)
                            {
                                Messaging.isError = true;
                                Messaging.messaging = "Do sự cố mạng, vui lòng thử lại!";
                                Messaging.Data = rs;
                            }
                            else
                            {
                                Messaging.isError = false;
                                Messaging.messaging = "Import dữ liệu thành công";
                            }
                        }
                    }
                }
            }
            catch
            {
                Messaging.isError = true;
                Messaging.messaging = "Do sự cố mạng, vui lòng thử lại!";
            }

            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }
    }
}