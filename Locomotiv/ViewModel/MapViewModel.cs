using GMap.NET;
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

namespace Locomotiv.ViewModel
{
    public class MapViewModel : BaseViewModel
    {
        private readonly IStationDAL _stationDal;
        private readonly IBlockDAL _blockDal;

        public ObservableCollection<Station> Stations { get; set; }
        public ObservableCollection<Block> Blocks { get; set; }
        public ObservableCollection<GMapMarker> Markers { get; set; }

        public MapViewModel(IStationDAL stationDal, IBlockDAL blockDal)
        {
            _stationDal = stationDal;
            _blockDal = blockDal;

            Stations = new ObservableCollection<Station>(_stationDal.GetAll());
            Blocks = new ObservableCollection<Block>(_blockDal.GetAll());
            Markers = new ObservableCollection<GMapMarker>();

            LoadMarkers();
        }

        private void LoadMarkers()
        {
            foreach (var block in Blocks)
            {
                AddBlockMarker(block);
            }
            foreach (var station in Stations)
            {
                AddStationMarker(station);
            }


        }

        private void AddStationMarker(Station station)
        {
            var mainMarker = CreateMainMarker(station);
            var infoMarker = CreateInfoWindowMarker(station);

            var mainButton = CreateMainMarkerButton(station);
            var infoPanel = CreateInfoPanel(station);

            mainMarker.Shape = mainButton;
            infoMarker.Shape = infoPanel;

            AttachMarkerEvents(mainButton, infoPanel);

            AddMarkersToCollection(mainMarker, infoMarker);
        }

        private void AddBlockMarker(Block block)
        {
            var mainMarker = CreateBlockMarker(block);
            var infoMarker = CreateBlockInfoWindowMarker(block);

            var mainButton = CreateBlockMainMarkerButton(block);
            var infoPanel = CreateBlockInfoPanel(block);

            mainMarker.Shape = mainButton;
            infoMarker.Shape = infoPanel;

            AttachMarkerEvents(mainButton, infoPanel);

            AddMarkersToCollection(mainMarker, infoMarker);
        }

        private GMapMarker CreateMainMarker(Station station)
        {
            return new GMapMarker(new PointLatLng(station.Latitude, station.Longitude))
            {
                Offset = new Point(-16, -32)
            };
        }

        private GMapMarker CreateBlockMarker(Block block)
        {
            return new GMapMarker(new PointLatLng(block.Latitude, block.Longitude))
            {
                Offset = new Point(-16, -32)
            };
        }

        private Button CreateMainMarkerButton(Station station)
        {
            return new Button
            {
                Content = station.Name,
                Padding = new Thickness(8, 2, 8, 2),
                Height = 32,
                Background = station.Type == StationType.Station ? Brushes.Red : Brushes.Green,
                Foreground = Brushes.White,
                Cursor = Cursors.Hand
            };
        }
        private Button CreateBlockMainMarkerButton(Block block)
        {
            return new Button
            {
                Content = $"📍{block.Id}",
                Width = 32,
                Height = 32,
                Background = Brushes.Yellow,
                Foreground = Brushes.White,
                Cursor = Cursors.Hand
            };
        }


        private GMapMarker CreateInfoWindowMarker(Station station)
        {
            return new GMapMarker(new PointLatLng(station.Latitude, station.Longitude))
            {
                Offset = new Point(-100, -120)
            };
        }

        private GMapMarker CreateBlockInfoWindowMarker(Block block)
        {
            return new GMapMarker(new PointLatLng(block.Latitude, block.Longitude))
            {
                Offset = new Point(-100, -120)
            };
        }

        private Border CreateInfoPanel(Station station)
        {
            var panel = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(8),
                Width = 180,
                Visibility = Visibility.Hidden
            };

            var stack = new StackPanel();
            string infoText =
                $"Station : {station.Name}\n" +
                "Localisation : Québec, QC\n" +
                "\n" +
                "Trains attribués :\n" +
                "  - Train 101 (Montréal → Québec)\n" +
                "  - Train 205 (Québec → Ottawa)\n" +
                "\n" +
                "Voies / Quais :\n" +
                "  - Quai 1\n" +
                "  - Quai 2\n" +
                "  - Quai 3\n" +
                "\n" +
                "Signaux :\n" +
                "  - Signal S12 (Priorité)\n" +
                "  - Signal S17 (Arrêt)\n" +
                "\n" +
                "Trains actuellement en gare :\n" +
                "  - Train 101 (Arrivé 14:25)\n" +
                "  - Train 330 (Départ 14:45)\n";

            stack.Children.Add(new TextBlock
            {
                Text = infoText,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            });

            var closeBtn = new Button
            {
                Content = "Fermer",
                Width = 60,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            closeBtn.Click += (s, e) => panel.Visibility = Visibility.Hidden;

            stack.Children.Add(closeBtn);

            panel.Child = stack;

            return panel;
        }

        private Border CreateBlockInfoPanel(Block block)
        {
            var panel = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(8),
                Width = 180,
                Visibility = Visibility.Hidden
            };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = "Block",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            });

            var closeBtn = new Button
            {
                Content = "Fermer",
                Width = 60,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            closeBtn.Click += (s, e) => panel.Visibility = Visibility.Hidden;

            stack.Children.Add(closeBtn);

            panel.Child = stack;

            return panel;
        }

        private void AttachMarkerEvents(Button markerButton, Border infoPanel)
        {
            markerButton.Click += (s, e) =>
            {
                infoPanel.Visibility = infoPanel.Visibility == Visibility.Visible
                    ? Visibility.Hidden
                    : Visibility.Visible;
            };
        }

        private void AddMarkersToCollection(GMapMarker mainMarker, GMapMarker infoMarker)
        {
            Markers.Add(mainMarker);
            Markers.Add(infoMarker);
        }
    }
}
