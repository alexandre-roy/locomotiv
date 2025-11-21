using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class PredefinedRouteDal : IPredefinedRouteDAL
    {
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public PredefinedRouteDal(ApplicationDbContext c)
        {
            _context = c;
        }


        public IList<PredefinedRoute> GetAll()
        {
            return _context.PredefinedRoutes
                .Include(b => b.StartStation)
                .Include(b => b.EndStation)
                .Include(b => b.BlockIds)
                .ToList();
        }
    }
}
