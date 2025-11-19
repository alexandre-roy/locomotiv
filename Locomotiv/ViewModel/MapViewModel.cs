using GMap.NET;
using GMap.NET.WindowsPresentation;
using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        public ObservableCollection<GMapMarker> Markers { get; set; }

        public MapViewModel(
            IStationDAL stationDal,
            IBlockDAL blockDal,
            IBlockPointDAL blockPointDal,
            INavigationService navigationService,
            IStationContextService stationContextService)
        {
            _stationDal = stationDal;
            _blockDal = blockDal;
            _blockPointDal = blockPointDal;
            _navigationService = navigationService;
            _stationContextService = stationContextService;

            Markers = new ObservableCollection<GMapMarker>();

            LoadPoints();
        }
        private void OpenTrainManagementWindow(Station station)
        {
            if (station == null) return;

            _stationContextService.CurrentStation = station;

            _navigationService.NavigateTo<TrainManagementViewModel>();
        }
        private void LoadPoints()
        {

            foreach (var blockPoint in _blockPointDal.GetAll())
                CreatePoint(blockPoint,
                    label: $"🛤️{blockPoint.Id}",
                    color: Brushes.Black,
                    infoText: GetBlockInfo(blockPoint));

            foreach (var station in _stationDal.GetAll())
                CreatePoint(station,
                    label: station.Name,
                    color: station.Type == StationType.Station ? Brushes.Red : Brushes.Green,
                    infoText: GetStationInfo(station));
        }

        private void CreatePoint(dynamic obj, string label, Brush color, string infoText)
        {
            double lat = obj.Latitude;
            double lng = obj.Longitude;

            var mainMarker = new GMapMarker(new PointLatLng(lat, lng))
            {
                Offset = new Point(-16, -32)
            };

            var infoMarker = new GMapMarker(new PointLatLng(lat, lng))
            {
                Offset = new Point(-100, -120)
            };

            var button = new Button
            {
                Content = label,
                Background = color,
                Foreground = Brushes.White,
                Padding = new Thickness(8, 2, 8, 2),
                Cursor = Cursors.Hand
            };

            var infoPanel = CreateInfoPanel(
                            infoText,
                            obj is Station,
                            obj as Station);

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
            var panel = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(8),
                Width = 300,
                Visibility = Visibility.Hidden
            };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            });

            if (isStation && station != null)
            {
                var addRemoveTrainBtn = new Button
                {
                    Content = "Ajouter/Supprimer un train",
                    Width = 200,
                    Margin = new Thickness(0, 0, 0, 10),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = station
                };

                addRemoveTrainBtn.Click += (s, e) =>
                {
                    var btn = s as Button;
                    var currentStation = btn.Tag as Station;
                    OpenTrainManagementWindow(currentStation);
                };

                stack.Children.Add(addRemoveTrainBtn);
            }

            var closeBtn = new Button
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

        private string GetStationInfo(Station st)
        {
            return
                $"Station : {st.Name}\n" +
                "Arrivés :\n" +
                "  - Train 101 (Montréal → Québec)\n" +
                "  - Train 205 (Québec → Ottawa)\n" +
                "\n" +
                "Départs :\n" +
                "  - Train 101 (Montréal → Québec)\n" +
                "  - Train 205 (Québec → Ottawa)";
        }

        private string GetBlockInfo(BlockPoint blockPoint)
        {
            var blocks = _blockDal.GetAll();
            List<string> connectedBlocks = new List<string>();

            foreach (Block block in blocks)
            {
                if (block.Points.Any(p => p.Id == blockPoint.Id))
                {
                    var otherPoint = block.Points.FirstOrDefault(p => p.Id != blockPoint.Id);

                    if (otherPoint != null)
                        connectedBlocks.Add($" - Block {block.Id} → vers BlockPoint {otherPoint.Id}");
                    else
                        connectedBlocks.Add($" - Block {block.Id} → (point unique)");
                }
            }

            string blocksInfo = string.Join("\n", connectedBlocks);

            return $"🛤️ BlockPoint {blockPoint.Id}\n\nBlocs connectés :\n{blocksInfo}";
        }
    }
}
=======
﻿using GMap.NET;
using GMap.NET.WindowsPresentation;
using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Locomotiv.View;
using Locomotiv.Utils.Commands;

