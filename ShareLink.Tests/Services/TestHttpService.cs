using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareLink.Services;
using ShareLink.Services.Interfaces;
using ShareLink.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLink.Tests.Services
{
    [TestClass]
    public class TestHttpService
    {
        private IHttpClient _httpClient;

        [TestInitialize]
        public void Initialize()
        {
            _httpClient = A.Fake<IHttpClient>();
        }
    }
}
