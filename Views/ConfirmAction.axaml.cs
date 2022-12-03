using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MedEye.Views
{
    public partial class ConfirmAction : Window
    {
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

        private void ExitClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AdaptToScreen()
        {
            var buttonWidth = 2 * this.ClientSize.Width / 9;

            Yes.Width = buttonWidth;
            No.Width = buttonWidth;

            Yes.FontSize = 32 * (this.ClientSize.Width / 1920);
            No.FontSize = 32 * (this.ClientSize.Width / 1920);

            ConfirmText.FontSize = 48 * (this.ClientSize.Width / 1920);
        }
    }
}
