using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        private readonly AppDBContext _db;
        public InquiryDetailRepository(AppDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InquiryDetail obj)
        {
            throw new NotImplementedException();
        }
    }
}
