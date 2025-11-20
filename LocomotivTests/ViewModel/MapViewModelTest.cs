using Locomotiv.Model.Interfaces;
using Locomotiv.ViewModel;
using Moq;

namespace LocomotivTests.ViewModel
{
    public class MapViewModelTest
    {
        private readonly Mock<IStationDAL> _stationDALMock;
        private readonly Mock<IBlockPointDAL> _blockPointsDALMock;
        private readonly Mock<IBlockDAL> _blockDALMock;
        private readonly MapViewModel _viewmodel;
        private readonly Station _station;
        private readonly List<BlockPoint> _blockPoints;
        private readonly Block _block;
        private readonly Block _blockNotConnected;

        public MapViewModelTest()
        {
            _stationDALMock = new Mock<IStationDAL>();
            _blockPointsDALMock = new Mock<IBlockPointDAL>();
            _blockDALMock = new Mock<IBlockDAL>();

            _viewmodel = new MapViewModel(
                _stationDALMock.Object,
                _blockDALMock.Object,
                _blockPointsDALMock.Object,
                false
            );

            _station = new Station
            {
                Id = 1,
                Name = "Test Station",
                Longitude = -71.204255,
                Latitude = 46.842256,
                Type = StationType.Station
            };

            _blockPoints = new List<BlockPoint>
            {
                new BlockPoint { Id = 1, Longitude = -71.204255, Latitude = 46.842256 },
                new BlockPoint { Id = 2, Longitude = -71.334879, Latitude = 46.747842 },
                new BlockPoint { Id = 3, Longitude = -71.123456, Latitude = 46.654321 }
            };

            _block = new Block
            {
                Id = 1,
                Points = new List<BlockPoint>
                {
                    _blockPoints.First(bp => bp.Id == 1),
                    _blockPoints.First(bp => bp.Id == 2)
                }
            };

            _blockNotConnected = new Block
            {
                Id = 2,
                Points = new List<BlockPoint>
                {
                    _blockPoints.First(bp => bp.Id == 3)
                }
            };

        }

        [Fact]
        public void GetStationInfo_ReturnsStationInfoString()
        {
            // Arrange
            var station = _station;

            // Act
            string stationstring = _viewmodel.GetStationInfo(station);

            // Assert
            Assert.Equal($"Station : {station.Name}\n" +
                "Arrivés :\n" +
                "  - Train 101 (Montréal → Québec)\n" +
                "  - Train 205 (Québec → Ottawa)\n" +
                "\n" +
                "Départs :\n" +
                "  - Train 101 (Montréal → Québec)\n" +
                "  - Train 205 (Québec → Ottawa)", 
                stationstring);
        }

        [Fact]
        public void GetBlockInfo_NoConnectedPoints_ReturnsCorrectBlockInfoString()
        {
            // Arrange
            var blockPoint = _blockPoints[0];

            // Act
            _blockDALMock.Setup(d => d.GetAll()).Returns(new List<Block> { _block });

            string blockstring = _viewmodel.GetBlockInfo(_blockPoints[0]);

            // Assert
            Assert.Equal(
                $"🛤️ BlockPoint {_blockPoints[0].Id}\n\n" +
                $"Blocs connectés :\n - Block {_block.Id} → vers BlockPoint {_blockPoints[1].Id}", 
                blockstring);
        }

        [Fact]
        public void GetBlockInfo_ConnectedPoints_ReturnsCorrectBlockInfoString()
        {
            // Arrange
            var blockPoint = _blockPoints[2];

            // Act
            _blockDALMock.Setup(d => d.GetAll()).Returns(new List<Block> { _blockNotConnected });

            string blockstring = _viewmodel.GetBlockInfo(_blockPoints[2]);

            // Assert
            Assert.Equal(
                $"🛤️ BlockPoint {_blockPoints[2].Id}\n\n" +
                $"Blocs connectés :\n - Block {_blockNotConnected.Id} → (point unique)",
                blockstring);
        }
    }
}
