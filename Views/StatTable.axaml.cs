using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using MedEye.DB;
using System.Collections.ObjectModel;
using Avalonia.ReactiveUI;
using MedEye.ExcelLoader;
using MedEye.ViewModels;
using ReactiveUI;

namespace MedEye.Views;

public partial class StatTable : ReactiveUserControl<StatTableViewModel>
{
    public StatTable()
    {
        InitializeComponent();
        Load.Click += ResultsClick;

        GetTyr.Click += TyrStatClick;
        GetFollowing.Click += FollowingStatClick;
        GetCombination.Click += CombinationStatClick;
        GetMerger.Click += MergerStatClick;

        GetTyr.IsEnabled = false;
        GetFollowing.IsEnabled = true;
        GetCombination.IsEnabled = true;
        GetMerger.IsEnabled = true;
        this.WhenActivated(d =>
        {
            Header.Text = $"Результат игрока № {ViewModel!.CurrentGamer.id} по игре \"Тир\"";
            var currentScores = ScoresWrap.GetScores(ViewModel!.CurrentGamer.id, 1);
            var dataSource = new ObservableCollection<string[]>();
            foreach (var score in currentScores)
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
        });
    }

    private void ResultsClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            ExcelGenerator.GenerateExcelByUserId(ViewModel!.CurrentGamer.id);
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            Console.WriteLine(ex.Message);
        }
    }

    private void TyrStatClick(object? sender, RoutedEventArgs e)
    {
        Header.Text = $"Результат игрока № {ViewModel!.CurrentGamer.id} по игре \"Тир\"";
        GetTyr.IsEnabled = false;
        GetFollowing.IsEnabled = true;
        GetCombination.IsEnabled = true;
        GetMerger.IsEnabled = true;
        UpdateTable(1);
    }

    private void FollowingStatClick(object? sender, RoutedEventArgs e)
    {
        Header.Text = $"Результат игрока № {ViewModel!.CurrentGamer.id} по игре \"Погоня\"";
        GetTyr.IsEnabled = true;
        GetFollowing.IsEnabled = false;
        GetCombination.IsEnabled = true;
        GetMerger.IsEnabled = true;
        UpdateTable(2);
    }

    private void CombinationStatClick(object? sender, RoutedEventArgs e)
    {
        Header.Text = $"Результат игрока № {ViewModel!.CurrentGamer.id} по игре \"Совмещение\"";
        GetTyr.IsEnabled = true;
        GetFollowing.IsEnabled = true;
        GetCombination.IsEnabled = false;
        GetMerger.IsEnabled = true;
        UpdateTable(3);
    }

    private void MergerStatClick(object? sender, RoutedEventArgs e)
    {
        Header.Text = $"Результат игрока № {ViewModel!.CurrentGamer.id} по игре \"Слияние\"";
        GetTyr.IsEnabled = true;
        GetFollowing.IsEnabled = true;
        GetCombination.IsEnabled = true;
        GetMerger.IsEnabled = false;
        UpdateTable(4);
    }
    private void UpdateTable(int game_id)
    {
        Results.Columns.Clear();
        var current_scores = ScoresWrap.GetScores(ViewModel!.CurrentGamer.id, game_id);
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