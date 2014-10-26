using System;
using ShareLink.Models;

namespace ShareLink.Services.Interfaces
{
    public interface IDataTransferService
    {
        IObservable<string> ShareTargetObservable { get; } 
        void Share(ShareData shareData);
    }
}