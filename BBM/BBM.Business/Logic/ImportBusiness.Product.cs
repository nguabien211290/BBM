using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using System;
using System.Collections.Generic;
using System.Data;
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

            var perfixPriceChannel = "Kenh_KM_";

            var perfixPriceChannel_StartDate = "Kenh_KM_NBD_";

            var perfixPriceChannel_EndDate = "Kenh_KM_NKT_";

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

                    try
                    {
                        foreach (DataColumn col in data.Tables[0].Columns)
                        {
                            var columnName = col.ColumnName;

                            var value = row[col.ColumnName].ToString();

                            if (string.IsNullOrEmpty(value))
                                continue;

                            if (columnName.Equals("Code"))
                            {
                                masp = value.Trim();

                                product = unitOfWork.ProductRepository.FindBy(o => o.masp.Equals(masp)).FirstOrDefault();

                                if (product == null || product.id <= 0)
                                {
                                    product = new shop_sanpham();
                                    product.masp = value.Trim();
                                }
                            }

                            switch (columnName)
                            {
                                case "ProductName":
                                    product.tensp = value;
                                    break;
                                case "Img":
                                    imgage.url = value;
                                    imgage.RefId = product.id;
                                    break;
                                case "PriceBase":
                                    var tmp_PriceBase = float.Parse(value);
                                    product.PriceBase = (int)tmp_PriceBase;
                                    break;
                                case "PriceCompare":
                                    var tmp_PriceCompare = float.Parse(value);
                                    product.PriceCompare = (int)tmp_PriceCompare;
                                    break;
                                case "PriceBase_Old":
                                    var tmp_PriceBase_Old = float.Parse(value);
                                    product.PriceBase_Old = (int)tmp_PriceBase_Old;
                                    break;
                                case "PriceInput":
                                    var tmp_PriceInput = float.Parse(value);
                                    product.PriceInput = (int)tmp_PriceInput;
                                    break;
                                case "PriceWholesale":
                                    var tmp_PriceWholesale = float.Parse(value);
                                    product.PriceWholesale = (int)tmp_PriceWholesale;
                                    break;
                                case "Status":
                                    var statusE = EnumHelper<StatusProduct>.Parse(value);
                                    product.Status = (int)statusE;
                                    break;
                                case "VAT":
                                    var vatE = EnumHelper<StatusVATProduct>.Parse(value);
                                    product.Status = (int)vatE;
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
                                    break;
                            }
                            if (columnName.StartsWith(perfixChannel))
                            {
                                var codeChannel = columnName.Substring(perfixChannel.Length);

                                var channel = channel_lst.FirstOrDefault(o => o.Code.Equals(codeChannel));

                                var priceDisscount_Value = 0;
                                var priceDisscount_StarDate = new DateTime?();
                                var priceDisscount_EndDate = new DateTime?();
                                try
                                {
                                    DataRow dr = data.Tables[0].Select("Code='" + masp + "'").FirstOrDefault();

                                    if (dr != null)
                                    {
                                        priceDisscount_Value = int.Parse(dr[perfixPriceChannel + channel.Code].ToString());
                                        priceDisscount_StarDate = Convert.ToDateTime(dr[perfixPriceChannel_StartDate + channel.Code].ToString());
                                        priceDisscount_EndDate = Convert.ToDateTime(dr[perfixPriceChannel_EndDate + channel.Code].ToString());
                                    }
                                }
                                catch
                                {
                                    priceDisscount_Value = 0;
                                    priceDisscount_StarDate = null;
                                    priceDisscount_EndDate = null;
                                }

                                if (channel != null)
                                {
                                    if (product.id <= 0)
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

                                        unitOfWork.ChanelPriceRepository.Add(newobj);
                                    }
                                    else
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

                                            unitOfWork.ChanelPriceRepository.Add(newobj);
                                        }
                                        else
                                        {
                                            channelPrice.Price = int.Parse(value);

                                            unitOfWork.ChanelPriceRepository.Update(channelPrice,
                                                o => o.Price,
                                                o => o.Price_Discount,
                                                o => o.StartDate_Discount,
                                                o => o.Enddate_Discount);
                                        }
                                    }
                                }
                            }
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
                                            EmployeeCreate = UserId
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
                        if (product.id <= 0)
                        {
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
                                                         o => o.PriceWholesale
                                                         );
                        }

                        await Update_is_import(true, null, percent);

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
    }
}
