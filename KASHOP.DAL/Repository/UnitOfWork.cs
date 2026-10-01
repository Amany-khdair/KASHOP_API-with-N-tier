using KASHOP.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ICategoryRepository? _categoryRepository;
        private IProductRepository? _productRepository;
        private ICartRepository? _cartRepository;
        private IOrderRepository? _orderRepository;
        public IOrderRepository OrderRepository
        {
            get
            {
                if (_orderRepository is null)
                {
                    _orderRepository = new OrderRepository(_context);
                }
                return _orderRepository;
            }
        }

        public ICategoryRepository CategoryRepository { 
            get{
                if(_categoryRepository is null)
                {
                    _categoryRepository = new CategoryRepository(_context);
                }
                return _categoryRepository;
            } }
        public IProductRepository ProductRepository { 
            get{
                if(_productRepository is null)
                {
                    _productRepository = new ProductRepository(_context);
                }
                return _productRepository;
            } }

        public ICartRepository CartRepository
        {
            get
            {
                if(_cartRepository is null)
                {
                    _cartRepository = new CartRepository(_context);
                }
                return _cartRepository;
            }
        }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;            
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
