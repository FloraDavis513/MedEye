using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MedEye.Views
{
    public partial class ConfirmAction : Window
    {
        private double windowWidth;
        private double windowHeight;
        private double mainWindowWidth;
        private double mainWindowHeight;

        public ConfirmAction()
        {
            InitializeComponent();

            No.Click += ExitClick;
        }

        public ConfirmAction(string message, EventHandler<RoutedEventArgs> act)
        {
            InitializeComponent();

            ConfirmText.Text = message;

            No.Click += ExitClick;
            Yes.Click += act;
            Yes.Click += ExitClick;
        }

        public ConfirmAction(double main_window_width, double main_window_height, string message, EventHandler<RoutedEventArgs> act)
        {
            InitializeComponent();

            mainWindowWidth = main_window_width;
            mainWindowHeight = main_window_height;
            windowWidth = 1055 * (main_window_width / 1920);
            windowHeight = 600 * (main_window_height / 1080);

            ConfirmText.Text = message;

            No.Click += ExitClick;
            Yes.Click += act;
            Yes.Click += ExitClick;
        }

        private void ExitClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnOpened(EventArgs e)
        {
            AdaptToScreen();
            base.OnOpened(e);
        }

        private void AdaptToScreen()
        {
            this.Width = windowWidth;
            this.Height = windowHeight;
            var buttonWidth = this.Width / 3;

            No.Width = buttonWidth;
            Yes.Width = buttonWidth;
            ConfirmText.Width = 0.9 * windowWidth;
            Yes.FontSize = 36 * (mainWindowWidth / 1920);
            No.FontSize = 36 * (mainWindowWidth / 1920);
            ConfirmText.FontSize = 48 * (mainWindowWidth / 1920);
        }
    }
}
