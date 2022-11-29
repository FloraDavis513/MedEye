using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using MedEye.DB;
using Avalonia.Threading;

namespace MedEye.Views
{
    public partial class Registry : Window
    {
        private SortedDictionary<string, int> actual_users;
        private static readonly DispatcherTimer close_timer = new DispatcherTimer();

        public Registry()
        {
            InitializeComponent();
            ActualizeUserList();

            MainMenu.Click += ExitClick;
            Change.Click += ChangeClick;
            Add.Click += AddClick;
            Delete.Click += DeleteClick;
            Results.Click += ResultsClick;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);

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

            var reg = this.Get<Button>("Change");
            var cab = this.Get<Button>("Delete");
            var exit = this.Get<Button>("MainMenu");
            if (!reg.IsInitialized || !cab.IsInitialized || !exit.IsInitialized)
                return;

            if (change.Property.Name == "WindowState" || change.Property.Name == "Width")
            {
                AdaptToScreen();
            }
            base.OnPropertyChanged(change);
        }

        private void CloseAfterRoute(object? sender, EventArgs e)
        {
            this.Close();
            close_timer.Stop();
        }

        private void ActualizeUserList()
        {
            actual_users = Users.GetUserList();
            var users = new List<string>();
            foreach (var item in actual_users)
                users.Add(item.Key);

            UserList.Items = users;
        }

        private void AdaptToScreen()
        {
            var buttonWidth = 2 * this.ClientSize.Width / 9;

            Change.Width = buttonWidth;
            Delete.Width = buttonWidth;
            MainMenu.Width = buttonWidth;
            Add.Width = buttonWidth;
            Results.Width = buttonWidth;

            Change.FontSize = 32 * (this.ClientSize.Width / 1920);
            Delete.FontSize = 32 * (this.ClientSize.Width / 1920);
            Add.FontSize = 32 * (this.ClientSize.Width / 1920);
            Results.FontSize = 32 * (this.ClientSize.Width / 1920);
            MainMenu.FontSize = 32 * (this.ClientSize.Width / 1920);
            MainHeader.FontSize = 48 * (this.ClientSize.Width / 1920);
            PlayersHeader.FontSize = 48 * (this.ClientSize.Width / 1920);
        }

        private void ExitClick(object? sender, RoutedEventArgs e)
        {
            new MainMenu().Show();
            close_timer.Start();
        }

        private void AddClick(object? sender, RoutedEventArgs e)
        {
            new GamerCard().Show();
            close_timer.Start();
        }

        private void DeleteClick(object? sender, RoutedEventArgs e)
        {
            if (UserList.SelectedIndex == -1)
                return;
            string selected_user = (string)UserList.SelectedItem;
            Users.DeleteUserById(actual_users[selected_user]);
            ActualizeUserList();
        }

        private void ChangeClick(object? sender, RoutedEventArgs e)
        {
            if (UserList.SelectedIndex == -1)
                return;
            string selected_user = (string)UserList.SelectedItem;
            new GamerCard(actual_users[selected_user]).Show();
            close_timer.Start();
        }

        private void ResultsClick(object? sender, RoutedEventArgs e)
        {
            if (UserList.SelectedIndex == -1)
                return;
            string selected_user = (string)UserList.SelectedItem;
            new StatTable(actual_users[selected_user]).Show();
            close_timer.Start();
        }
    }
}
