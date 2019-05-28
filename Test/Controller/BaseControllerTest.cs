using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Castle.Components.DictionaryAdapter.Xml;
using Manager;
using Manager.Interfaces;
using Microsoft.Owin;
using Models;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;

namespace Test.Controller
{
    [TestFixture]
    internal class BaseControllerTest : BaseController
    {
        private ITokenManager _tokenManager;
        private Dictionary<string, object> _data;
        private OwinContext _context;
        private AuthenticationHeaderValue _authHeader;

        private readonly string _ip = "127.0.0.1";
        private readonly string _token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ";



        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _data = new Dictionary<string, object>() { { "Authorization", _token } };
            this.Configuration = new HttpConfiguration();
            this.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/");
            _context = new OwinContext(_data);
            _authHeader = new AuthenticationHeaderValue(_token);
            this.Request.SetOwinContext(_context);
            this.Request.GetOwinContext().Request.RemoteIpAddress = _ip;
            this.Request.Headers.Authorization = _authHeader;
            SingletonManager.Instance.SetUserManager(Substitute.For<IUserManager>());
            SingletonManager.Instance.SetTokenManager(Substitute.For<ITokenManager>());
            SingletonManager.Instance.SetBetManager(Substitute.For<IBetManager>());
            SingletonManager.Instance.SetMatchManager(Substitute.For<IMatchManager>());
            SingletonManager.Instance.SetFootballDataManager(Substitute.For<IFootballDataManager>());
            SingletonManager.Instance.SetItemManager(Substitute.For<IItemManager>());
            SingletonManager.Instance.SetEmailManager(Substitute.For<IEmailManager>());
            SingletonManager.Instance.SetCompetitionManager(Substitute.For<ICompetitionManager>());
        }

        [TearDown]
        public void TearDown()
        { 
        }

        [Test]
        public void AssertThatGetUserManagerReturnsAValidResponseCodeAndCallsManager()
        {   
            Assert.IsInstanceOf<IUserManager>(GetUserManager());
        }

        [Test]
        public void AssertThatGetTokenManagerReturnsAValidResponseCodeAndCallsManager()
        {   
            Assert.IsInstanceOf<ITokenManager>(GetTokenManager());
        }

        [Test]
        public void AssertThatGetBetManagerReturnsAValidResponseCodeAndCallsManager()
        {  
            Assert.IsInstanceOf<IBetManager>(GetBetManager());
        }

        [Test]
        public void AssertThatGetMatchManagerReturnsAValidResponseCodeAndCallsManager()
        {   
            Assert.IsInstanceOf<IMatchManager>(GetMatchManager());
        }

        [Test]
        public void AssertThatGetFootballDataManagerReturnsAValidResponseCodeAndCallsManager()
        {  
            Assert.IsInstanceOf<IFootballDataManager>(GetFootballDataManager());
        }

        [Test]
        public void AssertThatGetItemManagerReturnsAValidResponseCodeAndCallsManager()
        {   
            Assert.IsInstanceOf<IItemManager>(GetItemManager());
        }

        [Test]
        public void AssertThatGetEmailManagerReturnsAValidResponseCodeAndCallsManager()
        {
            Assert.IsInstanceOf<IEmailManager>(GetEmailManager());
        }

        [Test]
        public void AssertThatGetCompetitionManagerReturnsAValidResponseCodeAndCallsManager()
        {   
            Assert.IsInstanceOf<ICompetitionManager>(GetCompetitionManager());
        }

        [Test]
        public async Task AssertThatGetUserByTokenReturnsAValidResponseCodeAndCallsManager()
        {
            GetTokenManager().GetTokenFromRequest(this.Request);
            GetTokenManager().ValidateToken(_token);
            GetTokenManager().Received().GetTokenFromRequest(Arg.Any<HttpRequestMessage>());
            GetTokenManager().Received().ValidateToken(Arg.Any<string>());
            await GetUserManager().GetUserByEmail(Arg.Any<string>());
        }
    }
}