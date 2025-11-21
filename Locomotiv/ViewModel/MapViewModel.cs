using GMap.NET;
using GMap.NET.WindowsPresentation;
using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services.Map;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Locomotiv.ViewModel
{
    public class MapViewModel : BaseViewModel
    {
        private readonly IStationDAL _stationDal;
        private readonly IBlockDAL _blockDal;
        private readonly IBlockPointDAL _blockPointDal;
        private readonly INavigationService _navigationService;
        private readonly IStationContextService _stationContextService;
        private readonly IUserSessionService _userSessionService;

        private readonly TrainMovementService _trainMovementService;
        private readonly MapMarkerFactory _markerFactory;
        private readonly MapInfoService _infoService;

        public ObservableCollection<GMapMarker> Markers { get; set; }

        private DispatcherTimer _movementTimer;
        private Dictionary<int, TrainMovementState> _activeTrains 
            = new Dictionary<int, TrainMovementState>();

        private Dictionary<int, (GMapMarker main, GMapMarker info)> _trainMarkers 
            = new Dictionary<int, (GMapMarker, GMapMarker)>();
        private Dictionary<int, TextBlock> _infoPanels = new Dictionary<int, TextBlock>();

        public ICommand StartAllTrainsCommand { get; }
        public ICommand StopAllTrainsCommand { get; }

        public MapViewModel(
            IStationDAL stationDal,
            IBlockDAL blockDal,
            IBlockPointDAL blockPointDal,
            INavigationService navigationService,
            IStationContextService stationContextService,
            IUserSessionService userSessionService,
            TrainMovementService trainMovementService,
            MapMarkerFactory markerFactory,
            MapInfoService infoService,
            bool loadPointsOnStartup = true)
        {
            _stationDal = stationDal;
            _blockDal = blockDal;
            _blockPointDal = blockPointDal;
            _navigationService = navigationService;
            _stationContextService = stationContextService;
            _userSessionService = userSessionService;
            _trainMovementService = trainMovementService;
            _markerFactory = markerFactory;
            _infoService = infoService;

            Markers = new ObservableCollection<GMapMarker>();

            _movementTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(MapConstants.TrainMovementIntervalSeconds)
            };
            _movementTimer.Tick += OnMovementTimerTick;

            StartAllTrainsCommand = new RelayCommand(StartAllTrainsWithRoutes);
            StopAllTrainsCommand = new RelayCommand(StopAllTrains);

            if (loadPointsOnStartup)
            {
                LoadPoints();
            }
        }

        private void LoadPoints()
        {
            LoadBlockPoints();

            if (_userSessionService.ConnectedUser?.IsAdmin == true)
            {
                LoadStations();
            }

            LoadTrainsOnBlocks();
        }

        private void LoadBlockPoints()
        {
            foreach (BlockPoint blockPoint in _blockPointDal.GetAll())
            {
                CreateMarkerForPoint(
                    blockPoint,
                    $"{MapConstants.BlockPointLabelPrefix}{blockPoint.Id}",
                    MapConstants.BlockPointColor,
                    _infoService.GetBlockPointInfo(blockPoint));
            }
        }

        private void LoadStations()
        {
            foreach (Station station in _stationDal.GetAll())
            {
                Brush color = station.Type == StationType.Station
                    ? MapConstants.StationColor
                    : MapConstants.PointColor;

                Button manageButton = CreateManageTrainsButton(station);

                CreateMarkerForPoint(
                    station,
                    station.Name,
                    color,
                    _infoService.GetStationInfo(station),
                    manageButton);
            }
        }

        private void LoadTrainsOnBlocks()
        {
            foreach (Block block in _blockDal.GetAll())
            {
                if (block.CurrentTrain != null)
                {
                    CreateMarkerForTrain(block.CurrentTrain, block);
                }
            }
        }

        private void CreateMarkerForPoint(
            IMapPoint mapPoint,
            string label,
            Brush color,
            string infoText,
            Button additionalButton = null)
        {
            var (mainMarker, infoMarker) = _markerFactory.CreateMarkerPair(
                mapPoint, label, color, infoText,
                onInfoPanelCreated: panel => StoreInfoPanel(mapPoint.Id, panel),
                additionalButton: additionalButton);

            Markers.Add(mainMarker);
            Markers.Add(infoMarker);
        }

        private void CreateMarkerForTrain(Train train, Block block)
        {
            var (mainMarker, infoMarker) = _markerFactory.CreateMarkerPair(
                block,
                $"{MapConstants.TrainLabelPrefix}{train.Id}",
                MapConstants.TrainColor,
                _infoService.GetTrainInfo(train),
                onInfoPanelCreated: panel => StoreInfoPanel(train.Id, panel));

            Markers.Add(mainMarker);
            Markers.Add(infoMarker);

            _trainMarkers[train.Id] = (mainMarker, infoMarker);
        }

        private Button CreateManageTrainsButton(Station station)
        {
            Button button = new Button
            {
                Content = "Ajouter/Supprimer un train",
                Width = MapConstants.ButtonWidth,
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Left,
                Tag = station
            };

            button.Click += (s, e) =>
            {
                if (s is Button btn && btn.Tag is Station st)
                {
                    OpenTrainManagementWindow(st);
                }
            };

            return button;
        }

        private void StoreInfoPanel(int id, Border panel)
        {
            if (panel.Child is StackPanel stack && stack.Children.Count > 0)
            {
                if (stack.Children[0] is TextBlock textBlock)
                {
                    _infoPanels[id] = textBlock;
                }
            }
        }

        private void OpenTrainManagementWindow(Station station)
        {
            if (station == null) return;

            _stationContextService.CurrentStation = station;
            _navigationService.NavigateTo<TrainManagementViewModel>();
        }

        private void RefreshAllInfoPanels()
        {
            foreach (var panelEntry in _infoPanels)
            {
                int id = panelEntry.Key;
                TextBlock textBlock = panelEntry.Value;

                Station station = _stationDal.FindById(id);
                if (station != null)
                {
                    textBlock.Text = _infoService.GetStationInfo(station);
                    continue;
                }

                BlockPoint blockPoint = _blockPointDal.GetAll().FirstOrDefault(bp => bp.Id == id);
                if (blockPoint != null)
                {
                    textBlock.Text = _infoService.GetBlockPointInfo(blockPoint);
                    continue;
                }


            }
        }

        private void OnMovementTimerTick(object sender, EventArgs e)
        {
            MoveTrains();
        }

        public void StartTrainMovement(Train train, PredefinedRoute route)
        {
            if (!ValidateTrainMovement(train, route))
                return;

            IList<Block> blocks = _stationDal.GetBlocksForPredefinedRoute(route.BlockIds);

            if (blocks == null || blocks.Count == 0)
                return;

            _trainMovementService.DepartFromAllStations(train.Id);

            TrainMovementState movementState = new TrainMovementState
            {
                Train = train,
                Route = route,
                Blocks = blocks,
                CurrentBlockIndex = 0,
                IsMoving = true
            };

            _activeTrains[train.Id] = movementState;

            _trainMovementService.PlaceTrainOnBlock(train, blocks[0]);
            UpdateOrCreateTrainMarker(train, blocks[0]);

            if (!_movementTimer.IsEnabled)
                _movementTimer.Start();
        }

        private bool ValidateTrainMovement(Train train, PredefinedRoute route)
        {
            return train != null &&
                   route != null &&
                   route.BlockIds != null &&
                   route.BlockIds.Count > 0;
        }

        public void StopAllTrains()
        {
            _activeTrains.Clear();
            _movementTimer.Stop();
        }

        private void MoveTrains()
        {
            List<int> trainsToRemove = new List<int>();

            foreach (var activeTrain in _activeTrains)
            {
                if (!activeTrain.Value.IsMoving)
                    continue;

                bool trainCompleted = MoveTrainToNextBlock(activeTrain.Key, activeTrain.Value);

                if (trainCompleted)
                {
                    trainsToRemove.Add(activeTrain.Key);
                }
            }

            RemoveCompletedTrains(trainsToRemove);
            RefreshAllInfoPanels();

            if (_activeTrains.Count == 0)
                _movementTimer.Stop();
        }

        private bool MoveTrainToNextBlock(int trainId, TrainMovementState state)
        {
            Block currentBlock = state.Blocks[state.CurrentBlockIndex];
            state.CurrentBlockIndex++;

            if (state.CurrentBlockIndex >= state.Blocks.Count)
            {
                HandleTrainArrival(trainId, state, currentBlock);
                return true;
            }

            Block nextBlock = state.Blocks[state.CurrentBlockIndex];
            _trainMovementService.MoveTrainToBlock(state.Train, currentBlock, nextBlock);
            UpdateOrCreateTrainMarker(state.Train, nextBlock);

            return false;
        }

        private void HandleTrainArrival(int trainId, TrainMovementState state, Block currentBlock)
        {
            _trainMovementService.ClearTrainFromBlock(currentBlock);

            if (state.Route.EndStation != null)
            {
                Station endStation = _stationDal.FindById(state.Route.EndStation.Id);
                if (endStation != null)
                {
                    _trainMovementService.ArriveAtStation(state.Train, endStation);
                }
            }

            RemoveTrainMarker(trainId);
        }

        private void RemoveCompletedTrains(List<int> trainIds)
        {
            foreach (int trainId in trainIds)
            {
                _activeTrains.Remove(trainId);
            }
        }

        private void UpdateOrCreateTrainMarker(Train train, Block block)
        {
            if (_trainMarkers.ContainsKey(train.Id))
            {
                UpdateTrainMarkerPosition(train.Id, block);
            }
            else
            {
                CreateMarkerForTrain(train, block);
            }
        }

        private void UpdateTrainMarkerPosition(int trainId, Block block)
        {
            PointLatLng newPosition = new PointLatLng(block.Latitude, block.Longitude);

            _trainMarkers[trainId].main.Position = newPosition;
            _trainMarkers[trainId].info.Position = newPosition;
        }

        private void RemoveTrainMarker(int trainId)
        {
            if (_trainMarkers.ContainsKey(trainId))
            {
                Markers.Remove(_trainMarkers[trainId].main);
                Markers.Remove(_trainMarkers[trainId].info);
                _trainMarkers.Remove(trainId);
            }

            _infoPanels.Remove(trainId);
        }

        public void StartAllTrainsWithRoutes()
        {
            IList<PredefinedRoute> routes = _stationDal.PrefefinedRouteForEachTrain();
            IList<Train> allTrains = _stationDal.GetAllTrain();
            HashSet<int> trainIdsOnBlocks = GetTrainIdsCurrentlyOnBlocks();

            int routeIndex = 0;
            foreach (Train train in allTrains)
            {
                if (routeIndex >= routes.Count)
                    break;

                if (CanStartTrain(train.Id, trainIdsOnBlocks))
                {
                    StartTrainMovement(train, routes[routeIndex]);
                    routeIndex++;
                }
            }
        }

        private HashSet<int> GetTrainIdsCurrentlyOnBlocks()
        {
            return _blockDal.GetTrainsCurrentlyOnBlocks()
                .Select(t => t.Id)
                .ToHashSet();
        }

        private bool CanStartTrain(int trainId, HashSet<int> trainIdsOnBlocks)
        {
            return !_activeTrains.ContainsKey(trainId) &&
                   !trainIdsOnBlocks.Contains(trainId);
        }

        internal string GetStationInfo(Station station)
        {
            return _infoService.GetStationInfo(station);
        }

        internal string GetBlockInfo(BlockPoint blockPoint)
        {
            return _infoService.GetBlockPointInfo(blockPoint);
        }

    }
}
