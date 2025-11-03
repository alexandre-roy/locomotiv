using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xaml.Schema;
using Locomotiv.Utils;
using Locomotiv.Utils.Services.Interfaces;

namespace Locomotiv.ViewModel
{
    internal class StationDetailsViewModel : BaseViewModel
    {
        private Station _station;

        public StationDetailsViewModel(Station station)
        {
            _station = station;
        }

        public string Name
        {
            get => _station.Name;
            set
            {
                if (_station.Name != value)
                {
                    _station.Name = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
