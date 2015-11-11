using System.Linq;
using NUnit.Framework;
using WebServer.BusinessLogic.AchievementCalculators;
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
    }

    public class AchievementCalculatorImplementation : AchievementCalculator
    {
        public AchievementCalculatorImplementation() : base("Test")
        {
        }

        protected override int CalculateProgress(SmartPlaneUser targetUser)
        {
            return 100;
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
