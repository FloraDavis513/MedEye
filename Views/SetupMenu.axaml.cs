using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MedEye.DB;

namespace MedEye.Views;

public partial class SetupMenu : Window
{
    private static readonly DispatcherTimer CloseTimer = new DispatcherTimer();

    private int _currentGame = 0;
    private List<Settings> _games;
    private DispatcherTimer NextGameTimer = new DispatcherTimer();


    public SetupMenu()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        MainMenu.Click += MainMenuClick;
        StartGame.Click += StartGameClick;

        Game1.SelectionChanged += SelectGame;


        CloseTimer.Tick += CloseAfterRoute;
        CloseTimer.Interval = new TimeSpan(1000000);

        foreach (var settings in SettingsWrap.GetSettings(0))
        {
            SettingsWrap.DeleteSettings(settings);
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        AdaptToScreen();

        base.OnOpened(e);
    }

    private void MainMenuClick(object? sender, RoutedEventArgs e)
    {
        new MainMenu().Show();
        CloseTimer.Start();
    }

    private void CloseAfterRoute(object? sender, EventArgs e)
    {
        Close();
        CloseTimer.Stop();
    }

    private Settings GetCurrentSettings()
    {
        var distance = DistanceRadioButtons.Children
            .OfType<RadioButton>()
            .FirstOrDefault(r => r.IsChecked ?? false);

        var isRed = IsRedRadioButtons.Children
            .OfType<RadioButton>()
            .FirstOrDefault(r => r.IsChecked ?? false);

        var gameId = ((ComboBox)Games.Children[^1]).SelectedIndex + 1;
        var priority = Games.Children.Count;
        var distanceValue = distance is null ? 0 : int.Parse((string)distance.Content);
        var isRedValue = isRed is not null && (isRed.IsChecked ?? false);
        var frequency = (FrequencyFlickerBox.SelectedIndex + 1) * 10;
        var flickerMode = TypeFlickerBox.SelectedIndex;
        var redBrightness = (int)BrightRedColorSlider.Value;
        var blueBrightness = (int)BrightBlueColorSlider.Value;
        var level = (int)LevelSlider.Value;
        var exerciseDuration = int.Parse(TimerTextBox.Text ?? "5");

        var settings = new Settings
        {
            UserId = 0,
            GameId = gameId,
            Priority = priority,
            Distance = distanceValue,
            IsRed = isRedValue,
            Frequency = frequency,
            FlickerMode = flickerMode,
            RedBrightness = redBrightness,
            BlueBrightness = blueBrightness,
            Level = level,
            ExerciseDuration = exerciseDuration
        };

        return settings;
    }

    private void ResetSettings()
    {
        foreach (var radioButton in DistanceRadioButtons.Children.OfType<RadioButton>())
        {
            radioButton.IsChecked = false;
        }

        foreach (var radioButton in IsRedRadioButtons.Children.OfType<RadioButton>())
        {
            radioButton.IsChecked = false;
        }

        FrequencyFlickerBox.SelectedIndex = 3;
        TypeFlickerBox.SelectedIndex = 2;
        BrightRedColorSlider.Value = 0;
        BrightBlueColorSlider.Value = 0;
        LevelSlider.Value = 0;
        TimerTextBox.Text = "5";
    }

    private void NextGame(object? sender, EventArgs e)
    {
        NextGameTimer.Stop();

        _currentGame++; 
        
        if (_currentGame >= _games.Count) return;
        
        var game = _games[_currentGame];
        switch (game.GameId)
        {
            case 1:
                new Tyr(game).Show();
                break;
            case 2:
                new Following(game).Show();
                break;
            case 3:
                new Combination(game).Show();
                break;
            case 4:
                new Merger(game).Show();
                break;
        }

        NextGameTimer.Interval = new TimeSpan(0, game.ExerciseDuration, 5);
        NextGameTimer.Start();
    }

    private void StartGameClick(object? sender, RoutedEventArgs e)
    {
        NextGameTimer.Stop();

        _games = SettingsWrap.GetSettings(0);
        _currentGame = 0;
        var game = _games[_currentGame];
        switch (game.GameId)
        {
            case 1:
                new Tyr(game).Show();
                break;
            case 2:
                new Following(game).Show();
                break;
            case 3:
                new Combination(game).Show();
                break;
            case 4:
                new Merger(game).Show();
                break;
        }

        NextGameTimer.Tick += NextGame;
        NextGameTimer.Interval = new TimeSpan(0, game.ExerciseDuration, 5);
        NextGameTimer.Start();
    }

    private void SelectGame(object? sender, SelectionChangedEventArgs e)
    {
        if (e.RemovedItems.Count > 0) return;

        var settings = GetCurrentSettings();

        SettingsWrap.AddSettings(settings);

        var game = new ComboBox
        {
            PlaceholderText = Game1.PlaceholderText,
            Items = new ComboBoxItem[]
            {
                new ComboBoxItem { Content = "Тир" },
                new ComboBoxItem { Content = "Погоня" },
                new ComboBoxItem { Content = "Совмещение" },
                new ComboBoxItem { Content = "Слияние" }
            },
            Width = 2 * this.ClientSize.Width / 9,
            FontSize = 32 * (this.ClientSize.Width / 1920)
        };

        game.SelectionChanged += SelectGame;

        Games.RowDefinitions.Add(new RowDefinition());

        Grid.SetRow(MainMenu, Games.Children.Count);

        Grid.SetRow(game, Games.Children.Count - 1);
        Games.Children.Add(game);

        ResetSettings();
    }

    private void AdaptToScreen()
    {
        var buttonWidth = 2 * this.ClientSize.Width / 9;


        Game1.Width = buttonWidth;
        MainMenu.Width = buttonWidth;
        DeleteGame.Width = buttonWidth;
        StartGame.Width = buttonWidth;
        FrequencyFlickerBox.Width = buttonWidth;
        TypeFlickerBox.Width = buttonWidth;

        Game1.FontSize = 32 * (this.ClientSize.Width / 1920);
        MainMenu.FontSize = 32 * (this.ClientSize.Width / 1920);
        DeleteGame.FontSize = 32 * (this.ClientSize.Width / 1920);
        StartGame.FontSize = 32 * (this.ClientSize.Width / 1920);
        Distance.FontSize = 32 * (this.ClientSize.Width / 1920);
        LeftFilterColor.FontSize = 32 * (this.ClientSize.Width / 1920);
        FrequencyFlickerText.FontSize = 32 * (this.ClientSize.Width / 1920);
        FrequencyFlickerBox.FontSize = 32 * (this.ClientSize.Width / 1920);
        TypeFlickerText.FontSize = 32 * (this.ClientSize.Width / 1920);
        TypeFlickerBox.FontSize = 32 * (this.ClientSize.Width / 1920);
        BrightRedColor.FontSize = 32 * (this.ClientSize.Width / 1920);
        BrightBlueColor.FontSize = 32 * (this.ClientSize.Width / 1920);
        Level.FontSize = 32 * (this.ClientSize.Width / 1920);
        Timer.FontSize = 32 * (this.ClientSize.Width / 1920);

        Header1.FontSize = 48 * (this.ClientSize.Width / 1920);
        Header2.FontSize = 48 * (this.ClientSize.Width / 1920);
    }
}