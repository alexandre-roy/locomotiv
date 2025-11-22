using System.Collections.ObjectModel;
using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.ViewModel;
using Moq;

namespace LocomotivTests.ViewModel
{
    public class HomeViewModelTest
    {
        private readonly Mock<IUserDAL> _userDALMock;
        private readonly Mock<IStationDAL> _stationDALMock;
        private readonly Mock<INavigationService> _navigationServiceMock;
        private readonly Mock<IUserSessionService> _userSessionServiceMock;
        private readonly Mock<IPredefinedRouteDAL> _predefinedRouteDALMock;
        private readonly Mock<ITrainDAL> _trainDALMock;
        private readonly HomeViewModel _viewmodel;

        public HomeViewModelTest()
        {
            _userDALMock = new Mock<IUserDAL>();
            _stationDALMock = new Mock<IStationDAL>();
            _navigationServiceMock = new Mock<INavigationService>();
            _userSessionServiceMock = new Mock<IUserSessionService>();
            _predefinedRouteDALMock = new Mock<IPredefinedRouteDAL>();
            _trainDALMock = new Mock<ITrainDAL>();

            _stationDALMock.Setup(dal => dal.GetAll())
                .Returns(new List<Station>());

            _viewmodel = new HomeViewModel(
                _userDALMock.Object,
                _navigationServiceMock.Object,
                _userSessionServiceMock.Object,
                _stationDALMock.Object,
                _predefinedRouteDALMock.Object,
                _trainDALMock.Object
            );
        }

        [Fact]
        public void Logout_ConnectedUser_LogsOutUser()
        {
            // Arrange
            _userSessionServiceMock.SetupGet(c => c.IsUserConnected)
                .Returns(true);

            // Act
            _viewmodel.LogoutCommand.Execute(null);

            // Assert: should log out user
            _userSessionServiceMock.VerifySet(c => c.ConnectedUser = null,
                Times.Once);
        }

        [Fact]
        public void Logout_ConnectedUser_NavigatesToConnectUser()
        {
            // Arrange
            _userSessionServiceMock.SetupGet(c => c.IsUserConnected)
                .Returns(true);

            // Act
            _viewmodel.LogoutCommand.Execute(null);

            // Assert: should navigate to connect user
            _navigationServiceMock.Verify(n => n.NavigateTo<ConnectUserViewModel>(),
                Times.Once);
        }

        [Fact]
        public void CanLogout_ConnectedUser_ReturnsTrue()
        {
            // Arrange
            _userSessionServiceMock.SetupGet(c => c.IsUserConnected)
                .Returns(true);

            // Act
            bool canLogout = _viewmodel.LogoutCommand.CanExecute(null);

            // Assert: should be able to log out
            Assert.True(canLogout);
        }

        [Fact]
        public void CanLogout_NoConnectedUser_ReturnsFalse()
        {
            // Arrange
            _userSessionServiceMock.SetupGet(c => c.IsUserConnected)
                .Returns(false);

            // Act
            bool canLogout = _viewmodel.LogoutCommand.CanExecute(null);

            // Assert: should not be able to log out
            Assert.False(canLogout);
        }

        [Fact]
        public void CanFindRoute_AllSelectionsMade_ReturnsTrue()
        {
            // Arrange
            Station start = new Station { Id = 1, Name = "A" };
            Station end = new Station { Id = 2, Name = "B" };
            Train train = new Train { Id = 1 };

            _stationDALMock.Setup(d => d.GetAll())
                .Returns(new List<Station>());
            _stationDALMock.Setup(d => d.GetTrainsInStation(start.Id))
                .Returns(new List<Train>());
            _predefinedRouteDALMock.Setup(d => d.GetAll())
                .Returns(new List<PredefinedRoute>());

            _viewmodel.SelectedStartStation = start;
            _viewmodel.SelectedEndStation = end;
            _viewmodel.SelectedTrain = train;

            // Act
            bool canFind = _viewmodel.FindRouteCommand.CanExecute(null);

            // Assert
            Assert.True(canFind);
        }

        [Fact]
        public void CanFindRoute_MissingSelection_ReturnsFalse()
        {
            // Arrange
            Station start = new Station { Id = 1, Name = "A" };

            _stationDALMock.Setup(d => d.GetAll())
                .Returns(new List<Station>());
            _stationDALMock.Setup(d => d.GetTrainsInStation(start.Id))
                .Returns(new List<Train>());
            _predefinedRouteDALMock.Setup(d => d.GetAll())
                .Returns(new List<PredefinedRoute>());

            _viewmodel.SelectedStartStation = start;

            // Act
            bool canFind = _viewmodel.FindRouteCommand.CanExecute(null);

            // Assert
            Assert.False(canFind);
        }

        [Fact]
        public void FindRoute_MatchingRoute_SetsCorrectSummary()
        {
            // Arrange
            Station start = new Station { Id = 1 };
            Station end = new Station { Id = 2 };
            Train train = new Train { Id = 1 };

            PredefinedRoute route = new PredefinedRoute
            {
                Name = "TestRoute",
                StartStation = start,
                EndStation = end,
                BlockIds = new List<int> { 1, 2, 3 }
            };

            _stationDALMock.Setup(d => d.GetAll())
                .Returns(new List<Station>());
            _stationDALMock.Setup(d => d.GetTrainsInStation(start.Id))
                .Returns(new List<Train>());
            _predefinedRouteDALMock.Setup(d => d.GetAll())
                .Returns(new List<PredefinedRoute> { route });

            _viewmodel.SelectedStartStation = start;
            _viewmodel.SelectedEndStation = end;
            _viewmodel.SelectedTrain = train;

            // Act
            _viewmodel.FindRouteCommand.Execute(null);

            // Assert
            Assert.Equal(
                "Itinéraire trouvé : TestRoute\nNombre de blocs : 3",
                _viewmodel.SelectedRouteSummary
            );
        }

        [Fact]
        public void FindRoute_NoMatchingRoute_SetsErrorMessage()
        {
            // Arrange
            Station start = new Station { Id = 1 };
            Station end = new Station { Id = 2 };
            Train train = new Train { Id = 1 };

            _stationDALMock.Setup(d => d.GetAll())
                .Returns(new List<Station>());
            _stationDALMock.Setup(d => d.GetTrainsInStation(start.Id))
                .Returns(new List<Train>());
            _predefinedRouteDALMock.Setup(d => d.GetAll())
                .Returns(new List<PredefinedRoute>());

            _viewmodel.SelectedStartStation = start;
            _viewmodel.SelectedEndStation = end;
            _viewmodel.SelectedTrain = train;

            // Act
            _viewmodel.FindRouteCommand.Execute(null);

            // Assert
            Assert.Equal("Aucun itinéraire trouvé entre ces deux stations.", _viewmodel.SelectedRouteSummary);
        }
    }
}