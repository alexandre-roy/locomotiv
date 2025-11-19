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

        public void Update(Station station)
        {
            _context.Stations.Update(station);
            _context.SaveChanges();
        }

        public IList<Train> GetTrainsForStation(int stationId)
        {
            var station = _context.Stations
                .Include(s => s.Trains)
                    .ThenInclude(t => t.Locomotives)
                .Include(s => s.Trains)
                    .ThenInclude(t => t.Wagons)
                .FirstOrDefault(s => s.Id == stationId);

            return station?.Trains?.ToList() ?? new List<Train>();
        }

        public IList<Train> GetTrainsInStation(int stationId)
        {
            var station = _context.Stations
                .Include(s => s.TrainsInStation)
                    .ThenInclude(t => t.Locomotives)
                .Include(s => s.TrainsInStation)
                    .ThenInclude(t => t.Wagons)
                .FirstOrDefault(s => s.Id == stationId);

            return station?.TrainsInStation?.ToList() ?? new List<Train>();
        }

        public void RemoveTrainFromStation(int stationId, int trainId)
        {
            var station = _context.Stations
                .Include(s => s.Trains)
                .Include(s => s.TrainsInStation)
                .FirstOrDefault(s => s.Id == stationId);

            if (station != null)
            {
                var trainToRemove = _context.Trains.Find(trainId);
                if (trainToRemove != null)
                {
                    if (station.Trains != null && station.Trains.Contains(trainToRemove))
                    {
                        station.Trains.Remove(trainToRemove);
                    }

                    if (station.TrainsInStation != null && station.TrainsInStation.Contains(trainToRemove))
                    {
                        station.TrainsInStation.Remove(trainToRemove);
                    }

                    _context.SaveChanges();
                }
            }
        }

        public void AddTrainToStation(int stationId, int trainId, bool addToTrainsInStation)
        {
            var station = _context.Stations
                .Include(s => s.Trains)
                .Include(s => s.TrainsInStation)
                .FirstOrDefault(s => s.Id == stationId);

            if (station != null)
            {
                var train = _context.Trains.Find(trainId);
                if (train != null)
                {
                    if (addToTrainsInStation)
                    {
                        if (station.TrainsInStation == null)
                        {
                            station.TrainsInStation = new List<Train>();
                        }
                        if (!station.TrainsInStation.Contains(train))
                        {
                            station.TrainsInStation.Add(train);
                        }
                    }
                    else
                    {
                        if (station.Trains == null)
                        {
                            station.Trains = new List<Train>();
                        }
                        if (!station.Trains.Contains(train))
                        {
                            station.Trains.Add(train);
                        }
                    }

                    _context.SaveChanges();
                }
            }
        }
    }
}