using GMap.NET;
using GMap.NET.WindowsPresentation;
using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Locomotiv.View;
using Locomotiv.Utils.Commands;

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
        public ObservableCollection<GMapMarker> Markers { get; set; }

        private DispatcherTimer _movementTimer;
        private Dictionary<int, TrainMovementState> _activeTrains = new Dictionary<int, TrainMovementState>();
        private Dictionary<int, GMapMarker> _trainMarkers = new Dictionary<int, GMapMarker>();
        private Dictionary<int, GMapMarker> _trainInfoMarkers = new Dictionary<int, GMapMarker>();

        private Dictionary<int, (Station station, TextBlock textBlock)> _stationInfoPanels = new Dictionary<int, (Station, TextBlock)>();
        private Dictionary<int, (BlockPoint blockPoint, TextBlock textBlock)> _blockPointInfoPanels = new Dictionary<int, (BlockPoint, TextBlock)>();
        private Dictionary<int, (Train train, TextBlock textBlock)> _trainInfoPanels = new Dictionary<int, (Train, TextBlock)>();

        public ICommand StartAllTrainsCommand { get; }
        public ICommand StopAllTrainsCommand { get; }

        public MapViewModel(
            IStationDAL stationDal,
            IBlockDAL blockDal,
            IBlockPointDAL blockPointDal,
            INavigationService navigationService,
            IStationContextService stationContextService,
            IUserSessionService userSessionService,
            bool loadPointsOnStartup = true)
        {
            _stationDal = stationDal;
            _blockDal = blockDal;
            _blockPointDal = blockPointDal;
            _navigationService = navigationService;
            _stationContextService = stationContextService;
            _userSessionService = userSessionService;

            Markers = new ObservableCollection<GMapMarker>();

            _movementTimer = new DispatcherTimer();
            _movementTimer.Interval = TimeSpan.FromSeconds(2);
            _movementTimer.Tick += OnMovementTimerTick;

            StartAllTrainsCommand = new RelayCommand(StartAllTrainsWithRoutes);
            StopAllTrainsCommand = new RelayCommand(StopAllTrains);

            if (loadPointsOnStartup)
            {
                LoadPoints();
            }
        }
        private void OpenTrainManagementWindow(Station station)
        {
            if (station == null) return;

            _stationContextService.CurrentStation = station;

            _navigationService.NavigateTo<TrainManagementViewModel>();
        }
        private void LoadPoints()
        {

            foreach (BlockPoint blockPoint in _blockPointDal.GetAll())
                CreatePoint(blockPoint,
                    label: $"🛤️{blockPoint.Id}",
                    color: Brushes.Black,
                    infoText: GetBlockInfo(blockPoint));
            if (_userSessionService.ConnectedUser?.IsAdmin == true)
            {
                foreach (Station station in _stationDal.GetAll())
                {
                    CreatePoint(station,
                        label: station.Name,
                        color: station.Type == StationType.Station ? Brushes.Red : Brushes.Green,
                        infoText: GetStationInfo(station));
                }
            }

            foreach (Block block in _blockDal.GetAll())
            {
                if (block.CurrentTrain is not null)
                    CreatePoint(block,
                       label: $"🚆{block.CurrentTrain.Id}",
                       color: Brushes.Blue,
                       infoText: GetTrainInfo(block.CurrentTrain));
            }

        }

        private void CreatePoint(dynamic obj, string label, Brush color, string infoText)
        {
            double lat = obj.Latitude;
            double lng = obj.Longitude;

            GMapMarker mainMarker = new GMapMarker(new PointLatLng(lat, lng))
            {
                Offset = new Point(-16, -32)
            };

            GMapMarker infoMarker = new GMapMarker(new PointLatLng(lat, lng))
            {
                Offset = new Point(-100, -120)
            };

            Button button = new Button
            {
                Content = label,
                Background = color,
                Foreground = Brushes.White,
                Padding = new Thickness(8, 2, 8, 2),
                Cursor = Cursors.Hand
            };

            Border infoPanel = CreateInfoPanel(
                            infoText,
                            obj is Station,
                            obj as Station);

            StackPanel stackPanel = infoPanel.Child as StackPanel;
            if (stackPanel != null && stackPanel.Children.Count > 0)
            {
                TextBlock textBlock = stackPanel.Children[0] as TextBlock;
                if (textBlock != null)
                {
                    if (obj is Station station)
                    {
                        _stationInfoPanels[station.Id] = (station, textBlock);
                    }
                    else if (obj is BlockPoint blockPoint)
                    {
                        _blockPointInfoPanels[blockPoint.Id] = (blockPoint, textBlock);
                    }
                    else if (obj is Block block && block.CurrentTrain != null)
                    {
                        _trainInfoPanels[block.CurrentTrain.Id] = (block.CurrentTrain, textBlock);
                    }
                }
            }

            mainMarker.Shape = button;
            infoMarker.Shape = infoPanel;

            button.Click += (s, e) =>
            {
                infoPanel.Visibility =
                    infoPanel.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            };

            Markers.Add(mainMarker);
            Markers.Add(infoMarker);
        }

        private Border CreateInfoPanel(string text, bool isStation, Station station = null)
        {
            Border panel = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(8),
                Width = 300,
                Visibility = Visibility.Hidden
            };

            StackPanel stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            });

            if (isStation && station != null && _userSessionService.ConnectedUser?.IsAdmin == true)
            {
                Button addRemoveTrainBtn = new Button
                {
                    Content = "Ajouter/Supprimer un train",
                    Width = 200,
                    Margin = new Thickness(0, 0, 0, 10),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = station
                };

                addRemoveTrainBtn.Click += (s, e) =>
                {
                    Button btn = s as Button;
                    Station currentStation = btn.Tag as Station;
                    OpenTrainManagementWindow(currentStation);
                };

                stack.Children.Add(addRemoveTrainBtn);
            }

            Button closeBtn = new Button
            {
                Content = "Fermer",
                Width = 80,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            closeBtn.Click += (s, e) => panel.Visibility = Visibility.Hidden;

            stack.Children.Add(closeBtn);

            panel.Child = stack;

            return panel;
        }

        internal string GetStationInfo(Station st)
        {
            string header =
                $"🏢 Station : {st.Name}\n" +
                $"📍 Localisation : ({st.Latitude}, {st.Longitude})";

            string assignedTrains =
                st.Trains != null && st.Trains.Count > 0
                ? string.Join("\n", st.Trains.Select(t => $"   • 🚉 Train {t.Id}"))
                : "   Aucun train attribué";

            string trainsInStation =
                st.TrainsInStation != null && st.TrainsInStation.Count > 0
                ? string.Join("\n", st.TrainsInStation.Select(t => $"   • 🚉 Train {t.Id}"))
                : "   Aucun train actuellement en gare";

            string signals = "   Aucun signal enregistré";

            return
                $"{header}\n\n" +
                $"🚆 Trains attribués :\n{assignedTrains}\n\n" +
                $"🚉 Trains en gare :\n{trainsInStation}\n\n" +
                $"🚦 Signaux :\n{signals}";
        }

        internal string GetBlockInfo(BlockPoint blockPoint)
        {
            IList<Block> blocks = _blockDal.GetAll();
            List<string> connectedBlocks = new List<string>();

            foreach (Block block in blocks)
            {
                if (block.Points.Any(p => p.Id == blockPoint.Id))
                {
                    BlockPoint otherPoint = block.Points.FirstOrDefault(p => p.Id != blockPoint.Id);

                    string status = block.CurrentTrain != null
                        ? "Train présent"
                        : "Libre";

                    if (otherPoint != null)
                    {
                        connectedBlocks.Add(
                            $" - Block {block.Id} ({status}) → vers BlockPoint {otherPoint.Id}"
                        );
                    }
                    else
                    {
                        connectedBlocks.Add(
                            $" - Block {block.Id} ({status}) → (point unique)"
                        );
                    }
                }
            }

            string blocksInfo = string.Join("\n", connectedBlocks);

            return $"🛤️ BlockPoint {blockPoint.Id}\n\nBlocs connectés :\n{blocksInfo}";
        }

        private string GetTrainInfo(Train train)
        {
            return "Petit train va loin";
        }

        /// <summary>
        /// Refreshes all info panels to reflect current state
        /// </summary>
        private void RefreshAllInfoPanels()
        {
            foreach (var infoStation in _stationInfoPanels)
            {
                Station station = _stationDal.FindById(infoStation.Key);
                if (station != null)
                {
                    infoStation.Value.textBlock.Text = GetStationInfo(station);
                }
            }

            foreach (var infoBlock in _blockPointInfoPanels)
            {
                infoBlock.Value.textBlock.Text = GetBlockInfo(infoBlock.Value.blockPoint);
            }

            foreach (var infoTrain in _trainInfoPanels)
            {
                infoTrain.Value.textBlock.Text = GetTrainInfo(infoTrain.Value.train);
            }
        }

        /// <summary>
        /// Timer tick event - moves all active trains one step
        /// </summary>
        private void OnMovementTimerTick(object sender, EventArgs e)
        {
            MoveTrains();
        }

        /// <summary>
        /// Starts movement for a specific train along a predefined route
        /// </summary>
        public void StartTrainMovement(Train train, PredefinedRoute route)
        {
            if (train == null || route == null || route.BlockIds == null || route.BlockIds.Count == 0)
                return;

            IList<Block> blocks = _stationDal.GetBlocksForPredefinedRoute(route.BlockIds);

            if (blocks == null || blocks.Count == 0)
                return;

            _stationDal.RemoveTrainFromAllStations(train.Id);

            TrainMovementState movementState = new TrainMovementState
            {
                Train = train,
                Route = route,
                Blocks = blocks,
                CurrentBlockIndex = 0,
                IsMoving = true
            };

            _activeTrains[train.Id] = movementState;

            Block firstBlock = blocks[0];
            firstBlock.CurrentTrain = train;
            train.Latitude = firstBlock.Latitude;
            train.Longitude = firstBlock.Longitude;

            UpdateTrainMarker(train, firstBlock);

            if (!_movementTimer.IsEnabled)
                _movementTimer.Start();
        }

        /// <summary>
        /// Stops movement for a specific train
        /// </summary>
        public void StopTrainMovement(int trainId)
        {
            if (_activeTrains.ContainsKey(trainId))
            {
                _activeTrains[trainId].IsMoving = false;
                _activeTrains.Remove(trainId);
            }

            if (_activeTrains.Count == 0)
                _movementTimer.Stop();
        }

        /// <summary>
        /// Stops all train movements
        /// </summary>
        public void StopAllTrains()
        {
            _activeTrains.Clear();
            _movementTimer.Stop();
        }

        /// <summary>
        /// Moves all active trains to their next block
        /// </summary>
        private void MoveTrains()
        {
            List<int> trainsToRemove = new List<int>();

            foreach (var kvp in _activeTrains)
            {
                int trainId = kvp.Key;
                TrainMovementState state = kvp.Value;

                if (!state.IsMoving)
                    continue;

                Block currentBlock = state.Blocks[state.CurrentBlockIndex];
                currentBlock.CurrentTrain = null;

                state.CurrentBlockIndex++;

                if (state.CurrentBlockIndex >= state.Blocks.Count)
                {
                    trainsToRemove.Add(trainId);

                    if (state.Route.EndStation != null)
                    {
                        Station endStation = _stationDal.FindById(state.Route.EndStation.Id);
                        if (endStation != null)
                        {
                            int currentTrainsInStation = endStation.TrainsInStation?.Count ?? 0;
                            bool hasCapacity = currentTrainsInStation < endStation.Capacity;

                            _stationDal.AddTrainToStation(endStation.Id, trainId, addToTrainsInStation: hasCapacity);
                        }
                    }

                    RemoveTrainMarker(trainId);
                    continue;
                }

                Block nextBlock = state.Blocks[state.CurrentBlockIndex];
                nextBlock.CurrentTrain = state.Train;
                state.Train.Latitude = nextBlock.Latitude;
                state.Train.Longitude = nextBlock.Longitude;

                UpdateTrainMarker(state.Train, nextBlock);
            }

            foreach (int trainId in trainsToRemove)
            {
                _activeTrains.Remove(trainId);
            }

            RefreshAllInfoPanels();

            if (_activeTrains.Count == 0)
                _movementTimer.Stop();
        }

        /// <summary>
        /// Updates or creates a marker for a train at its current position
        /// </summary>
        private void UpdateTrainMarker(Train train, Block block)
        {
            if (_trainMarkers.ContainsKey(train.Id))
            {
                GMapMarker marker = _trainMarkers[train.Id];
                marker.Position = new PointLatLng(block.Latitude, block.Longitude);

                if (_trainInfoMarkers.ContainsKey(train.Id))
                {
                    _trainInfoMarkers[train.Id].Position = new PointLatLng(block.Latitude, block.Longitude);
                }
            }
            else
            {
                GMapMarker mainMarker = new GMapMarker(new PointLatLng(block.Latitude, block.Longitude))
                {
                    Offset = new Point(-16, -32)
                };

                GMapMarker infoMarker = new GMapMarker(new PointLatLng(block.Latitude, block.Longitude))
                {
                    Offset = new Point(-100, -120)
                };

                Button button = new Button
                {
                    Content = $"🚆{train.Id}",
                    Background = Brushes.Blue,
                    Foreground = Brushes.White,
                    Padding = new Thickness(8, 2, 8, 2),
                    Cursor = Cursors.Hand
                };

                Border infoPanel = CreateInfoPanel(
                    GetTrainInfo(train),
                    false,
                    null);

                StackPanel stackPanel = infoPanel.Child as StackPanel;
                if (stackPanel != null && stackPanel.Children.Count > 0)
                {
                    TextBlock textBlock = stackPanel.Children[0] as TextBlock;
                    if (textBlock != null)
                    {
                        _trainInfoPanels[train.Id] = (train, textBlock);
                    }
                }

                mainMarker.Shape = button;
                infoMarker.Shape = infoPanel;

                button.Click += (s, e) =>
                {
                    infoPanel.Visibility =
                        infoPanel.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                };

                Markers.Add(mainMarker);
                Markers.Add(infoMarker);

                _trainMarkers[train.Id] = mainMarker;
                _trainInfoMarkers[train.Id] = infoMarker;
            }
        }

        /// <summary>
        /// Removes a train marker from the map
        /// </summary>
        private void RemoveTrainMarker(int trainId)
        {
            if (_trainMarkers.ContainsKey(trainId))
            {
                Markers.Remove(_trainMarkers[trainId]);
                _trainMarkers.Remove(trainId);
            }

            if (_trainInfoMarkers.ContainsKey(trainId))
            {
                Markers.Remove(_trainInfoMarkers[trainId]);
                _trainInfoMarkers.Remove(trainId);
            }

            if (_trainInfoPanels.ContainsKey(trainId))
            {
                _trainInfoPanels.Remove(trainId);
            }
        }

        /// <summary>
        /// Starts all trains that have predefined routes
        /// </summary>
        public void StartAllTrainsWithRoutes()
        {
            IList<PredefinedRoute> routes = _stationDal.PrefefinedRouteForEachTrain();
            IList<Train> allTrains = _stationDal.GetAllTrain();

            IList<Train> trainsOnBlocks = _blockDal.GetTrainsCurrentlyOnBlocks();
            HashSet<int> trainIdsOnBlocks = trainsOnBlocks.Select(t => t.Id).ToHashSet();

            int routeIndex = 0;
            foreach (Train train in allTrains)
            {
                if (routeIndex >= routes.Count)
                    break;

                if (_activeTrains.ContainsKey(train.Id) || trainIdsOnBlocks.Contains(train.Id))
                    continue;

                StartTrainMovement(train, routes[routeIndex]);
                routeIndex++;
            }
        }

    }
}