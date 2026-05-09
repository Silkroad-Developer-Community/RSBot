using RSBot.Core.Plugins;

namespace RSBot.Python
{
    public class PythonPlugin : IPlugin
    {
        public string InternalName => "RSBot.Python";
        public static PythonPlugin Instance { get; private set; }
        public PythonManager Manager { get; private set; }
        public void Initialize()
        {
            Instance = this;
            Manager = new PythonManager();
        }
        public void OnLoadCharacter() { }
    }
}
