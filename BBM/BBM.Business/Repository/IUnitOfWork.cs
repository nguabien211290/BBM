using BBM.Business.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Repository
{
    public interface IUnitOfWork
    {
        IRepository<shop_sanpham> ProductRepositoryV2 { get; }
        IRepository<shop_sanpham> ProductRepository { get; }
        IRepository<soft_Branches> BrachesRepository { get; }
        IRepository<soft_Branches_Product_Stock> BrachesStockRepository { get; }
        IRepository<soft_Channel> ChannelRepository { get; }
        IRepository<soft_Channel_Product_Price> ChanelPriceRepository { get; }
        IRepository<sys_Employee> EmployeeRepository { get; }
        IRepository<khachhang> CutomerRepository { get; }
        IRepository<soft_Discount> DisscountRepository { get; }
        IRepository<shop_bienthe> VariantRepository { get; }
        IRepository<soft_Suppliers> SuppliersRepository { get; }
        IRepository<soft_Catalog> CatalogRepository { get; }
        IRepository<shop_image> ImageRepository { get; }
        IRepository<soft_Config> ConfigRepository { get; }
        IRepository<donhang_chuyenphat_tp> CityRepository { get; }
        IRepository<donhang_chuyenphat_tinh> DistrictRepository { get; }
        #region Order
        IRepository<soft_Order> OrderBranchesRepository { get; }
        IRepository<donhang> OrderSaleRepository { get; }
        IRepository<donhang_ct> OrderSale_DetailRepository { get; }
        IRepository<soft_Order_Child> OrderChildRepository { get; }
        IRepository<soft_Order> OrderInputRepository { get; }
        IRepository<soft_Order> soft_Order { get; }
        #endregion
        Task SaveChanges();
    }
}
