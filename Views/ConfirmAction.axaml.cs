using Avalonia.Input;
using Avalonia.Interactivity;
using MedEye.ViewModels;

namespace MedEye.Views
{
    public partial class ConfirmAction : BaseDialogWindow<ViewModelBase>
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

            AdaptToScreen();
        }

        private void ExitClick(object? sender, RoutedEventArgs e) => Close();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
                Close();

            base.OnKeyDown(e);
        }

        private void AdaptToScreen()
        {
            Width = 1055 * (ClientSize.Width / 1920);
            Height = 600 * (ClientSize.Height / 1080);

            var buttonWidth = Width / 3;

            No.Width = buttonWidth;
            Yes.Width = buttonWidth;
            ConfirmText.Width = 0.9 * Width;
            Yes.FontSize = 36 * (ClientSize.Width / 1920);
            No.FontSize = 36 * (ClientSize.Width / 1920);
            ConfirmText.FontSize = 48 * (ClientSize.Width / 1920);
        }
    }
}
