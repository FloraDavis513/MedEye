using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace MedEye.Views
{
    public partial class Disclaimer : Window
    {
        private readonly DispatcherTimer closeTimer = new DispatcherTimer();

        public Disclaimer()
        {
            InitializeComponent();

            ToMainMenu.Click += MainMenuClick;

            closeTimer.Tick += CloseAfterRoute;
            closeTimer.Interval = new TimeSpan(1000000);
        }

        protected override void OnOpened(EventArgs e)
        {
            AdaptToScreen();
            base.OnOpened(e);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            if (!this.IsInitialized)
                return;

            if (change.Property.Name == "WindowState" || change.Property.Name == "Width")
                AdaptToScreen();

            base.OnPropertyChanged(change);
        }

        private void MainMenuClick(object? sender, RoutedEventArgs e)
        {
            new MainMenu().Show();
            closeTimer.Start();
        }

        private void CloseAfterRoute(object? sender, EventArgs e)
        {
            this.Close();
            closeTimer.Stop();
        }

        private void AdaptToScreen()
        {
            var buttonWidth = 2 * this.ClientSize.Width / 9;

            ToMainMenu.Width = buttonWidth;

            ToMainMenu.FontSize = 32 * (this.ClientSize.Width / 1920);
            DisclaimerHeader.FontSize = 96 * (this.ClientSize.Width / 1920);
            DisclaimerText.FontSize = 48 * (this.ClientSize.Width / 1920);
        }
    }
}
