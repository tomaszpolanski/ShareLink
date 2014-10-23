using System;
using ShareLink.Models;

namespace ShareLink.Services.Interfaces
{
    public interface IShareDataRepository
    {
        IObservable<ShareData> ShareDataObservable { get; }

        void Add(ShareData shareData);
    }
}
