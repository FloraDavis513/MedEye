﻿using Avalonia;
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
        private static readonly DispatcherTimer close_timer = new DispatcherTimer();

        public GamerCard(int user_id)
        {
            InitializeComponent();
            var button = this.FindControl<Button>("Back");
            button.Click += ExitClick;

            var button2 = this.FindControl<Button>("Save");
            button2.Click += SaveClick;

            Stat.Click += StatClick;

            current_gamer = Users.GetUserById(user_id);
            FirstName.Text = current_gamer.first_name;
            SecondName.Text = current_gamer.second_name;
            LastName.Text = current_gamer.last_name;
            Sex.Text = current_gamer.sex;
            BirthDate.Text = current_gamer.birth_date;
            GamerHeader.Text = $"---------------- Карта игрока № {current_gamer.id} ----------------";

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
            if (current_gamer.id == -1)
                Users.AddUser(FirstName.Text, SecondName.Text,
                    LastName.Text, BirthDate.Text, Sex.Text);
            else
            {
                Users.UpdateUser(current_gamer.id, FirstName.Text, SecondName.Text,
                    LastName.Text, BirthDate.Text, Sex.Text);
            }
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
            var stat = this.Get<Button>("Stat");

            reg.Width = buttonWidth;
            cab.Width = buttonWidth;
            stat.Width = buttonWidth;

            reg.FontSize = 32 * (this.ClientSize.Width / 1920);
            cab.FontSize = 32 * (this.ClientSize.Width / 1920);
            stat.FontSize = 32 * (this.ClientSize.Width / 1920);
            GamerHeader.Width = 1920 * (this.ClientSize.Width / 1920);
            
            foreach (var textBlock in GamerData.Children.OfType<TextBlock>())
            {
                textBlock.FontSize = 32 * (this.ClientSize.Width / 1920);
            }
        }
    }
}
