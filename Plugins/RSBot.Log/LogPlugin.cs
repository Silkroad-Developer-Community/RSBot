using RSBot.Core.Plugins;

namespace RSBot.Log
{
    public class LogPlugin : IPlugin
    {
        public string InternalName => "RSBot.Log";
        public static LogPlugin Instance { get; private set; }
        public LogManager Manager { get; private set; }

        public void Initialize() 
        {
            Instance = this;
            Manager = new LogManager();
        }
        public void OnLoadCharacter() { }
    }
}
