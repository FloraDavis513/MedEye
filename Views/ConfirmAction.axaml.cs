using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MedEye.Views
{
    public partial class ConfirmAction : Window
    {
        private double windowWidth;
        private double windowHeight;

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
            var buttonWidth = this.ClientSize.Width / 3;

            No.Width = buttonWidth;
            Yes.Width = buttonWidth;
            Yes.FontSize = 36 * (this.ClientSize.Width / windowWidth);
            No.FontSize = 36 * (this.ClientSize.Width / windowWidth);
            ConfirmText.FontSize = 48 * (this.ClientSize.Width / windowWidth);
        }
    }
}
