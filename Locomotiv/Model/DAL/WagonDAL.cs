using Locomotiv.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class WagonDAL : IWagonDAL
    {
        private readonly ApplicationDbContext _context;

        public WagonDAL(ApplicationDbContext c)
        {
            _context = c;
        }

        public IList<Wagon> GetAll()
        {
            return _context.Wagons.ToList();
        }
    }
}
