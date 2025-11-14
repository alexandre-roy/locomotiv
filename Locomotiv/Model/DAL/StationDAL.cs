using System;
using System.Collections.Generic;
using System.Linq;
using Locomotiv.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Locomotiv.Model.DAL
{
    public class StationDAL : IStationDAL
    {
        private readonly ApplicationDbContext _context;

        public StationDAL(ApplicationDbContext c)
        {
            _context = c;
        }

        public Station? FindByName(string n)
        {
            return _context.Stations.FirstOrDefault(s => s.Name == n);
        }

        public IList<Station> GetAll()
        {
            return _context.Stations.Include(s => s.Trains).ToList();
        }
    }
}