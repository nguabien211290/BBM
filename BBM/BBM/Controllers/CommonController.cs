using AutoMapper;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBM.Business.Infractstructure;
using BBM.Business.Infractstructure.Security;
using System.Web.Security;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using System.Web.Script.Serialization;
using Excel;
using System.Threading.Tasks;
using System.Transactions;
using BBM.Business;
using BBM.Business.Logic;

namespace BBM.Controllers
{
    public class CommonController : BaseController
    {
        private CRUD _crud;
        private admin_softbbmEntities _context;
        private ImportExcelBusiness _importBus;
        private IImportBusiness _ImportBus;
        public CommonController(ImportBusiness ImportBus)
        {
            _crud = new CRUD();
            _context = new admin_softbbmEntities();
            _importBus = new ImportExcelBusiness();
            _ImportBus = ImportBus;
        }


        public async Task<JsonResult> GetConfigSys()
        {
            var Messaging = new RenderMessaging();
            try
            {
                if (User == null)
                {
                    Messaging.isError = true;
                    Messaging.messaging = "Vui lòng đăng nhập lại!";
                }

                var config = new ConfigModel();
                config.User = Mapper.Map<Config_UserModel>(User);
                config.User.Branches = new List<BranchesModel>();
                config.User.Channel = new List<ChannelModel>();
                config.User.BranchesId = User.BranchesId;
                config.User.ChannelId = User.ChannelId;
                var lstBranches = Mapper.Map<List<BranchesModel>>(await _crud.GetAll<soft_Branches>());
                var lstchannel = Mapper.Map<List<ChannelModel>>(await _crud.GetAll<soft_Channel>());
                config.User.Channel = lstchannel;
                config.User.Branches = lstBranches;
                var emp = await _crud.GetAll<sys_Employee>();
                config.Employee = new List<EmployeeModel>();
                foreach (var item in emp)
                {
                    config.Employee.Add(new EmployeeModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Email = item.Email
                    });
                }

                var configMain = await _crud.FindBy<soft_Config>(o => o.Type == (int)TypeConfig.Main);
                if (configMain != null)
                    config.Config = Mapper.Map<Config>(configMain);

                Messaging.Data = config;
            }
            catch (Exception ex)
            {
                Messaging.isError = true;
                Messaging.messaging = "Lấy Cấu hình không thành công!";
            }
            return Json(Messaging, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadLstBranches()
        {
            var branches = Mapper.Map<List<BranchesModel>>(_context.soft_Branches.ToList());

            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadLstChannel()
        {
            var channel = Mapper.Map<List<ChannelModel>>(_context.soft_Channel.ToList());

            return Json(channel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadEmployes()
        {
            var rs = Mapper.Map<List<EmployeeModel>>(_context.sys_Employee.Where(o => !o.IsDelete).ToList());
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Import_Excel(string name, HttpPostedFileBase file)
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

                        DataSet result = reader.AsDataSet();

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
