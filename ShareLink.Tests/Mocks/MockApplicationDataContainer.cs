using System.Collections.Generic;
using ShareLink.Services.Interfaces;

namespace ShareLink.Tests.Mocks
{
    internal class MockApplicationDataContainer : IApplicationDataContainer
    {
        private readonly IDictionary<string, object> _container = new Dictionary<string, object>(); 

        public IDictionary<string, object> Values { get { return _container; } }

        public void Delete()
        {
            _container.Clear();
        }
    }
}