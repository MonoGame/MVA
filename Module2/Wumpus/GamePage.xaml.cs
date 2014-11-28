using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;

namespace Wumpus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly Game1 _game;
        public GamePage()
        {
            this.InitializeComponent();
        }

        public GamePage(string launchArguments)
        {
            _game = XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, this);
        }
    }
}
