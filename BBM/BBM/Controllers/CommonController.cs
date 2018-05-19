using AutoMapper;
using BBM.Business.Models.Enum;
using BBM.Business.Models.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using BBM.Business;
using BBM.Business.Logic;
using BBM.Business.Repository;
using BBM.Business.Model.Module;

namespace BBM.Controllers
{
    public class CommonController : BaseController
    {
        private IUnitOfWork _unitOW;
        public CommonController(IUnitOfWork unitOW)
        { 
            _unitOW = unitOW;
        }

        #region config
        public JsonResult GetConfigSys()
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
                var lstBranches = Mapper.Map<List<BranchesModel>>(_unitOW.BrachesRepository.GetAll().ToList());
                var lstchannel = Mapper.Map<List<ChannelModel>>(_unitOW.ChannelRepository.GetAll().ToList());
                config.User.Channel = lstchannel;
                config.User.Branches = lstBranches;
                var emp = _unitOW.EmployeeRepository.GetAll().ToList();
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

                var configMain = _unitOW.ConfigRepository.FindBy(o => o.Type == (int)TypeConfig.Main).FirstOrDefault();
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
            var branches = new List<BranchesModel>();
            branches.Add(new BranchesModel
            {
                BranchesId = 0,
                BranchesName = "Tất cả"
            });

            branches.AddRange(Mapper.Map<List<BranchesModel>>(_unitOW.BrachesRepository.GetAll().ToList()));

            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadLstChannel()
        {
            var channel = Mapper.Map<List<ChannelModel>>(_unitOW.ChannelRepository.GetAll().ToList());

            return Json(channel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadEmployes()
        {
            var rs = Mapper.Map<List<EmployeeModel>>(_unitOW.EmployeeRepository.FindBy(o => !o.IsDelete).ToList());
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCity_District()
        {
            var city = Mapper.Map<List<CityModel>>(_unitOW.CityRepository.GetAll().ToList());

            var district = Mapper.Map<List<DistrictModel>>(_unitOW.DistrictRepository.GetAll().ToList());

            return Json(new { city = city, district = district }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadLstSuppliers()
        {
            var rs = _unitOW.SuppliersRepository.GetAll().OrderBy(o => o.Name).ToList();

            var lstTmp = Mapper.Map<List<SuppliersModel>>(rs);

            return Json(lstTmp, JsonRequestBehavior.AllowGet);

        }

        public JsonResult LoadLstCatalog()
        {
            var rs = _unitOW.CatalogRepository.GetAll().OrderBy(o => o.Name).ToList();

            return Json(Mapper.Map<List<CatalogModel>>(rs), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadEmployess_Position()
        {
            var rs = _unitOW.PositionRepository.GetAll().ToList();

            return Json(Mapper.Map<List<Employee_TitleModel>>(rs), JsonRequestBehavior.AllowGet);
        }
        #endregion 
    }
}
