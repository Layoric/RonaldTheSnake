using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;
using RonaldTheSnake;
using MicrosoftAdvertising;
using Microsoft.Advertising;
using Microsoft.Xna.Framework;


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
            myAd.GotFocus += myAd_GotFocus;
            this.LostFocus += GamePage_LostFocus;
            
            // Create the game.
            _game = XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, this);
            _game.Exiting += _game_Exiting;
            myAd.IsEnabled = false;
            
            //myAd.Refresh();
            
        }

        void GamePage_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

        void myAd_GotFocus(object sender, RoutedEventArgs e)
        {

            //myAd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            //myAd.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void myAd_Loaded(object sender, RoutedEventArgs e)
        {
           // MetroGameWindow.Instance.Initialize(Window.Current.CoreWindow, this);
            Window.Current.Activate();
        }



        void _game_Exiting(object sender, System.EventArgs e)
        {
            Application.Current.Exit();
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
