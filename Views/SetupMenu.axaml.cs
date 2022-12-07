using System.Globalization;
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
        StartGame.Click += PreStartGameClick;
        AddGame.Click += AddGameHandle;
        SaveGame.Click += SaveGameHandle;
        DeleteGame.Click += DeleteGameHandle;

        GamesScroll.Height = ClientSize.Height / 1.2;
        GamesScroll.MaxHeight = ClientSize.Height / 1.2;


        CloseTimer.Tick += CloseAfterRoute;
        CloseTimer.Interval = new TimeSpan(1000000);

        foreach (var settings in SettingsWrap.GetSettings(0))
        {
            SettingsWrap.DeleteSettings(settings);
        }
    }

    public SetupMenu(int user_id)
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        MainMenu.Click += MainMenuClick;
        StartGame.Click += PreStartGameClick;
        AddGame.Click += AddGameHandle;
        SaveGame.Click += SaveGameHandle;
        DeleteGame.Click += DeleteGameHandle;

        GamesScroll.Height = ClientSize.Height / 1.2;
        GamesScroll.MaxHeight = ClientSize.Height / 1.2;


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
        var exerciseDuration = (int)(double.Parse(TimerTextBox.Text.Replace(",", ".") ?? "5",
            CultureInfo.InvariantCulture) * 60);

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

        NextGameTimer.Interval = new TimeSpan(0, 0, game.ExerciseDuration + 5);
        NextGameTimer.Start();
    }

    private void StartGameClick(object? sender, RoutedEventArgs e)
    {
        NextGameTimer.Stop();
        
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

        NextGameTimer.Tick += PreNextGame;
        NextGameTimer.Interval = new TimeSpan(0, 0, game.ExerciseDuration + 5);
        NextGameTimer.Start();
    }

    private void PreStartGameClick(object? sender, RoutedEventArgs e)
    {
        _games = SettingsWrap.GetSettings(0);
        _currentGame = 0;

        if (_games.Count == 0) return;
        new ContentDisplay(_currentGame % 5 + 1, StartGameClick).Show();
    }

    private void PreNextGame(object? sender, EventArgs e)
    {
        _currentGame++;
        if (_currentGame >= _games.Count) return;
        new ContentDisplay(_currentGame % 5 + 1, NextGame).Show();
    }

    private void SaveGameHandle(object? sender, EventArgs e)
    {
        var settings = GetCurrentSettings();

        SettingsWrap.AddSettings(settings);
    }
    
    private void DeleteGameHandle(object? sender, EventArgs e)
    {
        if (Games.Children.Count == 0) return;
        
        var game = Games.Children[^1];

        Games.Children.Remove(game);
    }

    private void AddGameHandle(object? sender, RoutedEventArgs e)
    {
        var game = new ComboBox
        {
            PlaceholderText = Game1.PlaceholderText,
            Items = new ComboBoxItem[]
            {
                new() { Content = "Тир" },
                new() { Content = "Погоня" },
                new() { Content = "Совмещение" },
                new() { Content = "Слияние" }
            },
            Width = 2 * ClientSize.Width / 9,
            FontSize = 32 * (ClientSize.Width / 1920)
        };

        Games.RowDefinitions.Add(new RowDefinition());

        Grid.SetRow(game, Games.Children.Count);
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
        AddGame.Width = buttonWidth;
        SaveGame.Width = buttonWidth;

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
        AddGame.FontSize = 32 * (this.ClientSize.Width / 1920);
        SaveGame.FontSize = 32 * (this.ClientSize.Width / 1920);

        Header1.FontSize = 48 * (this.ClientSize.Width / 1920);
        Header2.FontSize = 48 * (this.ClientSize.Width / 1920);
    }
}