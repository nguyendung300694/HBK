using HBK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HBK.Services
{
    public interface IProductCategoryService
    {
        IEnumerable<ProductCategory> GetAllProductCatrgory();
        ProductCategory GetProductCategoryById(int? id);
        void Save();
    }
    public class ProductCategoryService: IProductCategoryService
    {
        private readonly HBKDbContext _db;

        public ProductCategoryService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public IEnumerable<ProductCategory> GetAllProductCatrgory()
        {
            return _db.ProductCategorys;
        }

        public ProductCategory GetProductCategoryById(int? id)
        {
            return _db.ProductCategorys.Find(id);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}