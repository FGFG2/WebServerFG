using System.Collections.Generic;
using System.Linq;
using System.Net;
using NSubstitute;
using NUnit.Framework;
using WebServer.BusinessLogic;
using WebServer.Controllers;
using WebServer.DataContext;
using WebServer.Logging;
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
            SystemUnderTest = new PlaneDataController(_achievementDbMock,_achievementCalculatorMock, Substitute.For<ILoggerFacade>());
            _smartPlaneTestUser = CreateSmartPlaneUser();
            _achievementDbMock.GetSmartPlaneUserById(0).ReturnsForAnyArgs(_smartPlaneTestUser);
        }

        [Test]
        public void Test_SetMotor()
        {
            //Act
            var result = SystemUnderTest.SetMotor(new Dictionary<long, int> {{1, 1}});

            //Assert
            Assert.That(()=>_smartPlaneTestUser.MotorDatas.First().Value,Is.EqualTo(1));
            Assert.That(() =>_smartPlaneTestUser.MotorDatas.First().TimeStamp,Is.EqualTo(1));
            Assert.That(()=>result.StatusCode,Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void Test_SetMotor_NoData_Empty_List()
        {
            //Act
            var result = SystemUnderTest.SetMotor(new Dictionary<long, int>());

            //Assert    
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Test_SetMotor_NoData_Null()
        {
            //Act
            var result = SystemUnderTest.SetMotor(null);

            //Assert    
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Test_SetRudder()
        {
            //Act
            var result = SystemUnderTest.SetRudder(new Dictionary<long, int> { { 1, 1 } });
            
            //Assert
            Assert.That(()=>_smartPlaneTestUser.RudderDatas.First().Value,Is.EqualTo(1));
            Assert.That(() => _smartPlaneTestUser.RudderDatas.First().TimeStamp, Is.EqualTo(1));
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void Test_SetRudder_NoData_Empty_List()
        {
            //Act
            var result = SystemUnderTest.SetRudder(new Dictionary<long, int>());

            //Assert
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Test_SetRudder_NoData_Null()
        {
            //Act
            var result = SystemUnderTest.SetRudder(null);

            //Assert    
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Test_SetIsConnected()
        {
            //Act
            var result = SystemUnderTest.SetIsConnected(new Dictionary<long, bool> { { 1, true } });

            //Assert
            Assert.That(()=>_smartPlaneTestUser.ConnectedDatas.First().IsConnected,Is.EqualTo(true));
            Assert.That(()=>_smartPlaneTestUser.ConnectedDatas.First().TimeStamp,Is.EqualTo(1));
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void Test_SetIsConnected_NoData_Empty_List()
        {
            //Act
            var result = SystemUnderTest.SetIsConnected(new Dictionary<long, bool>());

            //Assert
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Test_SetIsConnected_NoData_Null()
        {
            //Act
            var result = SystemUnderTest.SetIsConnected(null);

            //Assert
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void Test_GetMotorDatas()
        {
            //Arrange
            _smartPlaneTestUser.MotorDatas.Add(new MotorData {TimeStamp = 10,Value = 100});

            //Act
            var result = SystemUnderTest.GetMotorDatas();

            //Assert
            Assert.That(() => result.First().TimeStamp, Is.EqualTo(10));
            Assert.That(() => result.First().Value, Is.EqualTo(100));
        }

        [Test]
        public void Test_GetMotorDatas_without_data()
        {
            //Act
            var result = SystemUnderTest.GetMotorDatas();

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }

        [Test]
        public void Test_GetRudderDatas()
        {
            //Arrange
            _smartPlaneTestUser.RudderDatas.Add(new RudderData { TimeStamp = 10, Value = 100 });

            //Act
            var result = SystemUnderTest.GetRudderDatas();

            //Assert
            Assert.That(() => result.First().TimeStamp, Is.EqualTo(10));
            Assert.That(() => result.First().Value, Is.EqualTo(100));
        }

        [Test]
        public void Test_GetRudderDatas_without_data()
        {
            //Act
            var result = SystemUnderTest.GetRudderDatas();

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }

        [Test]
        public void Test_GetConnectedDatas()
        {
            //Arrange
            _smartPlaneTestUser.ConnectedDatas.Add(new ConnectedData { TimeStamp = 10, IsConnected = true });

            //Act
            var result = SystemUnderTest.GetConnectedDatas();

            //Assert
            Assert.That(() => result.First().TimeStamp, Is.EqualTo(10));
            Assert.That(() => result.First().IsConnected, Is.True);
        }

        [Test]
        public void Test_GetConnectedDatas_without_data()
        {
            //Act
            var result = SystemUnderTest.GetConnectedDatas();

            //Assert
            Assert.That(() => result.Any(), Is.False);
        }
    }
}
