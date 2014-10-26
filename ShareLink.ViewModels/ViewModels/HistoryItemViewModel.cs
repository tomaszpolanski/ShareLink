using System;
using Microsoft.Practices.Prism.Mvvm;
using ShareLink.Models;

namespace ShareLink.ViewModels.ViewModels
{
    public class HistoryItemViewModel : BindableBase
    {
        public readonly ShareData SharedData;

        public string Title
        {
            get { return SharedData.Title; }
        }

        public Uri Uri
        {
            get { return SharedData.Uri; }
        }

        public string Description
        {
            get
            {
                return string.IsNullOrEmpty(SharedData.ApplicationName) ? string.Empty :
                    "Shared with: " + SharedData.ApplicationName;
            }
        }

        public HistoryItemViewModel(ShareData shareData)
        {
            SharedData = shareData;
        }
    }
}