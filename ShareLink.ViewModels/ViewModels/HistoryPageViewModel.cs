using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using ShareLink.Models;
using ShareLink.Services.Interfaces;

namespace ShareLink.ViewModels.ViewModels
{
    public class HistoryPageViewModel : ViewModel, IDisposable
    {
        private readonly IDisposable _sharedItemsSubscription;

        public ObservableCollection<ShareData> ShareDataList { get; private set; }

        public ICommand GoBackCommand { get; private set; }



        public HistoryPageViewModel(IShareDataRepository shareDataRepository, INavigationService navigationService)
        {
            ShareDataList = new ObservableCollection<ShareData>();

            _sharedItemsSubscription = shareDataRepository.ShareDataObservable.Subscribe(ShareDataList.Add);

            GoBackCommand = new DelegateCommand(navigationService.GoBack);
        }

        public void Dispose()
        {
            _sharedItemsSubscription.Dispose();
        }
    }
}
