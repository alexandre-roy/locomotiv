using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IUserDAL _userDAL;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;
        private readonly IStationDAL _stationDAL;
        private readonly IPredefinedRouteDAL _predefinedRouteDAL;

        public User? ConnectedUser
        {
            get => _userSessionService.ConnectedUser;
        }

        public string WelcomeMessage
        {
            get => ConnectedUser == null ? 
                "Bienvenue chère personne inconnue!" : $"Bienvenue {ConnectedUser.Prenom}!";
        }

        public bool IsAdmin => ConnectedUser?.IsAdmin ?? false;

        public bool IsEmployee => ConnectedUser != null && !ConnectedUser.IsAdmin;

        private Station? _employeeStation;
        private string _stationName;
        private int _stationCapacity;
        private int _trainsAssignedCount;
        private int _trainsInStationCount;
        private ICollection<Train>? _trainsAssigned;
        private ICollection<Train>? _trainsInStation;

        public Station? EmployeeStation
        {
            get => ConnectedUser?.Station;
            set => _employeeStation = value;
        }

        public string StationName
        {
            get => EmployeeStation?.Name ?? "Non assigné";
            set => _stationName = value;
        }

        public int StationCapacity
        {
            get => EmployeeStation?.Capacity ?? 0;
            set => _stationCapacity = value;
        }

        public int TrainsAssignedCount
        {
            get => EmployeeStation?.Trains?.Count ?? 0;
            set => _trainsAssignedCount = value;
        }

        public int TrainsInStationCount
        {
            get => EmployeeStation?.TrainsInStation?.Count ?? 0;
            set => _trainsInStationCount = value;
        }

        public ICollection<Train>? TrainsAssigned
        {
            get => EmployeeStation?.Trains;
            set => _trainsAssigned = value;
        }

        public ICollection<Train>? TrainsInStation
        {
            get => EmployeeStation?.TrainsInStation;
            set => _trainsInStation = value;
        }

        private int? _totalStations;
        private int? _totalTrains;
        private int? _totalTrainsInStations;
        private int? _totalAvailableTrains;
        private int? _totalWagons;
        private int? _totalLocomotives;

        public int TotalStations
        {
            get
            {
                if (!_totalStations.HasValue && IsAdmin)
                {
                    _totalStations = _predefinedRouteDAL?.GetAll()?.Count ?? 0;
                }
                return _totalStations ?? 0;
            }
            set { _totalStations = value; }
        }

        public int TotalTrains
        {
            get
            {
                if (!_totalTrains.HasValue && IsAdmin)
                {
                    _totalTrains = _stationDAL?.GetAllTrain()?.Count() ?? 0;
                }
                return _totalTrains ?? 0;
            }
            set { _totalTrains = value; }
        }

        public int TotalTrainsInStations
        {
            get
            {
                int compteur = 0;
                if (!_totalTrainsInStations.HasValue && IsAdmin)
                {
                    IList<Station> stations = _stationDAL?.GetAll();
                    foreach (Station station in stations ?? new List<Station>())
                    {
                        if (station.TrainsInStation is not null)
                        {
                            compteur += station.TrainsInStation.Count();
                        }
                    }
                }
                return compteur;
            }
            set { _totalTrainsInStations = value; }
        }

        public int TotalAvailableTrains
        {
            get
            {
                int compteur = 0;
                if (!_totalAvailableTrains.HasValue && IsAdmin)
                {
                    IList<Station> stations = _stationDAL?.GetAll();
                    foreach (Station station in stations ?? new List<Station>())
                    {
                        if (station.Trains is not null)
                        {
                            compteur += station.Trains.Count();
                        }
                    }
                }
                return compteur;
            }
            set { _totalAvailableTrains = value; }
        }

        public int TotalWagons
        {
            get
            {
                int compteur = 0;
                if (!_totalWagons.HasValue && IsAdmin)
                {
                    IList<Station> stations = _stationDAL?.GetAll();
                    foreach (Station station in stations ?? new List<Station>())
                    {
                        compteur += station.Trains?
                            .Sum(t => t.Wagons?.Count() ?? 0) ?? 0;
                        compteur += station.TrainsInStation?
                            .Sum(t => t.Wagons?.Count() ?? 0) ?? 0;
                    }
                }
                return compteur;
            }
            set { _totalWagons = value; }
        }

        public int TotalLocomotives
        {
            get
            {
                int compteur = 0;

                if (!_totalLocomotives.HasValue && IsAdmin)
                {
                    IList<Station> stations = _stationDAL?.GetAll();
                    foreach (Station station in stations ?? new List<Station>())
                    {
                        compteur += station.Trains?.Sum(t => t.Locomotives?.Count() ?? 0) ?? 0;
                        compteur += station.TrainsInStation?
                            .Sum(t => t.Locomotives?.Count() ?? 0) ?? 0;
                    }
                }
                return compteur;
            }
            set { _totalLocomotives = value; }
        }

        public ICommand LogoutCommand { get; set; }

        public HomeViewModel(
            IUserDAL userDAL, 
            INavigationService navigationService, 
            IUserSessionService userSessionService, 
            IStationDAL stationDAL, 
            IPredefinedRouteDAL predefinedRouteDAL
        )
        {
            _userDAL = userDAL;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            _stationDAL = stationDAL;
            _predefinedRouteDAL = predefinedRouteDAL;
            LogoutCommand = new RelayCommand(Logout, CanLogout);
        }

        private void Logout()
        {
            _userSessionService.ConnectedUser = null;
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }

        private bool CanLogout()
        {
            return _userSessionService.IsUserConnected;
        }
    }
}