namespace Locomotiv.ViewModel
{
    public class MapViewModel : BaseViewModel
    {
        private readonly IStationDAL _stationDal;
        private readonly IBlockDAL _blockDal;
        private readonly IBlockPointDAL _blockPointDal;
        public ICommand OpenTrainManagementCommand { get; }

        public ObservableCollection<GMapMarker> Markers { get; set; }

        public MapViewModel(IStationDAL stationDal, IBlockDAL blockDal, IBlockPointDAL blockPointDal, bool loadPointsOnStartup = true)
        {
            _stationDal = stationDal;
            _blockDal = blockDal;
            _blockPointDal = blockPointDal;

            Markers = new ObservableCollection<GMapMarker>();

            OpenTrainManagementCommand = new RelayCommand(OpenTrainManagementWindow);

            if (loadPointsOnStartup)
                LoadPoints();
        }
        private void OpenTrainManagementWindow()
        {
            var window = new TrainManagementWindowView();
            window.ShowDialog();
        }
        private void LoadPoints()
        {

            foreach (var blockPoint in _blockPointDal.GetAll())
                CreatePoint(blockPoint,
                    label: $"🛤️{blockPoint.Id}",
                    color: Brushes.Black,
                    infoText: GetBlockInfo(blockPoint));

            foreach (var station in _stationDal.GetAll())
                CreatePoint(station,
                    label: station.Name,
                    color: station.Type == StationType.Station ? Brushes.Red : Brushes.Green,
                    infoText: GetStationInfo(station));
        }

        public void CreatePoint(dynamic obj, string label, Brush color, string infoText)
        {
            double lat = obj.Latitude;
            double lng = obj.Longitude;

            var mainMarker = new GMapMarker(new PointLatLng(lat, lng))
            {
                Offset = new Point(-16, -32)
            };

            var infoMarker = new GMapMarker(new PointLatLng(lat, lng))
            {
                Offset = new Point(-100, -120)
            };

            var button = new Button
            {
                Content = label,
                Background = color,
                Foreground = Brushes.White,
                Padding = new Thickness(8, 2, 8, 2),
                Cursor = Cursors.Hand
            };

            var infoPanel = CreateInfoPanel(infoText, obj is Station);

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

        private Border CreateInfoPanel(string text, bool isStation)
        {
            var panel = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(8),
                Width = 300,
                Visibility = Visibility.Hidden
            };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10),
                TextWrapping = TextWrapping.Wrap
            });

            if (isStation)
            {
                var addRemoveTrainBtn = new Button
                {
                    Content = "Ajouter/Supprimer un train",
                    Width = 200,
                    Margin = new Thickness(0, 0, 0, 10),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                addRemoveTrainBtn.Click += (s, e) =>
                {
                    OpenTrainManagementCommand.Execute(null);
                };

                stack.Children.Add(addRemoveTrainBtn);
            }

            var closeBtn = new Button
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
            return
                $"Station : {st.Name}\n" +
                "Arrivés :\n" +
                "  - Train 101 (Montréal → Québec)\n" +
                "  - Train 205 (Québec → Ottawa)\n" +
                "\n" +
                "Départs :\n" +
                "  - Train 101 (Montréal → Québec)\n" +
                "  - Train 205 (Québec → Ottawa)";
        }

        internal string GetBlockInfo(BlockPoint blockPoint)
        {
            var blocks = _blockDal.GetAll();
            List<string> connectedBlocks = new List<string>();

            foreach (Block block in blocks)
            {
                if (block.Points.Any(p => p.Id == blockPoint.Id))
                {
                    var otherPoint = block.Points.FirstOrDefault(p => p.Id != blockPoint.Id);

                    if (otherPoint != null)
                        connectedBlocks.Add($" - Block {block.Id} → vers BlockPoint {otherPoint.Id}");
                    else
                        connectedBlocks.Add($" - Block {block.Id} → (point unique)");
                }
            }

            string blocksInfo = string.Join("\n", connectedBlocks);

            return $"🛤️ BlockPoint {blockPoint.Id}\n\nBlocs connectés :\n{blocksInfo}";
        }
    }
}
