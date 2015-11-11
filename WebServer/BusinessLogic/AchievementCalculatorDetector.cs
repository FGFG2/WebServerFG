using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebServer.BusinessLogic
{
    /// <summary>
    /// Searches for all implementations of IAchievementCalculator.
    /// </summary>
    public class AchievementCalculatorDetector : IAchievementCalculatorDetector
    {
        /// <summary>
        /// Searches for all implementations of IAchievementCalculator inside the Assembly of IAchievementCalculator
        /// and instantiates all of them.
        /// </summary>
        /// <returns>Instances of all found IAchievementCalculator</returns>
        public IEnumerable<IAchievementCalculator> FindAllAchievementCalculator()
        {
            // It is being assumed that all calculators are located inside the same assembly as the IAchievementCalculator interface.
            var assemblyOfAchievementCalculators = Assembly.GetAssembly(typeof(IAchievementCalculator));

            return from type in assemblyOfAchievementCalculators.GetTypes()
                   let isCalculator = type.GetInterfaces().Contains(typeof (IAchievementCalculator))
                   where isCalculator && type.IsAbstract == false 
                   select Activator.CreateInstance(type) as IAchievementCalculator;
        }
    }
}