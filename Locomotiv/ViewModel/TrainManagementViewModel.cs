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
        private readonly ITrainDAL _trainDAL;
        private readonly ILocomotiveDAL _locomotiveDAL;
        private readonly IWagonDAL _wagonDAL;
        private readonly IStationDAL _stationDAL;
        private readonly IStationContextService _stationContextService;
        private Station? _currentStation;

        public ObservableCollection<Train> Trains { get; set; }
        public ObservableCollection<Locomotive> AvailableLocomotives { get; set; }
        public ObservableCollection<Wagon> AvailableWagons { get; set; }
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

        private TrainType _newTrainType;
        public TrainType NewTrainType
        {
            get => _newTrainType;
            set
            {
                _newTrainType = value;
                OnPropertyChanged();
            }
        }

        private PriorityLevel _newTrainPriority;
        public PriorityLevel NewTrainPriority
        {
            get => _newTrainPriority;
            set
            {
                _newTrainPriority = value;
                OnPropertyChanged();
            }
        }

        private TrainState _newTrainState;
        public TrainState NewTrainState
        {
            get => _newTrainState;
            set
            {
                _newTrainState = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Locomotive> SelectedLocomotives { get; set; }
        public ObservableCollection<Wagon> SelectedWagons { get; set; }

        public Array TrainTypes => Enum.GetValues(typeof(TrainType));
        public Array PriorityLevels => Enum.GetValues(typeof(PriorityLevel));
        public Array TrainStates => Enum.GetValues(typeof(TrainState));

        public ICommand AddTrainCommand { get; set; }
        public ICommand DeleteTrainCommand { get; set; }
        public ICommand AddLocomotiveToTrainCommand { get; set; }
        public ICommand AddWagonToTrainCommand { get; set; }

        public TrainManagementViewModel(
            ITrainDAL trainDAL,
            ILocomotiveDAL locomotiveDAL,
            IWagonDAL wagonDAL,
            IStationDAL stationDAL,
            IStationContextService stationContextService)
        {
            _trainDAL = trainDAL;
            _locomotiveDAL = locomotiveDAL;
            _wagonDAL = wagonDAL;
            _stationDAL = stationDAL;
            _stationContextService = stationContextService;

            Trains = new ObservableCollection<Train>();
            AvailableLocomotives = new ObservableCollection<Locomotive>();
            AvailableWagons = new ObservableCollection<Wagon>();
            SelectedLocomotives = new ObservableCollection<Locomotive>();
            SelectedWagons = new ObservableCollection<Wagon>();

            AddTrainCommand = new RelayCommand(AddTrain, CanAddTrain);
            DeleteTrainCommand = new RelayCommand(DeleteTrain, CanDeleteTrain);
            AddLocomotiveToTrainCommand = new RelayCommand(AddLocomotiveToTrain);
            AddWagonToTrainCommand = new RelayCommand(AddWagonToTrain);

            NewTrainType = TrainType.Passenger;
            NewTrainPriority = PriorityLevel.Medium;
            NewTrainState = TrainState.InStation;
            if (_currentStation == null)
                _currentStation = _stationContextService.CurrentStation;

            if (_currentStation != null)
                _currentStation = _stationDAL.FindByName(_currentStation.Name);


            LoadData();
        }

        private void LoadData()
        {
            var locomotives = _locomotiveDAL.GetAll();
            AvailableLocomotives.Clear();
            foreach (var loco in locomotives)
            {
                AvailableLocomotives.Add(loco);
            }

            var wagons = _wagonDAL.GetAll();
            AvailableWagons.Clear();
            foreach (var wagon in wagons)
            {
                AvailableWagons.Add(wagon);
            }

            LoadTrainsForStation();
        }

        private void LoadTrainsForStation()
        {
            Trains.Clear();

            _currentStation = _stationContextService.CurrentStation;

            if (_currentStation != null)
            {
                var trainsInStation = _stationDAL.GetTrainsInStation(_currentStation.Id);

                foreach (var train in trainsInStation)
                {
                    Trains.Add(train);
                }

            }
        }

        private void AddTrain()
        {
            _currentStation = _stationDAL.FindByName(_currentStation.Name);

            var newTrain = new Train
            {
                TypeOfTrain = NewTrainType,
                PriotityLevel = NewTrainPriority,
                State = NewTrainState,
                Locomotives = new List<Locomotive>(SelectedLocomotives),
                Wagons = new List<Wagon>(SelectedWagons)
            };

            _trainDAL.Add(newTrain);

            if (_currentStation != null)
            {
                _stationDAL.AddTrainToStation(_currentStation.Id, newTrain.Id, addToTrainsInStation: true);
            }

            SelectedLocomotives.Clear();
            SelectedWagons.Clear();

            LoadTrainsForStation();
        }

        private bool CanAddTrain()
        {
            return SelectedLocomotives.Count > 0;
        }

        private void DeleteTrain()
        {
            _currentStation = _stationDAL.FindByName(_currentStation.Name);

            if (SelectedTrain != null && _currentStation != null)
            {
                _stationDAL.RemoveTrainFromStation(_currentStation.Id, SelectedTrain.Id);

                LoadTrainsForStation();
                SelectedTrain = null;
            }
        }

        private bool CanDeleteTrain()
        {
            return SelectedTrain != null;
        }

        private void AddLocomotiveToTrain()
        {
            // This will be handled by the UI with multi-select
        }

        private void AddWagonToTrain()
        {
            // This will be handled by the UI with multi-select
        }
    }
}
