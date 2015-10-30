using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Universial.Test;
using WebServer.Controllers;

namespace WebServer.Tests.Controllers
{
    public class PlaneDataControllerTest :TestBase<PlaneDataController>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new PlaneDataController();
        }

        [Test]
        public void Test_SetMotor()
        {
            //Arrange 
            
            //Act
            var result = SystemUnderTest.SetMotor(new Dictionary<int, int> {{1, 1}});

            //Assert
            Assert.That(()=>result.StatusCode,Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void Test_SetRudder()
        {
            //Arrange 

            //Act
            var result = SystemUnderTest.SetRuder(new Dictionary<int, int> { { 1, 1 } });

            //Assert
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public void Test_SetIsConnected()
        {
            //Arrange 

            //Act
            var result = SystemUnderTest.SetIsConnected(new Dictionary<int, bool> { { 1, true } });

            //Assert
            Assert.That(() => result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }
    }
}
