using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MedEye.DB;
using System.Collections.ObjectModel;
using DynamicData;
using MedEye.ExcelLoader;

namespace MedEye.Views
{
    public partial class StatTable : Window
    {
        private static readonly DispatcherTimer close_timer = new DispatcherTimer();
        private int currentId;

        public StatTable()
        {
            InitializeComponent();

            ToRegistry.Click += RegistryClick;
            Load.Click += ResultsClick;

            GetTyr.Click += TyrStatClick;
            GetFollowing.Click += FollowingStatClick;
            GetCombination.Click += CombinationStatClick;
            GetMerger.Click += MergerStatClick;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);
        }

        public StatTable(int gamer_id)
        {
            InitializeComponent();

            currentId = gamer_id;

            ToRegistry.Click += RegistryClick;
            Load.Click += ResultsClick;

            GetTyr.Click += TyrStatClick;
            GetFollowing.Click += FollowingStatClick;
            GetCombination.Click += CombinationStatClick;
            GetMerger.Click += MergerStatClick;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);

            var current_scores = ScoresWrap.GetScores(gamer_id);
            if (current_scores.Count == 0)
                return;
            var dataSource = new ObservableCollection<string[]>();
            foreach (var score in current_scores)
            {
                dataSource.Add(new[]{ score.DateCompletion.ToString(), score.MeanDeviationsX.ToString(),
                                       score.MeanDeviationsY.ToString(), score.MinDeviationsX.ToString(),
                                       score.MinDeviationsY.ToString(), score.MaxDeviationsX.ToString(),
                                       score.MaxDeviationsY.ToString(), score.Level.ToString(), score.Score.ToString() + "0"});
            }
            string[] headers = { "Дата", "Среднее\nотклонение\nпо X", "Среднее\nотклонение\nпо Y",
                                 "Мин.\nотклонение\nпо X", "Мин.\nотклонение\nпо Y",
                                 "Макс.\nотклонение\nпо X", "Макс.\nотклонение\nпо Y",
                                 "Уровень\nсложности", "Очки" };

            foreach (var idx in dataSource[0].Select((value, index) => index))
            {
                Results.Columns.Add(new DataGridTextColumn { Header = $"{headers[idx]}", Binding = new Binding($"[{idx}]") });
            }
            Results.Items = dataSource;
        }

        public StatTable(string game_name)
        {
            InitializeComponent();

            Header.Text = $"Результат игрока № xx по игре \"{game_name}\"";

            ToRegistry.Click += RegistryClick;
            Load.Click += ResultsClick;

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

            var reg = this.Get<Button>("ToRegistry");
            var cab = this.Get<Button>("Load");
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

        private void RegistryClick(object? sender, RoutedEventArgs e)
        {
            new Registry().Show();
            close_timer.Start();
        }

        private void ResultsClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                ExcelGenerator.GenerateExcelByUserId(1);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
            }
            
        }

        private void TyrStatClick(object? sender, RoutedEventArgs e)
        {
            Header.Text = $"Результат игрока № xx по игре \"Тир\"";
            UpdateTable(0);
        }

        private void FollowingStatClick(object? sender, RoutedEventArgs e)
        {
            Header.Text = $"Результат игрока № xx по игре \"Погоня\"";
            UpdateTable(1);
        }

        private void CombinationStatClick(object? sender, RoutedEventArgs e)
        {
            Header.Text = $"Результат игрока № xx по игре \"Совмещение\"";
            UpdateTable(2);
        }

        private void MergerStatClick(object? sender, RoutedEventArgs e)
        {
            Header.Text = $"Результат игрока № xx по игре \"Слияние\"";
            UpdateTable(3);
        }

        private void AdaptToScreen()
        {
            var buttonWidth = 2 * this.ClientSize.Width / 9;

            ToRegistry.Width = buttonWidth;
            Load.Width = buttonWidth;
            GetTyr.Width = buttonWidth / 1.25;
            GetFollowing.Width = buttonWidth / 1.25;
            GetCombination.Width = buttonWidth / 1.25;
            GetMerger.Width = buttonWidth / 1.25;

            ToRegistry.FontSize = 32 * (this.ClientSize.Width / 1920);
            Load.FontSize = 32 * (this.ClientSize.Width / 1920);
            GetTyr.FontSize = 32 * (this.ClientSize.Width / 1920);
            GetFollowing.FontSize = 32 * (this.ClientSize.Width / 1920);
            GetCombination.FontSize = 32 * (this.ClientSize.Width / 1920);
            GetMerger.FontSize = 32 * (this.ClientSize.Width / 1920);
            Header.FontSize = 32 * (this.ClientSize.Width / 1920);
        }

        private void UpdateTable(int game_id)
        {
            Results.Columns.Clear();
            var current_scores = ScoresWrap.GetScores(currentId);
            if (current_scores.Count == 0)
                return;
            var dataSource = new ObservableCollection<string[]>();
            foreach (var score in current_scores)
            {
                dataSource.Add(new[]{ score.DateCompletion.ToString(), score.MeanDeviationsX.ToString(),
                                       score.MeanDeviationsY.ToString(), score.MinDeviationsX.ToString(),
                                       score.MinDeviationsY.ToString(), score.MaxDeviationsX.ToString(),
                                       score.MaxDeviationsY.ToString(), score.Level.ToString(), score.Score.ToString() + $"{game_id}"});
            }
            string[] headers = { "Дата", "Среднее\nотклонение\nпо X", "Среднее\nотклонение\nпо Y",
                                 "Мин.\nотклонение\nпо X", "Мин.\nотклонение\nпо Y",
                                 "Макс.\nотклонение\nпо X", "Макс.\nотклонение\nпо Y",
                                 "Уровень\nсложности", "Очки" };

            foreach (var idx in dataSource[0].Select((value, index) => index))
            {
                Results.Columns.Add(new DataGridTextColumn { Header = $"{headers[idx]}", Binding = new Binding($"[{idx}]") });
            }
            Results.Items = dataSource;
        }
    }
}
