using BBM.Business.Model.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private admin_softbbmEntities context = new admin_softbbmEntities();

        private Repository<shop_sanpham> _productRepository;
        private Repository<shop_sanpham> _productRepositoryV2;
        private Repository<shop_image> _productImgRepository;
        private Repository<soft_Branches> _brachesRepository;
        private Repository<soft_Branches_Product_Stock> _brachesStockRepository;
        private Repository<soft_Channel> _channelRepository;
        private Repository<soft_Channel_Product_Price> _channelPriceRepository;
        private Repository<sys_Employee> _employeeRepository;
        private Repository<khachhang> _customerRepository;
        private Repository<soft_Discount> _disscountRepository;
        private Repository<shop_bienthe> _variantRepository;
        private Repository<soft_Suppliers> _suppliersRepository;
        private Repository<soft_Catalog> _catalogRepository;
        private Repository<soft_Config> _configRepository;

        public IRepository<soft_Config> ConfigRepository
        {
            get
            {
                if (_configRepository == null)
                {
                    _configRepository = new Repository<soft_Config>(context);
                }
                return _configRepository;
            }
        }
        public IRepository<soft_Catalog> CatalogRepository
        {
            get
            {
                if (_catalogRepository == null)
                {
                    _catalogRepository = new CatalogRepository(context);
                }
                return _catalogRepository;
            }
        }
        public IRepository<shop_image> ImageRepository
        {
            get
            {
                if (_productImgRepository == null)
                {
                    _productImgRepository = new Repository<shop_image>(context);
                }
                return _productImgRepository;
            }
        }
        public IRepository<soft_Suppliers> SuppliersRepository
        {
            get
            {
                if (_suppliersRepository == null)
                {
                    _suppliersRepository = new Repository<soft_Suppliers>(context);
                }
                return _suppliersRepository;
            }
        }
        public IRepository<shop_bienthe> VariantRepository
        {
            get
            {
                if (_variantRepository == null)
                {
                    _variantRepository = new Repository<shop_bienthe>(context);
                }
                return _variantRepository;
            }
        }
        public IRepository<soft_Discount> DisscountRepository
        {
            get
            {
                if (_disscountRepository == null)
                {
                    _disscountRepository = new Repository<soft_Discount>(context);
                }
                return _disscountRepository;
            }
        }
        public IRepository<khachhang> CutomerRepository
        {
            get
            {
                if (_customerRepository == null)
                {
                    _customerRepository = new CustomerRepository(context);
                }
                return _customerRepository;
            }
        }
        public IRepository<shop_sanpham> ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(context);
                }
                return _productRepository;
            }
        }

        public IRepository<shop_sanpham> ProductRepositoryV2
        {
            get
            {
                if (_productRepositoryV2 == null)
                {
                    _productRepositoryV2 = new Repository<shop_sanpham>(context);
                }
                return _productRepositoryV2;
            }
        }
        public IRepository<soft_Branches> BrachesRepository
        {
            get
            {
                if (_brachesRepository == null)
                {
                    _brachesRepository = new Repository<soft_Branches>(context);
                }
                return _brachesRepository;
            }
        }
        public IRepository<soft_Branches_Product_Stock> BrachesStockRepository
        {
            get
            {
                if (_brachesStockRepository == null)
                {
                    _brachesStockRepository = new Repository<soft_Branches_Product_Stock>(context);
                }
                return _brachesStockRepository;
            }
        }
        public IRepository<soft_Channel> ChannelRepository
        {
            get
            {
                if (_channelRepository == null)
                {
                    _channelRepository = new Repository<soft_Channel>(context);
                }
                return _channelRepository;
            }
        }
        public IRepository<soft_Channel_Product_Price> ChanelPriceRepository
        {
            get
            {
                if (_channelPriceRepository == null)
                {
                    _channelPriceRepository = new Repository<soft_Channel_Product_Price>(context);
                }
                return _channelPriceRepository;
            }
        }
        public IRepository<sys_Employee> EmployeeRepository
        {
            get
            {
                if (_employeeRepository == null)
                {
                    _employeeRepository = new Repository<sys_Employee>(context);
                }
                return _employeeRepository;
            }
        }

        #region Order
        private Repository<donhang> _orderSaleRepository;
        private Repository<donhang_ct> _orderSale_DetailRepository;
        private Repository<soft_Order_Child> _orderChildRepository;
        private Repository<soft_Order> _orderInputRepository;
        private Repository<soft_Order> _orderBranchesRepository;
        private Repository<soft_Order> _soft_OrderdRepository;


        public IRepository<donhang> OrderSaleRepository
        {
            get
            {
                if (_orderSaleRepository == null)
                {
                    _orderSaleRepository = new OrderSaleRepository(context);
                }
                return _orderSaleRepository;
            }
        }
        public IRepository<donhang_ct> OrderSale_DetailRepository
        {
            get
            {
                if (_orderSale_DetailRepository == null)
                {
                    _orderSale_DetailRepository = new Repository<donhang_ct>(context);
                }
                return _orderSale_DetailRepository;
            }
        }
        public IRepository<soft_Order_Child> OrderChildRepository
        {
            get
            {
                if (_orderChildRepository == null)
                {
                    _orderChildRepository = new Repository<soft_Order_Child>(context);
                }
                return _orderChildRepository;
            }
        }
        public IRepository<soft_Order> OrderInputRepository
        {
            get
            {
                if (_orderInputRepository == null)
                {
                    _orderInputRepository = new OrderInputRepository(context);
                }
                return _orderInputRepository;
            }
        }
        public IRepository<soft_Order> OrderBranchesRepository
        {
            get
            {
                if (_orderBranchesRepository == null)
                {
                    _orderBranchesRepository = new OrderBranchRepository(context);
                }
                return _orderBranchesRepository;
            }
        }

        public IRepository<soft_Order> soft_Order
        {
            get
            {
                if (_soft_OrderdRepository == null)
                {
                    _soft_OrderdRepository = new Repository<soft_Order>(context);
                }
                return _soft_OrderdRepository;
            }
        }
        #endregion

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
