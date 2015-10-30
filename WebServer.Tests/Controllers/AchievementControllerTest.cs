using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Universial.Test;
using WebServer.Controllers;

namespace WebServer.Tests.Controllers
{
    public class AchievementControllerTest : TestBase<AchievementController>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new AchievementController();
        }

        [Test]
        public void Test_GetAllAchievements()
        {
            //Arrange 
            
            //Act
            var result = SystemUnderTest.GetAllAchievements();

            //Assert
            Assert.That(()=>result,Is.Not.Null);
        }

        [Test]
        public void Test_GetObtainedAchievements()
        {
            //Arrange 

            //Act
            var result = SystemUnderTest.GetObtainedAchievements();

            //Assert
            Assert.That(() => result, Is.Not.Null);
        }
    }
}
