using HBK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBK.Services
{
    public interface IProductService
    {
        void Save();
        IEnumerable<Product> GetFourHotProduct();
        void CreateProduct(Product product);
        void CreateProductExtend(ProductExtend productExtend);
    }

    public class ProductService : IProductService
    {
        private readonly HBKDbContext _db;

        public ProductService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public void CreateProduct(Product product)
        {
            _db.Products.Add(product);
            Save();
        }

        public void CreateProductExtend(ProductExtend productExtend)
        {
            _db.ProductExtends.Add(productExtend);
        }

        public IEnumerable<Product> GetFourHotProduct()
        {
            return _db.Products.OrderByDescending(m=>m.View).Take(4);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}