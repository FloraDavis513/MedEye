using Avalonia.Interactivity;
using Avalonia.Input;

using MedEye.ViewModels;

namespace MedEye.Views
{
    public partial class ModalWindow : BaseDialogWindow<ViewModelBase>
    {
        public ModalWindow()
        {
            InitializeComponent();

            With.Click += WithClick;
            Without.Click += WithoutClick;

            AdaptToScreen();
        }

        private void WithClick(object? sender, RoutedEventArgs e)
        {
            Close("With");
            
        }

        private void WithoutClick(object? sender, RoutedEventArgs e)
        {
            Close("Without");
        }

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

            With.Width = buttonWidth;
            Without.Width = buttonWidth;
            With.FontSize = 48 * (ClientSize.Width / 1920);
            Without.FontSize = 48 * (ClientSize.Width / 1920);
            Header.FontSize = 96 * (ClientSize.Width / 1920);
        }
    }
}