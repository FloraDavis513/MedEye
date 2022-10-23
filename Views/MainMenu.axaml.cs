using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System;

namespace MedEye.Views
{
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();

            var button = this.FindControl<Button>("Exit");
            Cabinet.Click += (sender, args) => new SetupMenu().Show();

            button.Click += Button_Click;
        }

        protected override void OnOpened(EventArgs e)
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
                var button_width = 2 * this.ClientSize.Width / 9;
                reg.Width = button_width;
                cab.Width = button_width;
                settings.Width = button_width;
                exit.Width = button_width;
            }
            base.OnPropertyChanged(change);
        }

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
