using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    class RestlessFlightAchievementCalculatorTest : AchievementTestBase<RestlessFlightAchievementCalculator>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new RestlessFlightAchievementCalculator();
        }

        [Test]
        public void Test_if_Calculator_behaves_good_with_no_data()
        {
            //Arrange
            SmartPlaneUser user = CreateSmartPlaneUser();
            //Act
            SystemUnderTest.CalculateAchievementProgress(user);
            //Assert
            Assert.That(user.Achievements.First().Progress, Is.EqualTo(0));
        }

        [Test]
        public void Test_if_calculates_0_with_no_flights()
        {
            //Arrange
            SmartPlaneUser user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 1, Value = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }

        [TestCase(100)]
        [TestCase(90)]
        [TestCase(80)]
        [TestCase(70)]
        [TestCase(60)]
        [TestCase(50)]
        [TestCase(40)]
        [TestCase(30)]
        [TestCase(20)]
        [TestCase(10)]
        public void Test_if_progress_is_calculated_correctly(int percent)
        {
            //Arrange
            const int timeBetween = 3000;
            SmartPlaneUser user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData {TimeStamp = 0, Value = true});
            for (int i = 0; i < percent+1; i++)
            {
                user.MotorDatas.Add(new MotorData {TimeStamp = i* timeBetween, Value = i});
            }
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 350000, Value = false });           
            //Act
            SystemUnderTest.CalculateAchievementProgress(user);
            //Assert
            Assert.That(user.Achievements.First().Progress, Is.EqualTo(percent));            
        }
    }
}
