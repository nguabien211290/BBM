using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Model.Enum;
using BBM.Business.Models.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BBM.Business.Logic
{
    public partial class ImportBusiness
    {
        public async Task<List<shop_sanpham>> ImportData(DataSet data, int UserId)
        {
            var masp = string.Empty;

            var line = 1;

            var perfixBranches = "Kho_";

            var perfixChannel = "Kenh_";

            var perfixPriceChannel = "KM_Kenh_";

            var perfixPriceChannel_StartDate = "KM_NBD_Kenh_";

            var perfixPriceChannel_EndDate = "KM_NKT_Kenh_";

            var lstError = new List<shop_sanpham>();

            await Update_is_import(true, null, 0, true);

            int percent = 0;

            try
            {
                var supplie_lst = unitOfWork.SuppliersRepository.GetAll().ToList();

                var catalog_lst = unitOfWork.CatalogRepository.GetAll().ToList();

                var channel_lst = unitOfWork.ChannelRepository.GetAll().ToList();

                var braches_lst = unitOfWork.BrachesRepository.GetAll().ToList();

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    line++;

                    bool isError = false;

                    percent = (int)Math.Round(100.0 * line / data.Tables[0].Rows.Count);

                    var product = new shop_sanpham();

                    var imgage = new shop_image();

                    bool isUpdatePriceChanle = false;
                    bool isUpdatePriceChanleDisscount = false;
                    bool isUpdateProduct = false;
                    bool isUpdateStock = false;
                    try
                    {
                        foreach (DataColumn col in data.Tables[0].Columns)
                        {
                            var columnName = col.ColumnName;

                            var value = row[col.ColumnName].ToString();

                            if (columnName.Equals("Code"))
                            {
                                masp = value.Trim();

                                if (string.IsNullOrEmpty(masp))
                                {
                                    isError = true;
                                    break;
                                }


                                product = unitOfWork.ProductRepository.FindBy(o => o.masp.Equals(masp)).FirstOrDefault();

                                if (product == null || product.id <= 0)
                                {
                                    product = new shop_sanpham();
                                    product.masp = value.Trim();
                                }
                            }

                            if (string.IsNullOrEmpty(value))
                                continue;

                            #region San phẩm
                            switch (columnName)
                            {
                                case "ProductName":
                                    product.tensp = value;
                                    isUpdateProduct = true;
                                    break;
                                case "Img":
                                    imgage.url = value;
                                    imgage.RefId = product.id;
                                    isUpdateProduct = true;
                                    break;
                                case "PriceBase":
                                    var tmp_PriceBase = float.Parse(value);
                                    product.PriceBase = (int)tmp_PriceBase;
                                    isUpdateProduct = true;
                                    break;
                                case "PriceCompare":
                                    var tmp_PriceCompare = float.Parse(value);
                                    product.PriceCompare = (int)tmp_PriceCompare;
                                    isUpdateProduct = true;
                                    break;
                                case "PriceBase_Old":
                                    var tmp_PriceBase_Old = float.Parse(value);
                                    product.PriceBase_Old = (int)tmp_PriceBase_Old;
                                    isUpdateProduct = true;
                                    break;
                                case "PriceInput":
                                    var tmp_PriceInput = float.Parse(value);
                                    product.PriceInput = (int)tmp_PriceInput;
                                    isUpdateProduct = true;
                                    break;
                                case "PriceWholesale":
                                    var tmp_PriceWholesale = float.Parse(value);
                                    product.PriceWholesale = (int)tmp_PriceWholesale;
                                    isUpdateProduct = true;
                                    break;
                                case "Status":
                                    var statusE = EnumHelper<StatusProduct>.Parse(value);
                                    product.Status = (int)statusE;
                                    isUpdateProduct = true;
                                    break;
                                case "VAT":
                                    var vatE = EnumHelper<StatusVATProduct>.Parse(value);
                                    product.Status = (int)vatE;
                                    isUpdateProduct = true;
                                    break;
                                case "Suppliers":

                                    var nameSupp = value.ToLower().Trim();

                                    var supplie = supplie_lst.FirstOrDefault(o => o.Name.ToLower().Equals(nameSupp));

                                    if (supplie != null)
                                        product.SuppliersId = supplie.SuppliersId;
                                    else
                                    {
                                        supplie = new soft_Suppliers
                                        {
                                            Name = nameSupp,
                                            DateCreate = DateTime.Now,
                                            EmployeeCreate = UserId
                                        };

                                        product.SuppliersId = supplie.SuppliersId;

                                        unitOfWork.SuppliersRepository.Add(supplie);

                                        supplie_lst.Add(supplie);
                                    }
                                    isUpdateProduct = true;
                                    break;
                                case "Catalog":
                                    var nameCatalog = value.ToLower().Trim();

                                    var catalog = catalog_lst.FirstOrDefault(o => o.Name.ToLower().Equals(nameCatalog));

                                    if (catalog != null)
                                        product.CatalogId = catalog.Id;
                                    else
                                    {
                                        catalog = new soft_Catalog
                                        {
                                            Name = nameCatalog,
                                            DateCreate = DateTime.Now,
                                            EmployeeCreate = UserId
                                        };

                                        product.CatalogId = catalog.Id;

                                        unitOfWork.CatalogRepository.Add(catalog);

                                        catalog_lst.Add(catalog);
                                    }
                                    isUpdateProduct = true;
                                    break;
                            }
                            #endregion
                            #region Gia kenh
                            if (columnName.StartsWith(perfixChannel))
                            {
                                var codeChannel = columnName.Substring(perfixChannel.Length);

                                var channel = channel_lst.FirstOrDefault(o => o.Code.Equals(codeChannel));

                                if (channel != null)
                                {
                                    bool isupdate_disscount = true;

                                    var priceDisscount_Value = 0;
                                    DateTime? priceDisscount_StarDate = null;
                                    DateTime? priceDisscount_EndDate = null;
                                    try
                                    {
                                        DataRow dr = data.Tables[0].Select("Code='" + masp + "'").FirstOrDefault();

                                        if (dr != null)
                                        {
                                            priceDisscount_Value = int.Parse(dr[perfixPriceChannel + channel.Code].ToString());

                                            var StarDate = Convert.ToDateTime(dr[perfixPriceChannel_StartDate + channel.Code].ToString());
                                            var EndDate = Convert.ToDateTime(dr[perfixPriceChannel_EndDate + channel.Code].ToString());

                                            priceDisscount_StarDate = StarDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddDays(-1);
                                            priceDisscount_EndDate = EndDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                                        }
                                    }
                                    catch
                                    {
                                        priceDisscount_Value = 0;
                                        priceDisscount_StarDate = null;
                                        priceDisscount_EndDate = null;
                                        isupdate_disscount = false;
                                    }

                                    if (product.id > 0)
                                    {
                                        var channelPrice = unitOfWork.ChanelPriceRepository.FindBy(o => o.ChannelId == channel.Id && o.ProductId == product.id).FirstOrDefault();

                                        if (channelPrice == null)
                                        {
                                            var newobj = new soft_Channel_Product_Price
                                            {
                                                ProductId = product.id,
                                                ChannelId = channel.Id,
                                                Price = int.Parse(value),
                                                DateCreate = DateTime.Now,
                                                EmployeeCreate = UserId,
                                                Price_Discount = priceDisscount_Value,
                                                StartDate_Discount = priceDisscount_StarDate,
                                                Enddate_Discount = priceDisscount_EndDate
                                            };

                                            isUpdatePriceChanle = true;

                                            unitOfWork.ChanelPriceRepository.Add(newobj);
                                        }
                                        else
                                        {
                                            channelPrice.Price = int.Parse(value);
                                            channelPrice.Price_Discount = priceDisscount_Value;
                                            channelPrice.StartDate_Discount = priceDisscount_StarDate;
                                            channelPrice.Enddate_Discount = priceDisscount_EndDate;

                                            isUpdatePriceChanle = true;

                                            if (isupdate_disscount)
                                                unitOfWork.ChanelPriceRepository.Update(channelPrice,
                                                    o => o.Price,
                                                    o => o.Price_Discount,
                                                    o => o.StartDate_Discount,
                                                    o => o.Enddate_Discount);
                                            else
                                                unitOfWork.ChanelPriceRepository.Update(channelPrice,
                                                  o => o.Price);
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region Giá khuyến mãi
                            if (columnName.StartsWith(perfixPriceChannel) && !isUpdatePriceChanle)
                            {
                                var codeChannel = columnName.Substring(perfixPriceChannel.Length);

                                var channel = unitOfWork.ChannelRepository.FindBy(o => o.Code == codeChannel).FirstOrDefault();// channel_lst.FirstOrDefault(o => o.Code.Equals(codeChannel));

                                if (channel != null)
                                {
                                    var priceDisscount_Value = 0;
                                    DateTime? priceDisscount_StarDate = null;
                                    DateTime? priceDisscount_EndDate = null;
                                    try
                                    {
                                        DataRow dr = data.Tables[0].Select("Code='" + masp + "'").FirstOrDefault();

                                        if (dr != null)
                                        {
                                            priceDisscount_Value = int.Parse(dr[perfixPriceChannel + channel.Code].ToString());

                                            string startDate = perfixPriceChannel_StartDate + channel.Code;
                                            string endDate = perfixPriceChannel_EndDate + channel.Code;

                                            var StarDate = Convert.ToDateTime(dr[startDate].ToString());
                                            var EndDate = Convert.ToDateTime(dr[endDate].ToString());

                                            priceDisscount_StarDate = StarDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddDays(-1);
                                            priceDisscount_EndDate = EndDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                                        }


                                        if (product.id > 0)
                                        {
                                            var channelPrice = unitOfWork.ChanelPriceRepository.FindBy(o => o.ChannelId == channel.Id
                                                                                            && o.ProductId == product.id).FirstOrDefault();

                                            if (channelPrice != null)
                                            {
                                                channelPrice.Price_Discount = priceDisscount_Value;
                                                channelPrice.StartDate_Discount = priceDisscount_StarDate;
                                                channelPrice.Enddate_Discount = priceDisscount_EndDate;

                                                unitOfWork.ChanelPriceRepository.Update(channelPrice,
                                                    o => o.Price_Discount,
                                                    o => o.StartDate_Discount,
                                                    o => o.Enddate_Discount);

                                                isUpdatePriceChanleDisscount = true;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        priceDisscount_Value = 0;
                                        priceDisscount_StarDate = null;
                                        priceDisscount_EndDate = null;
                                    }
                                }
                            }
                            #endregion
                            #region Tồn kho
                            if (columnName.StartsWith(perfixBranches))
                            {

                                var codeBranche = columnName.Substring(perfixBranches.Length);

                                var barches = braches_lst.FirstOrDefault(o => o.Code.Equals(codeBranche));

                                if (barches != null)
                                {
                                    if (product.id <= 0)
                                    {
                                        var newobj = new soft_Branches_Product_Stock
                                        {
                                            ProductId = product.id,
                                            BranchesId = barches.BranchesId,
                                            Stock_Total = double.Parse(value),
                                            DateCreate = DateTime.Now,
                                            EmployeeCreate = UserId,
                                        };

                                        unitOfWork.BrachesStockRepository.Add(newobj);
                                    }
                                    else
                                    {
                                        var stock = unitOfWork.BrachesStockRepository.FindBy(o => o.BranchesId == barches.BranchesId && o.ProductId == product.id).FirstOrDefault();

                                        if (stock == null)
                                        {
                                            var newobj = new soft_Branches_Product_Stock
                                            {
                                                ProductId = product.id,
                                                BranchesId = barches.BranchesId,
                                                Stock_Total = double.Parse(value),
                                                DateCreate = DateTime.Now,
                                                EmployeeCreate = UserId
                                            };

                                            unitOfWork.BrachesStockRepository.Add(newobj);
                                        }
                                        else
                                        {
                                            stock.Stock_Total = double.Parse(value);

                                            unitOfWork.BrachesStockRepository.Update(stock, o => o.Stock_Total);
                                        }
                                    }

                                }
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        lstError.Add(new shop_sanpham
                        {
                            tensp = product.tensp,
                            masp = product.masp,
                            id = line
                        });

                        isError = true;
                    }

                    if (isError)
                        continue;
                    try
                    {
                        if (isUpdateProduct)
                        {
                            if (product.id <= 0)
                            {
                                product.DateCreate = DateTime.Now;

                                product.FromCreate = (int)TypeFromCreate.Soft;

                                unitOfWork.ProductRepository.Add(product);

                                unitOfWork.ImageRepository.Add(imgage);
                            }
                            else
                            {
                                unitOfWork.ProductRepository.Update(product, o => o.tensp,
                                                             o => o.PriceBase,
                                                             o => o.PriceCompare,
                                                             o => o.PriceBase_Old,
                                                             o => o.PriceInput,
                                                             o => o.Status,
                                                             o => o.StatusVAT,
                                                             o => o.SuppliersId,
                                                             o => o.CatalogId,
                                                             o => o.PriceWholesale);
                            }
                        }
                        await Update_is_import(true, null, percent);

                        if (isUpdateProduct
                            ||isUpdatePriceChanle
                            || isUpdateStock
                            || isUpdatePriceChanleDisscount)
                            await unitOfWork.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        lstError.Add(new shop_sanpham
                        {
                            tensp = product.tensp,
                            masp = product.masp,
                            id = line
                        });

                    }
                }
                var error = string.Empty;

                if (lstError != null && lstError.Count > 0)
                    error = new JavaScriptSerializer().Serialize(lstError.Select(o => new { masp = o.masp, tensp = o.tensp, id = o.id }));

                await Update_is_import(false, error, percent, true);

                return lstError;
            }
            catch (Exception ex)
            {

                lstError.Add(new shop_sanpham
                {
                    tensp = ex.Message,
                    masp = masp,
                    id = line
                });

                var error = new JavaScriptSerializer().Serialize(lstError.Select(o => new { masp = o.masp, tensp = o.tensp, id = o.id }));

                await Update_is_import(false, error, -1, true);

                return null;
            }
        }
        private async Task<int> Update_is_import(bool isimport, string error, int percent_Process, bool iscommit = false)
        {
            try
            {
                var configMain = unitOfWork.ConfigRepository.FindBy(o => o.Type == (int)TypeConfig.Main).FirstOrDefault();

                configMain.Is_import = isimport;

                if (!isimport && !string.IsNullOrEmpty(error))
                    configMain.Error_Import = error;
                else
                    configMain.Error_Import = null;

                configMain.Percent_Process = percent_Process;

                unitOfWork.ConfigRepository.Update(configMain, o => o.Is_import, o => o.Error_Import, o => o.Percent_Process);

                if (iscommit)
                    await unitOfWork.SaveChanges();

                return configMain.Percent_Process.Value;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<shop_sanpham>> ImportPriceDisscount(DataSet data, int UserId)
        {
            var masp = string.Empty;

            var line = 1;

            var lstError = new List<shop_sanpham>();

            await Update_is_import(true, null, 0, true);

            int percent = 0;

            try
            {
                var supplie_lst = unitOfWork.SuppliersRepository.GetAll().ToList();

                var catalog_lst = unitOfWork.CatalogRepository.GetAll().ToList();

                var channel_lst = unitOfWork.ChannelRepository.GetAll().ToList();

                var braches_lst = unitOfWork.BrachesRepository.GetAll().ToList();

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    line++;

                    bool isError = false;

                    percent = (int)Math.Round(100.0 * line / data.Tables[0].Rows.Count);

                    var product = new shop_sanpham();
                    try
                    {
                        foreach (DataColumn col in data.Tables[0].Columns)
                        {
                            var columnName = col.ColumnName;

                            var value = row[col.ColumnName].ToString();

                            if (columnName.Equals("Code"))
                            {
                                masp = value.Trim();

                                if (string.IsNullOrEmpty(masp))
                                {
                                    isError = true;
                                    break;
                                }

                                product = unitOfWork.ProductRepository.FindBy(o => o.masp.Equals(masp)).FirstOrDefault();

                                if (product == null)
                                    continue;
                            }

                            if (columnName.StartsWith("Giá KM"))
                            {
                                var channelOL = unitOfWork.ChannelRepository.FindBy(o => o.Code == "OL").FirstOrDefault();// channel_lst.FirstOrDefault(o => o.Code.Equals(codeChannel));

                                if (channelOL != null)
                                {
                                    try
                                    {

                                        if (product.id > 0)
                                        {
                                            var channelPrice = unitOfWork.ChanelPriceRepository.FindBy(o => o.ChannelId == channelOL.Id
                                                                                            && o.ProductId == product.id).FirstOrDefault();

                                            if (channelPrice != null)
                                            {
                                                channelPrice.Price_Discount = int.Parse(value);

                                                unitOfWork.ChanelPriceRepository.Update(channelPrice, o => o.Price_Discount);
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lstError.Add(new shop_sanpham
                        {
                            tensp = product.tensp,
                            masp = product.masp,
                            id = line
                        });

                        isError = true;
                    }

                    if (isError)
                        continue;


                    try
                    {
                        await unitOfWork.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        lstError.Add(new shop_sanpham
                        {
                            tensp = product.tensp,
                            masp = product.masp,
                            id = line
                        });

                    }
                }
                var error = string.Empty;

                if (lstError != null && lstError.Count > 0)
                    error = new JavaScriptSerializer().Serialize(lstError.Select(o => new { masp = o.masp, tensp = o.tensp, id = o.id }));

                await Update_is_import(false, error, percent, true);

                return lstError;
            }
            catch (Exception ex)
            {

                lstError.Add(new shop_sanpham
                {
                    tensp = ex.Message,
                    masp = masp,
                    id = line
                });

                var error = new JavaScriptSerializer().Serialize(lstError.Select(o => new { masp = o.masp, tensp = o.tensp, id = o.id }));

                await Update_is_import(false, error, -1, true);

                return null;
            }
        }

        //public void ToExcel(HttpResponseBase Response, int type)
        //{
        //    var data = new DataTable();

        //    var isWork = true;

        //    switch (type)
        //    {
        //        case (int)TypeView.Product:
        //            break;
        //        default:
        //            isWork = false;
        //            break;
        //    }

        //    if (!isWork)
        //        return;

        //    var grid = new System.Web.UI.WebControls.GridView();
        //    grid.DataSource = GetData();
        //    grid.DataBind();
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment; filename=FileName.xls");
        //    Response.ContentType = "application/excel";
        //    StringWriter sw = new StringWriter();
        //    HtmlTextWriter htw = new HtmlTextWriter(sw);

        //    grid.RenderControl(htw);
        //    Response.Write(sw.ToString());
        //    Response.End();
        //}

        private DataTable GetData()
        {
            // Here we create a DataTable with four columns.
            DataTable dtSample = new DataTable();
            dtSample.Columns.Add("Dosage", typeof(int));
            dtSample.Columns.Add("Drug", typeof(string));
            dtSample.Columns.Add("Patient", typeof(string));
            dtSample.Columns.Add("Date", typeof(DateTime));

            // Here we add five DataRows.
            dtSample.Rows.Add(25, "Indocin", "David", DateTime.Now);
            dtSample.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            dtSample.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            dtSample.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            dtSample.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);
            dtSample.Rows.Add(25, "Indocin", "David", DateTime.Now);
            dtSample.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            dtSample.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            dtSample.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            dtSample.Rows.Add(25, "Indocin", "David", DateTime.Now);
            dtSample.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            dtSample.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            dtSample.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            dtSample.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);
            dtSample.Rows.Add(25, "Indocin", "David", DateTime.Now);

            return dtSample;

        }
    }
}
