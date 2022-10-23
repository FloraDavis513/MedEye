using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;

namespace MedEye.Views
{
    public partial class MainMenu : Window
    {
        private static readonly DispatcherTimer close_timer = new DispatcherTimer();

        public MainMenu()
        {
            InitializeComponent();

            var button = this.FindControl<Button>("Exit");
            Cabinet.Click += (sender, args) => new SetupMenu().Show();
            button.Click += ExitClick;

            var button2 = this.FindControl<Button>("Cabinet");
            button2.Click += CabinetClick;

            var button3 = this.FindControl<Button>("Registry");
            button3.Click += RegistryClick;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(10000000);
        }

        protected override void OnOpened(EventArgs e)
        {
            AdaptToScreen();

            var logo = this.Get<Image>("Logo");

            logo.Source = new Bitmap("..\\..\\..\\Assets\\logo.png");

            base.OnOpened(e);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            if (!this.IsInitialized)
                return;
            
            var reg = this.Get<Button>("Registry");
            var cab = this.Get<Button>("Cabinet");
            var settings = this.Get<Button>("Settings");
            var exit = this.Get<Button>("Exit");
            if (!reg.IsInitialized || !cab.IsInitialized || !settings.IsInitialized || !exit.IsInitialized)
                return;

            if (change.Property.Name == "WindowState" || change.Property.Name == "Width")
            {
                AdaptToScreen();
            }
            base.OnPropertyChanged(change);
        }

        private void ExitClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CabinetClick(object? sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            close_timer.Start();
        }

        private void RegistryClick(object? sender, RoutedEventArgs e)
        {
            new ModalWindow().Show();
        }

        private void CloseAfterRoute(object? sender, EventArgs e)
        {
            this.Close();
            close_timer.Stop();
        }

        private void AdaptToScreen()
        {
            var button_width = 2 * this.ClientSize.Width / 9;

            var reg = this.Get<Button>("Registry");
            var cab = this.Get<Button>("Cabinet");
            var settings = this.Get<Button>("Settings");
            var exit = this.Get<Button>("Exit");

            reg.Width = button_width;
            cab.Width = button_width;
            settings.Width = button_width;
            exit.Width = button_width;

            reg.FontSize = 32 * (this.ClientSize.Width / 1920);
            cab.FontSize = 32 * (this.ClientSize.Width / 1920);
            settings.FontSize = 32 * (this.ClientSize.Width / 1920);
            exit.FontSize = 32 * (this.ClientSize.Width / 1920);

            var h1 = this.Get<TextBlock>("MainHeader");
            var h2 = this.Get<TextBlock>("SubHeader");
            h1.FontSize = 96 * (this.ClientSize.Width / 1920);
            h2.FontSize = 48 * (this.ClientSize.Width / 1920);
        }
    }
}
