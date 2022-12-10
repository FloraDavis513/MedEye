using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Avalonia.Input;
using MedEye.DB;
using Avalonia;

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
            var defaultGamer = Users.GetUserById(-1);
            new SetupMenu(defaultGamer.id).Show();
            this.Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }

            base.OnKeyDown(e);
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
            this.Position = new PixelPoint( (int)(432.5 * (mainWindowWidth / 1920)),
                                            (int)(240 * (mainWindowHeight / 1080)));
            var buttonWidth = windowWidth / 3;

            With.Width = buttonWidth;
            Without.Width = buttonWidth;
            With.FontSize = 48 * (mainWindowWidth / 1920);
            Without.FontSize = 48 * (mainWindowWidth / 1920);
            Header.FontSize = 96 * (mainWindowWidth / 1920);
        }
    }
}