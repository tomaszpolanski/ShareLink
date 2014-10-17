using System.Collections.Generic;

namespace ShareLink.Services.Interfaces
{
    public interface IApplicationDataContainer
    {
        IDictionary<string, object> Values { get; }

        void Delete();
    }
}