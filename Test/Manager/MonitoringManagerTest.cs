using System.Net.Http;
using Manager;
using Manager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Test.Manager
{
    [TestFixture]
    internal class MonitoringManagerTest

    {
        private HttpClient _httpClient;
        private IMonitoringManager _monitoringManager;

        [OneTimeSetUp]
        public void SetUp()
        {
            _httpClient = Substitute.For<HttpClient>();
            _monitoringManager = SingletonManager.Instance.SetMonitoringManager(new MonitoringManager(_httpClient));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }

        [Test]
        public void AssertThatMonitoringCallsApi()
        {
            // actually doesn't work xd
            _monitoringManager.ResponseApi();
            _httpClient.Received().GetAsync("competitions/2000");
        }
    }
}