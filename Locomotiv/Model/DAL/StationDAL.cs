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
      return _context.Stations
          .Include(s => s.Trains)
              .ThenInclude(t => t.Locomotives)
          .Include(s => s.Trains)
              .ThenInclude(t => t.Wagons)
          .Include(s => s.TrainsInStation)
              .ThenInclude(t => t.Locomotives)
          .Include(s => s.TrainsInStation)
              .ThenInclude(t => t.Wagons)
          .FirstOrDefault(s => s.Name == n);        }

        public IList<Station> GetAll()
        {
      return _context.Stations
          .Include(s => s.Trains)
              .ThenInclude(t => t.Locomotives)
          .Include(s => s.Trains)
              .ThenInclude(t => t.Wagons)
          .Include(s => s.TrainsInStation)
              .ThenInclude(t => t.Locomotives)
          .Include(s => s.TrainsInStation)
              .ThenInclude(t => t.Wagons)
          .ToList();
        }
    }
}