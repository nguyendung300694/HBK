using HBK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HBK.Services

{
    public interface ICommonService
    {
        List<Common> getAllSpecialtyType();

        Common getCommonById(String Id);

        IEnumerable<CommonModelView> getAllCommon();

        void Create(Common common);

        void Destroy(Common common);

        void Save();
    }

    public class CommonService : ICommonService
    {
        private HBKDbContext _db;

        public CommonService(IDbFactory dbFactory)
        {
            _db = dbFactory.Init();
        }

        public IEnumerable<CommonModelView> getAllCommon()
        {
            List<CommonModelView> listCommon = new List<CommonModelView>();
            listCommon = _db.Commons.Select(common => new CommonModelView
            {
                ComCode = common.ComCode,
                ComSubCode = common.ComSubCode == null ? App_GlobalResources.Langues.Root : common.ComSubCode,
                CommonType = common.CommonType,
                ComName = common.ComName,
                ComName2 = common.ComName2,
                TempComSubCode = common.ComSubCode,
                IsChange = false
            }).ToList();
            return listCommon;
        }

        public List<Common> getAllSpecialtyType()
        {
            //return _db.Commons.Where(c =>c.CommonType == 1 && c.ChildCommom.Any()).ToList();
            return _db.Commons.Where(c=>c.ChildCommom.Any(b => b.CommonType == 1)).ToList();
        }

        public Common getCommonById(String Id)
        {
            return _db.Commons.Find(Id);
        }

        public void Create(Common common)
        {
            _db.Commons.Add(common);
            _db.SaveChanges();
        }

        public void Destroy(Common common)
        {
            _db.Commons.Remove(common);
            _db.SaveChanges();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}