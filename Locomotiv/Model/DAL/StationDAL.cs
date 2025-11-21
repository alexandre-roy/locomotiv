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
        /// <summary>
        /// Retrieves a station by its unique identifier, including related trains, locomotives, and wagons.
        /// </summary>
        /// <remarks>The returned station includes its associated trains, locomotives, wagons, and trains
        /// currently in the station. This method performs eager loading of related entities to ensure all relevant data
        /// is available in the result.</remarks>
        /// <param name="id">The unique identifier of the station to retrieve.</param>
        /// <returns>A <see cref="Station"/> object representing the station with the specified identifier, or <see
        /// langword="null"/> if no matching station is found.</returns>
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
              .FirstOrDefault(s => s.Id == id);
        
        }
        /// <summary>
        /// Retrieves a list of all stations, including their associated trains, locomotives, and wagons.
        /// </summary>
        /// <remarks>The returned stations include their related entities, allowing access to trains,
        /// locomotives, and wagons without additional queries. This method is intended for scenarios where complete
        /// station details are required.</remarks>
        /// <returns>A list of <see cref="Station"/> objects representing all stations in the data source. Each station includes
        /// its related trains, locomotives, and wagons. If no stations exist, returns an empty list.</returns>
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
        public IList<Train> GetAllTrain()
        {

            return _context.Trains
                .Include(t => t.Locomotives)
                .Include(t => t.Wagons)
                .ToList();
        }

        /// <summary>
        /// Retrieves a list of trains associated with the specified station.
        /// </summary>
        /// <remarks>The returned trains include related locomotive and wagon information. This method
        /// does not throw an exception if the station is not found; instead, it returns an empty list.</remarks>
        /// <param name="stationId">The unique identifier of the station for which to retrieve trains.</param>
        /// <returns>A list of <see cref="Train"/> objects assigned to the specified station. Returns an empty list if the
        /// station does not exist or has no trains.</returns>
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

        /// <summary>
        /// Retrieves a list of trains currently located at the specified station.
        /// </summary>
        /// <remarks>The returned trains include their associated locomotives and wagons. This method does
        /// not throw an exception if the station is not found; instead, it returns an empty list.</remarks>
        /// <param name="stationId">The unique identifier of the station for which to retrieve trains. Must correspond to an existing station.</param>
        /// <returns>A list of trains present at the specified station. Returns an empty list if the station does not exist or
        /// has no trains.</returns>
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
        /// <summary>
        /// Removes the specified train from all stations.
        /// </summary>
        /// <remarks>This method removes the train from both Trains and TrainsInStation collections
        /// across all stations. Useful when a train is departing or needs to be reassigned.</remarks>
        /// <param name="trainId">The unique identifier of the train to remove from all stations.</param>
        public void RemoveTrainFromAllStations(int trainId)
        {
            IList<Station> allStations = _context.Stations
                .Include(s => s.Trains)
                .Include(s => s.TrainsInStation)
                .ToList();

            Train? train = _context.Trains.Find(trainId);
            if (train == null)
                return;

            foreach (Station station in allStations)
            {
                bool modified = false;

                if (station.Trains != null && station.Trains.Contains(train))
                {
                    station.Trains.Remove(train);
                    modified = true;
                }

                if (station.TrainsInStation != null && station.TrainsInStation.Contains(train))
                {
                    station.TrainsInStation.Remove(train);
                    modified = true;
                }
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Removes the specified train from the given station, updating the station's train collections accordingly.
        /// </summary>
        /// <remarks>If the specified station or train does not exist, no changes are made. The method
        /// updates both the station's train lists and persists changes to the database context.</remarks>
        /// <param name="stationId">The unique identifier of the station from which the train will be removed.</param>
        /// <param name="trainId">The unique identifier of the train to remove from the station.</param>
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

        /// <summary>
        /// Adds a train to the specified station, either to the list of trains currently in the station or to the
        /// general list of trains associated with the station.
        /// </summary>
        /// <remarks>If the specified station or train does not exist, no changes are made. Changes are
        /// persisted to the underlying data context immediately.</remarks>
        /// <param name="stationId">The unique identifier of the station to which the train will be added.</param>
        /// <param name="trainId">The unique identifier of the train to add to the station.</param>
        /// <param name="addToTrainsInStation">If <see langword="true"/>, adds the train to the list of trains currently present in the station and removes
        /// it from the general list if necessary; otherwise, adds the train to the general list of trains associated
        /// with the station.</param>
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

                        int currentCount = station.TrainsInStation.Count;
                        bool hasCapacity = currentCount < station.Capacity;

                        if (hasCapacity)
                        {
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
        /// <summary>
        /// Adds a new train to the specified station.
        /// </summary>
        /// <remarks>If the specified station does not exist, no action is taken. The train is added to
        /// both the station's train collection and the database context.</remarks>
        /// <param name="stationId">The unique identifier of the station to which the train will be added.</param>
        /// <param name="train">The train entity to associate with the station. Cannot be null.</param>
        public void CreateTrainForStation(int stationId, Train train)
        {
            Station? station = _context.Stations
                .Include(s => s.Trains)
                .FirstOrDefault(s => s.Id == stationId);
            if (station != null)
            {
                if (station.Trains == null)
                {
                    station.Trains = new List<Train>();
                }

                _context.Trains.Add(train);
                station.Trains.Add(train);
                _context.SaveChanges();
            }
        }
        /// <summary>
        /// Permanently deletes the specified train and all of its associated locomotives and wagons from the given
        /// station.
        /// </summary>
        /// <remarks>If either the station or train does not exist, or if the train is not associated with
        /// the specified station, no changes are made. This operation removes all related data for the train and cannot
        /// be undone.</remarks>
        /// <param name="stationId">The unique identifier of the station from which the train will be removed.</param>
        /// <param name="trainId">The unique identifier of the train to be deleted.</param>
        public void DeleteTrainPermanently(int stationId, int trainId)
        {
            Station? station = _context.Stations
                .Include(s => s.Trains)
                .FirstOrDefault(s => s.Id == stationId);

            Train? train = _context.Trains
                .Include(t => t.Locomotives)
                .Include(t => t.Wagons)
                .FirstOrDefault(t => t.Id == trainId);

            if (station != null && train != null)
            {
                if (station.Trains != null && station.Trains.Contains(train))
                {
                    station.Trains.Remove(train);
                }

                if (train.Locomotives != null)
                {
                    _context.Locomotives.RemoveRange(train.Locomotives);
                }

                if (train.Wagons != null)
                {
                    _context.Wagons.RemoveRange(train.Wagons);
                }

                _context.Trains.Remove(train);
                _context.SaveChanges();
            }
        }

        public IList<PredefinedRoute> PrefefinedRouteForEachTrain()
        {
            IList<PredefinedRoute> routes = _context.PredefinedRoutes
                .Include(s => s.StartStation)
                .Include(s => s.EndStation)
                .ToList();


            return routes;
        }

        public IList<Block> GetBlocksForPredefinedRoute(List<int> idBlocks) 
        {
            IList<Block> blocks = new List<Block>();

            foreach (int id in idBlocks)
            {
                blocks.Add(_context.Blocks
                    .Include(b => b.Points)
                    .Include(b => b.CurrentTrain)
                    .FirstOrDefault(b => b.Id == id)!);
            }

            return blocks;
        }
    }
}