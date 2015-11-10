using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Universial.Test;
using WebServer.BusinessLogic;
using WebServer.Controllers;
using WebServer.DataContext;
using WebServer.Models;

namespace WebServer.Tests.Controllers
{
    public class PlaneDataControllerTest :AchievementTestBase<PlaneDataController>
    {
        private IAchievementDb _achievementDbMock;
        private SmartPlaneUser _smartPlaneTestUser;
        private IAchievementCalculationManager _achievementCalculatorMock;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            _achievementDbMock = Substitute.For<IAchievementDb>();
            _achievementCalculatorMock = Substitute.For<IAchievementCalculationManager>();
            SystemUnderTest = new PlaneDataController(_achievementDbMock,_achievementCalculatorMock);
            _smartPlaneTestUser = CreateSmartPlaneUser();
            _achievementDbMock.GetSmartPlaneUserById(0).ReturnsForAnyArgs(_smartPlaneTestUser);
        }

        [Test]
        public void Test_SetMotor()
        {
            //Arrange             
            
            //Act
            var result = SystemUnderTest.SetMotor(new Dictionary<int, int> {{1, 1}});

            //Assert
            Assert.That(()=>_smartPlaneTestUser.MotorDatas.First().Value,Is.EqualTo(1));
            Assert.That(() =>_smartPlaneTestUser.MotorDatas.First().TimeStamp,Is.EqualTo(1));
            Assert.That(()=>result.StatusCode,Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void Test_SetMotor_NoData()
        {
            //Arrange
            
            //Act
            var result = SystemUnderTest.SetMotor(new Dictionary<int, int>());

            //Assert    
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }                

        [Test]
        public void Test_SetRudder()
        {
            //Arrange 
           
            //Act
            var result = SystemUnderTest.SetRudder(new Dictionary<int, int> { { 1, 1 } });
            
            //Assert
            Assert.That(()=>_smartPlaneTestUser.RudderDatas.First().Value,Is.EqualTo(1));
            Assert.That(() => _smartPlaneTestUser.RudderDatas.First().TimeStamp, Is.EqualTo(1));
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void Test_SetRudder_NoData()
        {
            //Arrange


            //Act
            var result = SystemUnderTest.SetRudder(new Dictionary<int, int>());

            //Assert
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Test_SetIsConnected()
        {
            //Arrange 

            //Act
            var result = SystemUnderTest.SetIsConnected(new Dictionary<int, bool> { { 1, true } });

            //Assert
            Assert.That(()=>_smartPlaneTestUser.ConnectedDatas.First().IsConnected,Is.EqualTo(true));
            Assert.That(()=>_smartPlaneTestUser.ConnectedDatas.First().TimeStamp,Is.EqualTo(1));
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void Test_SetIsConnected_NoData()
        {
            //Arrange

            //Act
            var result = SystemUnderTest.SetIsConnected(new Dictionary<int, bool>());

            //Assert
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
