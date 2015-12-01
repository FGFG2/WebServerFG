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
    class SmoothFlightAchievementCalculatorTest : AchievementTestBase<SmoothFlightAchievementCalculator>
    {
        public const int OnePercentStep = SmoothFlightAchievementCalculator.OnePercentStep;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new SmoothFlightAchievementCalculator();
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
        public void Test_if_percent_is_reached_in_one_flight(int percent)
        {
            //Arrange
            const long timeBetween = 6000;
            SmartPlaneUser user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            for (int i = 0; i < percent+1; i++)
            {                
                user.MotorDatas.Add(new MotorData { TimeStamp = i * timeBetween, Value = 125 });                
            }
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 110 * timeBetween, Value = false});

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(user.Achievements.First().Progress, Is.EqualTo(percent));
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
        public void Test_if_percent_is_reached_with_little_variant_data(int percent)
        {
            //Arrange            
            const int timeBetween = 6000;
            SmartPlaneUser user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            for (int i = 0; i < percent+1; i++)
            {
                user.MotorDatas.Add(new MotorData { TimeStamp = i * timeBetween, Value =(i*10) });
            }            
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 700000, Value = false });
            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(user.Achievements.First().Progress, Is.EqualTo(percent));
        }

        [Test]
        public void Test_if_no_progress_is_reached_with_alternating_values()
        {
            //Arrange
            const long timeBetween = 6000;
            SmartPlaneUser user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            for (int i = 0; i < 100 + 1; i++)
            {
                user.MotorDatas.Add(new MotorData { TimeStamp = i * timeBetween, Value = 125 });
                user.MotorDatas.Add(new MotorData { TimeStamp = (i * timeBetween)+500, Value = 225 });
            }            
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 110 * timeBetween, Value = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(user.Achievements.First().Progress, Is.EqualTo(0));
        }
    }
}
