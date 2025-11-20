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

        public Station? FindById(int id)
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
          .FirstOrDefault(s => s.Id == id);        }

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

        public IList<Train> GetTrainsForStation(int stationId)
        {
            Station? station = _context.Stations
                .Include(s => s.Trains)
                    .ThenInclude(t => t.Locomotives)
                .Include(s => s.Trains)
                    .ThenInclude(t => t.Wagons)
                .FirstOrDefault(s => s.Id == stationId);

            return station?.Trains?.ToList() ?? new List<Train>();
        }

        public IList<Train> GetTrainsInStation(int stationId)
        {
            Station? station = _context.Stations
                .Include(s => s.TrainsInStation)
                    .ThenInclude(t => t.Locomotives)
                .Include(s => s.TrainsInStation)
                    .ThenInclude(t => t.Wagons)
                .FirstOrDefault(s => s.Id == stationId);

            return station?.TrainsInStation?.ToList() ?? new List<Train>();
        }

        public void RemoveTrainFromStation(int stationId, int trainId)
        {
            Station? station = _context.Stations
                .Include(s => s.Trains)
                .Include(s => s.TrainsInStation)
                .FirstOrDefault(s => s.Id == stationId);

            if (station != null)
            {
                Train? trainToRemove = _context.Trains.Find(trainId);
                if (trainToRemove != null)
                {
                    if (station.Trains != null && station.Trains.Contains(trainToRemove))
                    {
                        station.Trains.Remove(trainToRemove);
                    }

                    if (station.TrainsInStation != null && station.TrainsInStation.Contains(trainToRemove))
                    {
                        station.TrainsInStation.Remove(trainToRemove);

                        if (station.Trains == null)
                        {
                            station.Trains = new List<Train>();
                        }
                        if (!station.Trains.Contains(trainToRemove))
                        {
                            station.Trains.Add(trainToRemove);
                        }
                    }

                    _context.SaveChanges();
                }
            }
        }

        public void AddTrainToStation(int stationId, int trainId, bool addToTrainsInStation)
        {
            Station? station = _context.Stations
                .Include(s => s.Trains)
                .Include(s => s.TrainsInStation)
                .FirstOrDefault(s => s.Id == stationId);

            if (station != null)
            {
                Train? train = _context.Trains.Find(trainId);
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

                        if (station.Trains != null && station.Trains.Contains(train))
                        {
                            station.Trains.Remove(train);
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