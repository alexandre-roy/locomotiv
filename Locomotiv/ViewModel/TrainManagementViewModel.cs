using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class TrainManagementViewModel : BaseViewModel
    {
        private readonly IStationDAL _stationDAL;
        private readonly IStationContextService _stationContextService;
        private readonly INavigationService _navigationService;
        private Station? _currentStation;

        public ObservableCollection<Train> Trains { get; set; }
        public ObservableCollection<Train> AvailableTrains { get; set; }

        private Train? _selectedTrain;
        public Train? SelectedTrain
        {
            get => _selectedTrain;
            set
            {
                _selectedTrain = value;
                OnPropertyChanged();
            }
        }

        private Train? _selectedAvailableTrain;
        public Train? SelectedAvailableTrain
        {
            get => _selectedAvailableTrain;
            set
            {
                _selectedAvailableTrain = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddTrainCommand { get; set; }
        public ICommand DeleteTrainCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public TrainManagementViewModel(
            IStationDAL stationDAL,
            IStationContextService stationContextService,
            INavigationService navigationService)
        {
            _stationDAL = stationDAL;
            _stationContextService = stationContextService;
            _navigationService = navigationService;

            Trains = new ObservableCollection<Train>();
            AvailableTrains = new ObservableCollection<Train>();

            AddTrainCommand = new RelayCommand(AddTrain, CanAddTrain);
            DeleteTrainCommand = new RelayCommand(DeleteTrain, CanDeleteTrain);
            CloseCommand = new RelayCommand(Close);

            LoadData();
        }

        private void LoadData()
        {
            LoadTrainsForStation();
            LoadAvailableTrains();
        }

        private void LoadTrainsForStation()
        {
            Trains.Clear();

            _currentStation = _stationContextService.CurrentStation;

            if (_currentStation != null)
            {
                IList<Train> trainsInStation = _stationDAL.GetTrainsInStation(_currentStation.Id);

                foreach (Train train in trainsInStation)
                {
                    Trains.Add(train);
                }
            }
        }

        private void LoadAvailableTrains()
        {
            AvailableTrains.Clear();

            _currentStation = _stationContextService.CurrentStation;

            if (_currentStation != null)
            {
                IList<Train> availableTrains = _stationDAL.GetTrainsForStation(_currentStation.Id);

                foreach (Train train in availableTrains)
                {
                    AvailableTrains.Add(train);
                }
            }
        }

        private void AddTrain()
        {
            if (SelectedAvailableTrain != null && _currentStation != null)
            {
                _currentStation = _stationDAL.FindById(_currentStation.Id);

                if (_currentStation != null)
                {
                    _stationDAL.AddTrainToStation(_currentStation.Id, SelectedAvailableTrain.Id, addToTrainsInStation: true);

                    LoadData();
                    SelectedAvailableTrain = null;
                }
            }
        }



        private void DeleteTrain()
        {
            _currentStation = _stationDAL.FindById(_currentStation.Id);

            if (SelectedTrain != null && _currentStation != null)
            {
                _stationDAL.RemoveTrainFromStation(_currentStation.Id, SelectedTrain.Id);

                LoadData();
                SelectedTrain = null;
            }
        }

        private bool CanAddTrain()
        {
            return SelectedAvailableTrain != null;
        }


        private bool CanDeleteTrain()
        {
            return SelectedTrain != null;
        }

        private void Close()
        {
            _navigationService.NavigateTo<MapViewModel>();
        }
    }
}
