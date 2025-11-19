using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.Interfaces
{
    public interface IStationDAL
    {
        Station? FindByName(string name);
        IList<Station> GetAll();
        void Update(Station station);
        IList<Train> GetTrainsForStation(int stationId);
        IList<Train> GetTrainsInStation(int stationId);
        void RemoveTrainFromStation(int stationId, int trainId);
        void AddTrainToStation(int stationId, int trainId, bool addToTrainsInStation);
    }
}
