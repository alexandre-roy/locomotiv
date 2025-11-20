using Locomotiv.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class LocomotiveDAL : ILocomotiveDAL
    {
        private readonly ApplicationDbContext _context;

        public LocomotiveDAL(ApplicationDbContext c)
        {
            _context = c;
        }

        public IList<Locomotive> GetAll()
        {
            return _context.Locomotives.ToList();
        }
    }
}
