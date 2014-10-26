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

        public ObservableCollection<HistoryItemViewModel> ShareDataList { get; private set; }

        public ICommand GoBackCommand { get; private set; }
        public ICommand ReshareCommand { get; private set; }


        public HistoryPageViewModel(IShareDataRepository shareDataRepository, INavigationService navigationService, IDataTransferService dataTransferService)
        {
            ShareDataList = new ObservableCollection<HistoryItemViewModel>();

            _sharedItemsSubscription = shareDataRepository.ShareDataObservable.Subscribe(AddHistoryItem);

            GoBackCommand = new DelegateCommand(navigationService.GoBack);
            ReshareCommand = new DelegateCommand<HistoryItemViewModel>(historyItem => ShareLink(dataTransferService, historyItem));
        }

        public void Dispose()
        {
            _sharedItemsSubscription.Dispose();
        }

        private void AddHistoryItem(ShareData shareData)
        {
            ShareDataList.Add(new HistoryItemViewModel(shareData));
        }

        private static void ShareLink(IDataTransferService transferService, HistoryItemViewModel historyItem)
        {
            transferService.Share(historyItem.SharedData);
        }
    }
}
