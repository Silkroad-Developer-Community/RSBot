using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Plugins;
using System.Windows.Forms;

namespace RSBot.Training
{
    public class TrainingView : IBotbaseView
    {
        public string Name => "RSBot.Training";

        public string DisplayName => "Training";

        public string TabText => DisplayName;

        /// <summary>
        ///     Gets the view.
        /// </summary>
        /// <returns></returns>
        public Control View => Views.View.Instance;



        /// <summary>
        ///     Translate the botbase plugin
        /// </summary>
        /// <param name="language">The language</param>
        public void Translate()
        {
            LanguageManager.Translate(View, Kernel.Language);
        }
    }
}
