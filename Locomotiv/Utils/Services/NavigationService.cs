using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.ViewModel;

namespace Locomotiv.Utils.Services
{
    public class NavigationService : BaseViewModel, INavigationService
    {
        private BaseViewModel _currentView;
        private Func<Type, BaseViewModel> _viewModelFactory;
        private Stack<BaseViewModel> _navigationHistory = new Stack<BaseViewModel>();

        public BaseViewModel CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public NavigationService(Func<Type, BaseViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            if (_currentView != null)
            {
                _navigationHistory.Push(_currentView);
            }

            BaseViewModel viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewModel;
        }

        public void NavigateBack()
        {
            if (_navigationHistory.Count > 0)
            {
                CurrentView = _navigationHistory.Pop();
            }
        }
    }
}
