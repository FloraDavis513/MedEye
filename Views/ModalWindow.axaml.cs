using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MedEye.Views
{
    public partial class ModalWindow : Window
    {
        private double windowWidth;
        private double windowHeight;
        private double mainWindowWidth;
        private double mainWindowHeight;
        private EventHandler<RoutedEventArgs> act;

        public ModalWindow()
        {
            InitializeComponent();

            var button = this.FindControl<Button>("With");
            button.Click += WithClick;

            var button2 = this.FindControl<Button>("Without");
            button2.Click += WithoutClick;
        }

        public ModalWindow(double main_window_width, double main_window_height, EventHandler<RoutedEventArgs> act)
        {
            InitializeComponent();

            mainWindowWidth = main_window_width;
            mainWindowHeight = main_window_height;
            windowWidth = 1055 * (main_window_width / 1920);
            windowHeight = 600 * (main_window_height / 1080);
            this.act = act;

            With.Click += WithClick;

            Without.Click += WithoutClick;
            Without.Click += act;
        }

        private void WithClick(object? sender, RoutedEventArgs e)
        {
            new GamerSelector(mainWindowWidth, mainWindowHeight, act).Show();
            this.Close();
        }

        private void WithoutClick(object? sender, RoutedEventArgs e)
        {
            new SetupMenu(-1).Show();
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

            With.Width = buttonWidth;
            Without.Width = buttonWidth;
            With.FontSize = 48 * (this.ClientSize.Width / windowWidth);
            Without.FontSize = 48 * (this.ClientSize.Width / windowWidth);
            Header.FontSize = 96 * (this.ClientSize.Width / windowWidth);
        }
    }
}
