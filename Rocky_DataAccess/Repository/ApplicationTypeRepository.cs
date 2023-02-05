using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class AppplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly AppDBContext _db;

        public AppplicationTypeRepository(AppDBContext db) : base(db) 
        {
            _db= db;
        }

        public void Update(ApplicationType applicationType)
        {
            var objFromDb = FirstOrDefault(u => u.Id == applicationType.Id);
            if(objFromDb != null)
            {
                objFromDb.Name= applicationType.Name;
            }
        }
    }
}
