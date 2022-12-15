using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MedEye.DB;
using System;
using System.Linq;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Data;
using Avalonia.Media;
using DynamicData;

namespace MedEye.Views
{
    public partial class GamerCard : Window
    {
        private Gamer current_gamer;
        private readonly DispatcherTimer close_timer = new DispatcherTimer();

        public GamerCard(int user_id)
        {
            InitializeComponent();
            var button = this.FindControl<Button>("Back");
            button.Click += ExitClick;

            var button2 = this.FindControl<Button>("Save");
            button2.Click += SaveClick;

            current_gamer = Users.GetUserById(user_id);
            FirstName.Text = current_gamer.first_name;
            SecondName.Text = current_gamer.second_name;
            LastName.Text = current_gamer.last_name;
            Sex.Text = current_gamer.sex;
            BirthDate.Text = current_gamer.birth_date;
            GamerHeader.Text = $"―――――――― Карта игрока № {current_gamer.id} ――――――――";

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);
        }

        public GamerCard()
        {
            InitializeComponent();
            var button = this.FindControl<Button>("Back");
            button.Click += ExitClick;

            var button2 = this.FindControl<Button>("Save");
            button2.Click += SaveClick;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);

            current_gamer = new Gamer { id = -1, first_name = "", second_name = "", last_name = "", birth_date = "", sex = "" };
            var gamer_list = new List<Gamer>();
            gamer_list.Add(current_gamer);
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

            var reg = this.Get<Button>("Save");
            var cab = this.Get<Button>("Back");
            if (!reg.IsInitialized || !cab.IsInitialized)
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

        private void ExitClick(object? sender, RoutedEventArgs e)
        {
            new Registry().Show();
            close_timer.Start();
        }

        private void SaveClick(object? sender, RoutedEventArgs e)
        {
            var converter = new BrushConverter();
            var base_color = (Brush)converter.ConvertFrom("#1372B7");
            FirstName.BorderBrush = base_color;
            SecondName.BorderBrush = base_color;
            LastName.BorderBrush = base_color;
            BirthDate.BorderBrush = base_color;
            Sex.BorderBrush = base_color;
            if (FirstName.Text == null || FirstName.Text == "" ||
                SecondName.Text == null || SecondName.Text == "" ||
                LastName.Text == null || LastName.Text == "" ||
                BirthDate.Text == null || BirthDate.Text == "" ||
                Sex.Text == null || Sex.Text == "")
            {
                if (FirstName.Text == null || FirstName.Text == "")
                    FirstName.BorderBrush = Brushes.Red;
                if (SecondName.Text == null || SecondName.Text == "")
                    SecondName.BorderBrush = Brushes.Red;
                if (LastName.Text == null || LastName.Text == "")
                    LastName.BorderBrush = Brushes.Red;
                if (BirthDate.Text == null || BirthDate.Text == "")
                    BirthDate.BorderBrush = Brushes.Red;
                if (Sex.Text == null || Sex.Text == "")
                    Sex.BorderBrush = Brushes.Red;
                Caption.IsVisible = true;
                return;
            }

            if (current_gamer.id == -1)
                Users.AddUser(FirstName.Text != null ? FirstName.Text : "", SecondName.Text != null ? SecondName.Text : "",
                    LastName.Text != null ? LastName.Text : "", BirthDate.Text != null ? BirthDate.Text : "", Sex.Text != null ? Sex.Text : "");
            else
            {
                Users.UpdateUser(current_gamer.id, FirstName.Text, SecondName.Text,
                    LastName.Text, BirthDate.Text, Sex.Text);
            }
            Caption.IsVisible = false;
            
            FirstName.BorderBrush = base_color;
            SecondName.BorderBrush = base_color;
            LastName.BorderBrush = base_color;
            BirthDate.BorderBrush = base_color;
            Sex.BorderBrush = base_color;
            new Registry().Show();
            close_timer.Start();
        }

        private void StatClick(object? sender, RoutedEventArgs e)
        {
            new StatTable(current_gamer.id).Show();
            close_timer.Start();
        }

        private void AdaptToScreen()
        {
            var buttonWidth = 2 * this.ClientSize.Width / 9;

            var reg = this.Get<Button>("Save");
            var cab = this.Get<Button>("Back");

            reg.Width = buttonWidth;
            cab.Width = buttonWidth;

            reg.FontSize = 32 * (this.ClientSize.Width / 1920);
            cab.FontSize = 32 * (this.ClientSize.Width / 1920);
            GamerHeader.Width = 1920 * (this.ClientSize.Width / 1920);
            
            foreach (var textBlock in GamerData.Children.OfType<TextBlock>())
            {
                textBlock.FontSize = 32 * (this.ClientSize.Width / 1920);
            }
        }
    }
}
