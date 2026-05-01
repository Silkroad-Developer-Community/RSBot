using RSBot.Core.Plugins;
using RSBot.Statistics.Stats;

namespace RSBot.Statistics
{
    public class StatisticsPlugin : IPlugin
    {
        public string InternalName => "RSBot.Statistics";
        public void Initialize()
        {
            CalculatorRegistry.Initialize();
        }
        public void OnLoadCharacter() { }
    }
}
