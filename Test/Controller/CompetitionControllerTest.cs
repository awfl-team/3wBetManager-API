﻿using _3wBetManager_API.Controllers;
using DAO;
using DAO.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Test.Controller
{
    [TestFixture]
    public class CompetitionControllerTest
    {
        [SetUp]
        public void SetUp()
        {
            _competitionController = new CompetitionController();
            _competitionDao = SingletonDao.Instance.SetCompetitionDao(Substitute.For<ICompetitionDao>());
        }

        [TearDown]
        public void TearDown()
        {
            _competitionDao.ClearReceivedCalls();
        }

        private CompetitionController _competitionController;
        private ICompetitionDao _competitionDao;

        /*[Test]
        public void GetAllTest()
        {
            var getAllCompetition = _competitionController.GetAll();

            _competitionDao.Received().FindAllCompetitions();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(getAllCompetition);
        }*/
    }
}