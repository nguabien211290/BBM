using BBM.Business.Infractstructure;
using BBM.Business.Model.Entity;
using BBM.Business.Models.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Script.Serialization;

namespace BBM.Business
{
    public class ImportExcelBusiness
    {
        private admin_softbbmEntities _context;
        private CRUD _crud;
        public ImportExcelBusiness()
        {
            _context = new admin_softbbmEntities();
            _crud = new CRUD();
        }
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
                var supplie_lst = await _crud.GetAll<soft_Suppliers>();

                var catalog_lst = await _crud.GetAll<soft_Catalog>();

                var channel_lst = await _crud.GetAll<soft_Channel>();

                var braches_lst = await _crud.GetAll<soft_Branches>();

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

                                product = await _crud.FindBy<shop_sanpham>(o => o.masp.Equals(masp));

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

                                        _crud.Add<soft_Suppliers>(supplie);

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

                                        _crud.Add<soft_Catalog>(catalog);

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
                                        _crud.Add<soft_Channel_Product_Price>(newobj);
                                    }
                                    else
                                    {
                                        var channelPrice = await _crud.FindBy<soft_Channel_Product_Price>(o => o.ChannelId == channel.Id && o.ProductId == product.id);

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

                                            _crud.Add<soft_Channel_Product_Price>(newobj);
                                        }
                                        else
                                        {
                                            channelPrice.Price = int.Parse(value);

                                            _crud.Update<soft_Channel_Product_Price>(channelPrice,
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

                                        _crud.Add<soft_Branches_Product_Stock>(newobj);
                                    }
                                    else
                                    {
                                        var stock = await _crud.FindBy<soft_Branches_Product_Stock>(o => o.BranchesId == barches.BranchesId && o.ProductId == product.id);

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

                                            _crud.Add<soft_Branches_Product_Stock>(newobj);
                                        }
                                        else
                                        {
                                            stock.Stock_Total = double.Parse(value);

                                            _crud.Update<soft_Branches_Product_Stock>(stock, o => o.Stock_Total);
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

                    using (var transaction = _crud.DbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            if (product.id <= 0)
                            {
                                _crud.Add<shop_sanpham>(product);

                                _crud.Add<shop_image>(imgage);
                            }
                            else
                            {
                                _crud.Update<shop_sanpham>(product, o => o.tensp,
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

                            // if (line % 100 == 0)
                            //{
                            await Update_is_import(true, null, percent);

                            await _crud.SaveChangesAsync(true);

                            transaction.Commit();
                            //}
                        }
                        catch (Exception ex)
                        {
                            lstError.Add(new shop_sanpham
                            {
                                tensp = product.tensp,
                                masp = product.masp,
                                id = line
                            });

                            transaction.Rollback();
                        }
                    }
                }

                //await _crud.SaveChangesAsync();

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
                var configMain = await _crud.FindBy<soft_Config>(o => o.Type == (int)TypeConfig.Main);

                configMain.Is_import = isimport;

                if (!isimport && !string.IsNullOrEmpty(error))
                    configMain.Error_Import = error;
                else
                    configMain.Error_Import = null;

                configMain.Percent_Process = percent_Process;

                _crud.Update<soft_Config>(configMain, o => o.Is_import, o => o.Error_Import, o => o.Percent_Process);

                if (iscommit)
                    await _crud.SaveChangesAsync();

                return configMain.Percent_Process.Value;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        private async Task<admin_softbbmEntities> AddorUpdateToContext<T>(admin_softbbmEntities context, T entity, int count, int commitCount, bool recreateContext, int percent, params Expression<Func<T, object>>[] updatedProperties) where T : class
        {
            if (updatedProperties != null)
            {
                var dbEntityEntry = context.Entry(entity);

                context.Set<T>().Attach(entity);

                if (updatedProperties.Any())
                {
                    foreach (var property in updatedProperties)
                    {
                        dbEntityEntry.Property(property).IsModified = true;
                    }
                }
                else
                {
                    context.Entry(entity).State = EntityState.Modified;
                }
            }
            else
            {
                context.Set<T>().Add(entity);
            }

            if (count % commitCount == 0)
            {
                await Update_is_import(false, null, percent, false);

                await context.SaveChangesAsync();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new admin_softbbmEntities();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }
            return context;
        }

        public async Task<List<shop_sanpham>> ImportData2(DataSet data, int UserId)
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

            admin_softbbmEntities context = null;

            try
            {
                context = new admin_softbbmEntities();

                context.Configuration.AutoDetectChangesEnabled = false;

                var supplie_lst = context.soft_Suppliers.ToList();

                var catalog_lst = context.soft_Catalog.ToList();

                var channel_lst = context.soft_Channel.ToList();

                var braches_lst = context.soft_Branches.ToList();

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    line++;

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

                                product = context.shop_sanpham.FirstOrDefault(o => o.masp.Equals(masp));

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

                                        context = await AddorUpdateToContext<soft_Suppliers>(context, supplie, line, 100, true, percent, null);

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

                                        context = await AddorUpdateToContext<soft_Catalog>(context, catalog, line, 100, true, percent, null);

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

                                        context = await AddorUpdateToContext<soft_Channel_Product_Price>(context, newobj, line, 100, true, percent, null);

                                    }
                                    else
                                    {
                                        var channelPrice = context.soft_Channel_Product_Price.FirstOrDefault(o => o.ChannelId == channel.Id && o.ProductId == product.id); //await _crud.FindBy<soft_Channel_Product_Price>(o => o.ChannelId == channel.Id && o.ProductId == product.id);

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

                                            context = await AddorUpdateToContext<soft_Channel_Product_Price>(context, newobj, line, 100, true, percent, null);

                                        }
                                        else
                                        {
                                            channelPrice.Price = int.Parse(value);

                                            channelPrice.Price_Discount = priceDisscount_Value;

                                            channelPrice.StartDate_Discount = priceDisscount_StarDate;

                                            channelPrice.Enddate_Discount = priceDisscount_EndDate;

                                            context = await AddorUpdateToContext<soft_Channel_Product_Price>(context, channelPrice, line, 100, true, percent,
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
                                        context = await AddorUpdateToContext<soft_Branches_Product_Stock>(context, newobj, line, 100, true, percent, null);
                                    }
                                    else
                                    {
                                        var stock = context.soft_Branches_Product_Stock.FirstOrDefault(o => o.BranchesId == barches.BranchesId && o.ProductId == product.id);//await _crud.FindBy<soft_Branches_Product_Stock>(o => o.BranchesId == barches.BranchesId && o.ProductId == product.id);

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
                                            context = await AddorUpdateToContext<soft_Branches_Product_Stock>(context, newobj, line, 100, true, percent, null);

                                        }
                                        else
                                        {
                                            stock.Stock_Total = double.Parse(value);

                                            context = await AddorUpdateToContext<soft_Branches_Product_Stock>(context, stock, line, 100, true, percent, o => o.Stock_Total);

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
                    }
                    try
                    {
                        if (product.id <= 0)
                        {
                            context = await AddorUpdateToContext<shop_sanpham>(context, product, line, 100, true, percent, null);

                            context = await AddorUpdateToContext<shop_image>(context, imgage, line, 100, true, percent, null);

                        }
                        else
                        {

                            context = await AddorUpdateToContext<shop_sanpham>(context, product, line, 100, true, percent,
                                                         o => o.tensp,
                                                         o => o.PriceBase,
                                                         o => o.PriceCompare,
                                                         o => o.PriceBase_Old,
                                                         o => o.PriceInput,
                                                         o => o.Status,
                                                         o => o.StatusVAT,
                                                         o => o.SuppliersId);

                        }

                        //if (line % 100 == 0)
                        //{
                        //    await Update_is_import(true, null, percent);

                        //    await _crud.SaveChangesAsync(true);
                        //}
                    }
                    catch (Exception ex)
                    {
                        lstError.Add(new shop_sanpham
                        {
                            tensp = product.tensp,
                            masp = product.masp,
                            id = line
                        });
                        context.Dispose();
                        context = new admin_softbbmEntities();
                        context.Configuration.AutoDetectChangesEnabled = false;
                    }
                }

                context.SaveChanges();

                var error = string.Empty;
                if (lstError != null && lstError.Count > 0)
                    error = new JavaScriptSerializer().Serialize(lstError.Select(o => new { masp = o.masp, tensp = o.tensp, id = o.id }));

                await Update_is_import(false, error, percent, true);

                return lstError;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
            //catch (Exception ex)
            //{

            //    lstError.Add(new shop_sanpham
            //    {
            //        tensp = ex.Message,
            //        masp = masp,
            //        id = line
            //    });

            //    var error = new JavaScriptSerializer().Serialize(lstError.Select(o => new { masp = o.masp, tensp = o.tensp, id = o.id }));

            //    await Update_is_import(false, error, -1, true);

            //    return null;
            //}
        }

    }
}