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
            Header.Text = $"Результат игрока № {currentId} по игре \"Тир\"";

            ToRegistry.Click += RegistryClick;
            Load.Click += ResultsClick;

            GetTyr.Click += TyrStatClick;
            GetFollowing.Click += FollowingStatClick;
            GetCombination.Click += CombinationStatClick;
            GetMerger.Click += MergerStatClick;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);
            
            var current_scores = ScoresWrap.GetScores(gamer_id, 1);
            var dataSource = new ObservableCollection<string[]>();
            foreach (var score in current_scores)
            {
                dataSource.Add(new[]{ score.DateCompletion.Substring(0, score.DateCompletion.IndexOf(" ")), Math.Round(score.MeanDeviationsX, 1).ToString(),
                                       Math.Round(score.MeanDeviationsY, 1).ToString(), Math.Round(score.MinDeviationsX, 1).ToString(),
                                       Math.Round(score.MinDeviationsY, 1).ToString(), Math.Round(score.MaxDeviationsX, 1).ToString(),
                                       Math.Round(score.MaxDeviationsY, 1).ToString(), score.Level.ToString(), score.Score.ToString(),
                                       score.Involvement.ToString()});
            }
            // Fake row for empty table.
            if(dataSource.Count == 0)
                dataSource.Add(new[]{ "", "", "", "", "", "", "", "", "", ""});
            string[] headers = { "Дата", "Среднее\nотклонение\nпо X", "Среднее\nотклонение\nпо Y",
                                 "Мин.\nотклонение\nпо X", "Мин.\nотклонение\nпо Y",
                                 "Макс.\nотклонение\nпо X", "Макс.\nотклонение\nпо Y",
                                 "Уровень\nсложности", "Очки", "Вовлечённость" };

            foreach (var idx in dataSource[0].Select((value, index) => index))
            {
                Results.Columns.Add(new DataGridTextColumn { Header = $"{headers[idx]}", Binding = new Binding($"[{idx}]") });
            }
            Results.Items = dataSource;
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
                ExcelGenerator.GenerateExcelByUserId(0);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
            }
            
        }

        private void TyrStatClick(object? sender, RoutedEventArgs e)
        {
            Header.Text = $"Результат игрока № {currentId} по игре \"Тир\"";
            UpdateTable(1);
        }

        private void FollowingStatClick(object? sender, RoutedEventArgs e)
        {
            Header.Text = $"Результат игрока № {currentId} по игре \"Погоня\"";
            UpdateTable(2);
        }

        private void CombinationStatClick(object? sender, RoutedEventArgs e)
        {
            Header.Text = $"Результат игрока № {currentId} по игре \"Совмещение\"";
            UpdateTable(3);
        }

        private void MergerStatClick(object? sender, RoutedEventArgs e)
        {
            Header.Text = $"Результат игрока № {currentId} по игре \"Слияние\"";
            UpdateTable(4);
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
            Results.FontSize = 24 * (this.ClientSize.Width / 1920);
        }

        private void UpdateTable(int game_id)
        {
            Results.Columns.Clear();
            var current_scores = ScoresWrap.GetScores(currentId, game_id);
            var dataSource = new ObservableCollection<string[]>();
            foreach (var score in current_scores)
            {
                dataSource.Add(new[]{ score.DateCompletion.Substring(0, score.DateCompletion.IndexOf(" ")), Math.Round(score.MeanDeviationsX, 1).ToString(),
                                       Math.Round(score.MeanDeviationsY, 1).ToString(), Math.Round(score.MinDeviationsX, 1).ToString(),
                                       Math.Round(score.MinDeviationsY, 1).ToString(), Math.Round(score.MaxDeviationsX, 1).ToString(),
                                       Math.Round(score.MaxDeviationsY, 1).ToString(), score.Level.ToString(), score.Score.ToString(),
                                       score.Involvement.ToString()});
            }
            // Fake row for empty table.
            if (dataSource.Count == 0)
                dataSource.Add(new[] { "", "", "", "", "", "", "", "", "", "" });
            string[] headers = { "Дата", "Среднее\nотклонение\nпо X", "Среднее\nотклонение\nпо Y",
                                 "Мин.\nотклонение\nпо X", "Мин.\nотклонение\nпо Y",
                                 "Макс.\nотклонение\nпо X", "Макс.\nотклонение\nпо Y",
                                 "Уровень\nсложности", "Очки", "Вовлечённость" };

            foreach (var idx in dataSource[0].Select((value, index) => index))
            {
                Results.Columns.Add(new DataGridTextColumn { Header = $"{headers[idx]}", Binding = new Binding($"[{idx}]") });
            }
            Results.Items = dataSource;
        }
    }
}
