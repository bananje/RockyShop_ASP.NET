using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDBContext _db;

        public CategoryRepository(AppDBContext db) : base(db) 
        {
            _db= db;
        }

        public void Update(Category category)
        {
            var objFromDb = FirstOrDefault(u => u.Id == category.Id);
            if(objFromDb != null)
            {
                objFromDb.CategoryName = category.CategoryName;
                objFromDb.DisplayOrder= category.DisplayOrder;
            }
        }
    }
}
