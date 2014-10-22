using System;

namespace Services.Interfaces
{
    public interface IDataTransferService
    {
        void Share(string title, string description, Uri webLink, Uri icon);
    }
}