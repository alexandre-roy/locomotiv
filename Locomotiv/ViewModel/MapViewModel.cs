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
        private readonly IBlockPointDAL _blockPointDal;

        public ObservableCollection<GMapMarker> Markers { get; set; }

        public MapViewModel(IStationDAL stationDal, IBlockDAL blockDal, IBlockPointDAL blockPointDal)
        {
            _stationDal = stationDal;
            _blockDal = blockDal;
            _blockPointDal = blockPointDal;

            Markers = new ObservableCollection<GMapMarker>();

            LoadPoints();
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

            var infoPanel = CreateInfoPanel(infoText);

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

        private Border CreateInfoPanel(string text)
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
