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
    }
}
