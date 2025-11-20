using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IUserDAL _userDAL;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSessionService;
        private readonly IStationDAL _stationDAL;
        private readonly ApplicationDbContext _context;

        public User? ConnectedUser
        {
            get => _userSessionService.ConnectedUser;
        }

        public string WelcomeMessage
        {
            get => ConnectedUser == null ? "Bienvenue chère personne inconnue!" : $"Bienvenue {ConnectedUser.Prenom}!";
        }

        public bool IsAdmin => ConnectedUser?.IsAdmin ?? false;
        public bool IsEmployee => ConnectedUser != null && !ConnectedUser.IsAdmin;

        // Employee Station details
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

        // Admin Stations, Trains, Wagons, Locomotive details
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
                    _totalStations = _stationDAL?.GetAll()?.Count ?? 0;
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
                    _totalTrains = _context?.Trains?.Count() ?? 0;
                }
                return _totalTrains ?? 0;
            }
            set { _totalTrains = value; }
        }

        public int TotalTrainsInStations
        {
            get
            {
                if (!_totalTrainsInStations.HasValue && IsAdmin)
                {
                    IList<Station> stations = _stationDAL?.GetAll();
                    _totalTrainsInStations = stations?.SelectMany(s => s.TrainsInStation ?? new List<Train>()).Distinct().Count() ?? 0;
                }
                return _totalTrainsInStations ?? 0;
            }
            set { _totalTrainsInStations = value; }
        }

        public int TotalAvailableTrains
        {
            get
            {
                if (!_totalAvailableTrains.HasValue && IsAdmin)
                {
                    IList<Train> allTrains = _context?.Trains?.ToList() ?? new List<Train>();
                    IList<Station> stations = _stationDAL?.GetAll();
                    IList<Train> trainsInStations = stations?.SelectMany(s => s.TrainsInStation ?? new List<Train>()).Distinct().ToList() ?? new List<Train>();
                    _totalAvailableTrains = allTrains.Count - trainsInStations.Count;
                }
                return _totalAvailableTrains ?? 0;
            }
            set { _totalAvailableTrains = value; }
        }

        public int TotalWagons
        {
            get
            {
                if (!_totalWagons.HasValue && IsAdmin)
                {
                    _totalWagons = _context?.Wagons?.Count() ?? 0;
                }
                return _totalWagons ?? 0;
            }
            set { _totalWagons = value; }
        }

        public int TotalLocomotives
        {
            get
            {
                if (!_totalLocomotives.HasValue && IsAdmin)
                {
                    _totalLocomotives = _context?.Locomotives?.Count() ?? 0;
                }
                return _totalLocomotives ?? 0;
            }
            set { _totalLocomotives = value; }
        }

        public HomeViewModel(IUserDAL userDAL, INavigationService navigationService, IUserSessionService userSessionService, IStationDAL stationDAL, ApplicationDbContext context)
        {
            _userDAL = userDAL;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            _stationDAL = stationDAL;
            _context = context;
            LogoutCommand = new RelayCommand(Logout, CanLogout);
        }

        // Commande pour la déconnexion
        public ICommand LogoutCommand { get; set; }

        // Méthode pour gérer la déconnexion de l'utilisateur
        private void Logout()
        {
            _userSessionService.ConnectedUser = null;
            _navigationService.NavigateTo<ConnectUserViewModel>();
        }

        // Vérifie si la commande de déconnexion peut être exécutée
        private bool CanLogout()
        {
            return _userSessionService.IsUserConnected;
        }
    }
}
