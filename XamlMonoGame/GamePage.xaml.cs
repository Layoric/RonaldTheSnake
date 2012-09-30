using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;
using RonaldTheSnake;
using MicrosoftAdvertising;
using Microsoft.Advertising;


namespace XamlMonoGame
{
    /// <summary>
    /// The root page used to display the game.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly Game1 _game;

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();
            myAd.Loaded += myAd_Loaded;
            // Create the game.
            _game = XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, this);
            
        }

        void myAd_Loaded(object sender, RoutedEventArgs e)
        {
            _game.MyFocus();
        }

        void _game_Exiting(object sender, System.EventArgs e)
        {
            
        }

        //Goback to main page when the back button is pressed
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //var mainPage = ((App)App.Current).mainPage;

            //Window.Current.Content = mainPage;
            //Window.Current.Activate();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //popUpMenu.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //popUpMenu.IsOpen = !popUpMenu.IsOpen;
        }


    }
}
