using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    class SmoothRudderFlightAchievementCalculatorTest : AchievementTestBase<SmoothRudderFlightAchievementCalculator>
    {
        public const int OnePercentStep = SmoothRudderFlightAchievementCalculator.OnePercentStep;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new SmoothRudderFlightAchievementCalculator(Substitute.For<ILoggerFacade>());
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
                user.RudderDatas.Add(new RudderData { TimeStamp = i * timeBetween, Value = 125 });                
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
                user.RudderDatas.Add(new RudderData { TimeStamp = i * timeBetween, Value =(i*10) });
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
                user.RudderDatas.Add(new RudderData { TimeStamp = i * timeBetween, Value = 125 });
                user.RudderDatas.Add(new RudderData { TimeStamp = (i * timeBetween)+500, Value = 225 });
            }            
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 110 * timeBetween, Value = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(user.Achievements.First().Progress, Is.EqualTo(0));
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
        public void Test_if_progress_is_calculated_correct_over_multiple_connections()
        {
            //Arrange
            SmartPlaneUser user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            for (int i = 0; i <= 6; i++)
            {
                user.RudderDatas.Add(new RudderData { TimeStamp = i * 10000, Value = 125 });                
            }
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 60000, Value = false });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 60000, Value = true });
            for (int i = 6; i <= 12; i++)
            {             
                user.RudderDatas.Add(new RudderData { TimeStamp = (i+1) * 10000, Value = 200 });
            }
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 150000, Value = false });
            //Act
            SystemUnderTest.CalculateAchievementProgress(user);
            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(20));
        }

        [Test]
        public void Test_if_calculates_0_with_no_flights()
        {
            //Arrange
            var user = CreateSmartPlaneUser();
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 0, Value = true });
            user.ConnectedDatas.Add(new ConnectedData { TimeStamp = 1, Value = false });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First().Progress, Is.EqualTo(0));
        }
    }
}
