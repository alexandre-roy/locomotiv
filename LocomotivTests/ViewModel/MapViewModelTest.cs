using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.ViewModel;
using Moq;

namespace LocomotivTests.ViewModel
{
    public class MapViewModelTest
    {
        private readonly Mock<IStationDAL> _stationDALMock;
        private readonly Mock<IBlockDAL> _blockDALMock;
        private readonly Mock<IBlockPointDAL> _blockPointDALMock;
        private readonly MapViewModel _viewmodel;

        public MapViewModelTest()
        {
            _stationDALMock = new Mock<IStationDAL>();
            _blockDALMock = new Mock<IBlockDAL>();
            _blockPointDALMock = new Mock<IBlockPointDAL>();

            _viewmodel = new MapViewModel(
                _stationDALMock.Object,
                _blockDALMock.Object,
                _blockPointDALMock.Object
            );
        }
    }
}