using System.Linq;
using NSubstitute;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
using WebServer.Logging;
using WebServer.Models;

namespace WebServer.Tests.BusinessLogic.AchievementCalculators
{
    public class AchievementCalculatorTest : AchievementTestBase<AchievementCalculatorImplementation>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            SystemUnderTest = new AchievementCalculatorImplementation();
        }
        
        [Test]
        public void Test_if_calculator_adds_a_achievement_to_the_user()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(()=>user.Achievements.Any(a=>a.Name.Equals("Test")),Is.True);
        }

        [Test]
        public void Test_if_calculator_does_not_add_a_achievement_twice_to_the_user()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.Count(a => a.Name.Equals("Test")), Is.EqualTo(1));
        }

        [Test]
        public void Test_if_calculator_does_not_calculate_the_progress_when_progress_is_100()
        {
            //Arrange 
            var user = CreateSmartPlaneUser();
            user.Achievements.Add(new Achievement
            {
                Name = "Test",
                ImageUrl = "",
                Description = "TestDescription",
                Progress = 100
            });

            //Act
            SystemUnderTest.CalculateAchievementProgress(user);

            //Assert
            Assert.That(() => user.Achievements.First(a => a.Name.Equals("Test")).Progress, Is.EqualTo(100));
        }
    }

    public class AchievementCalculatorImplementation : AchievementCalculator
    {
        public AchievementCalculatorImplementation() : base("Test", Substitute.For<ILoggerFacade>())
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            return 50;
        }

        protected override Achievement CreateAchievement()
        {
            return new Achievement
            {
                Name = "Test", ImageUrl = "",Description = "TestDescription", Progress = 0
            };
        }
    }
}
