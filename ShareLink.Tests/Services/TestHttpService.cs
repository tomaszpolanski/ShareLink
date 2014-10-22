using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using ShareLink.Models;
using ShareLink.Services;
using ShareLink.Services.Interfaces;
using ShareLink.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareLink.Tests.Services
{
    [TestClass]
    public class TestHttpService
    {
        private IHttpClient _httpClient;
        private HttpService _httpService;

        private const string Response = "<meta name=\"description\" content=\"This is page's content.\" /><meta name=\"robots\" content=\"index, follow\" /><meta name=\"verify-v1\" content=\"x42ckCSDiernwyVbSdBDlxN0x9AgHmZz312zpWWtMf4=\" /><title>This is the title</title><link rel=\"shortcut icon\" href=\"http://3dbin.com/favicon.png\" type=\"image/x-icon\" /><link rel=\"shortcut icon\" href=\"http://anotherURL/someicofile.png\" type=\"image/x-icon\">just to make sure it works with different link ending</link><link rel=\"stylesheet\" type=\"text/css\" href=\"http://3dbin.com/css/1261391049/style.min.css\" />";
            

        [TestInitialize]
        public void Initialize()
        {
            _httpClient = A.Fake<IHttpClient>();
            _httpService = new HttpService(_httpClient);
        }

        [TestMethod]
        public async Task TestGettingTitle()
        {
            A.CallTo(() => _httpClient.GetStringAsync(A<Uri>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(Response));

            HtmlPage htmlPage = await _httpService.GetHtmlPageAsync(new Uri("http://test.test"), CancellationToken.None);

            Assert.AreEqual("This is the title", htmlPage.Title);
        }

        [TestMethod]
        public async Task TestGettingIcon()
        {
            A.CallTo(() => _httpClient.GetStringAsync(A<Uri>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(Response));

            HtmlPage htmlPage = await _httpService.GetHtmlPageAsync(new Uri("http://test.test"), CancellationToken.None);

            Assert.AreEqual("http://3dbin.com/favicon.png", htmlPage.Icon.ToString());
        }
    }
}
