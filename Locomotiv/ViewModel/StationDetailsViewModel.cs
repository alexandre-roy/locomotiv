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
        private Station _gare;

        public StationDetailsViewModel(Station gare)
        {
            _gare = gare;
        }

        public string Location
        {
            get => _gare.Location;

            set
            {
                if (_gare.Location != value)
                {
                    _gare.Location = value;
                    OnPropertyChanged(nameof(Location));
                    ValidateProprety(nameof(Location), value);
                }
            }
        }
    }
}
