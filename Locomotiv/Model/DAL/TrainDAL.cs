using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Locomotiv.Model.DAL
{
    public class TrainDAL : ITrainDAL
    {
        private readonly ApplicationDbContext _context;

        public TrainDAL(ApplicationDbContext c)
        {
            _context = c;
        }

        public IList<Train> GetAll()
        {
return _context.Trains
          .Include(t => t.Locomotives)
          .Include(t => t.Wagons)
          .ToList();
        }
    }
}
