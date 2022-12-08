using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using MedEye.DB;

namespace MedEye.Views;

public partial class SetupMenu : Window
{
    private static readonly DispatcherTimer CloseTimer = new DispatcherTimer();

    private int _currentGame = 0;
    private int _userId = -1;
    private int _gameNumber = -1;
    private List<Settings> _gamesSettings;
    private DispatcherTimer NextGameTimer = new DispatcherTimer();


    public SetupMenu()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        MainMenu.Click += (s, e) =>
        {
            new ConfirmAction(ClientSize.Width, ClientSize.Height,
                "Вы уверены что хотите выйти в главное меню? Настройки приложения не будут сохранены!",
                MainMenuClick).Show();
        };
        StartGame.Click += PreStartGameClick;
        AddGame.Click += AddGameHandle;
        SaveGame.Click += SaveGameHandle;
        DeleteGame.Click += (s, e) =>
        {
            if (Games.Children.Count == 0) return;
            new ConfirmAction(this.ClientSize.Width, this.ClientSize.Height,
                "Вы уверены, что хотите удалить игру из списка?", DeleteGameHandle).Show();
        };

        GamesScroll.Height = ClientSize.Height / 1.2;
        GamesScroll.MaxHeight = ClientSize.Height / 1.2;


        CloseTimer.Tick += CloseAfterRoute;
        CloseTimer.Interval = new TimeSpan(1000000);

        _gamesSettings = SettingsWrap.GetSettings(_userId);
        foreach (var settings in _gamesSettings)
        {
            SettingsWrap.DeleteSettings(settings);
            _gamesSettings.Remove(settings);
        }

        ResetSettings();
    }

    public SetupMenu(int userId)
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        _userId = userId;

        MainMenu.Click += (s, e) =>
        {
            new ConfirmAction(this.ClientSize.Width, this.ClientSize.Height,
                "Вы уверены, что хотите перейти в главное меню? Все несохраненные измения будут потеряны!",
                MainMenuClick).Show();
        };
        StartGame.Click += PreStartGameClick;
        AddGame.Click += AddGameHandle;
        SaveGame.Click += SaveGameHandle;
        DeleteGame.Click += (s, e) =>
        {
            if (Games.Children.Count == 0) return;
            new ConfirmAction(this.ClientSize.Width, this.ClientSize.Height,
                "Вы уверены, что хотите удалить игру?", DeleteGameHandle).Show();
        };

        GamesScroll.Height = ClientSize.Height / 1.2;
        GamesScroll.MaxHeight = ClientSize.Height / 1.2;

        Games.Children.Remove(DefaultGame);

        CloseTimer.Tick += CloseAfterRoute;
        CloseTimer.Interval = new TimeSpan(1000000);

        _gamesSettings = SettingsWrap.GetSettings(_userId);

        if (_gamesSettings.Count == 0)
        {
            ResetSettings();
            return;
        }

        if (_userId == -1)
        {
            foreach (var settings in _gamesSettings)
            {
                SettingsWrap.DeleteSettings(settings);
            }

            _gamesSettings.Clear();
            ResetSettings();
        }
        else
        {
            foreach (var settings in _gamesSettings)
            {
                AddGameToGrid();
                _gameNumber++;
                var game = (ComboBox)Games.Children[_gameNumber];
                game.SelectedIndex = settings.GameId - 1;
            }

            ((ComboBox)Games.Children[_gameNumber]).FontWeight = FontWeight.Bold;
            SetSettings(_gamesSettings[_gameNumber]);
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
        foreach (var setting in SettingsWrap.GetSettings(_userId))
        {
            SettingsWrap.DeleteSettings(setting);
        }

        var priority = 1;
        foreach (var setting in _gamesSettings.Where(setting => setting.GameId != 0))
        {
            SettingsWrap.AddSettings(setting.SetPriority(priority++));
        }

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

        var gameId = ((ComboBox)Games.Children[_gameNumber]).SelectedIndex + 1;
        var priority = _gameNumber + 1;
        var distanceValue = distance is null ? 0 : int.Parse((string)distance.Content);
        var isRedValue = isRed is not null && (isRed.IsChecked ?? false);
        var frequency = (FrequencyFlickerBox.SelectedIndex + 1) * 10;
        var flickerMode = TypeFlickerBox.SelectedIndex;
        var redBrightness = (int)BrightRedColorSlider.Value;
        var blueBrightness = (int)BrightBlueColorSlider.Value;
        var level = (int)LevelSlider.Value;
        var exerciseDuration = (int)(double.Parse(TimerTextBox.Text.Replace(",", "."),
            CultureInfo.InvariantCulture) * 60);

        var settings = new Settings
        {
            UserId = _userId,
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

        FrequencyFlickerBox.SelectedIndex = 0;
        TypeFlickerBox.SelectedIndex = 0;
        BrightRedColorSlider.Value = 0;
        BrightBlueColorSlider.Value = 0;
        LevelSlider.Value = 0;
        TimerTextBox.Text = "0";
    }

    private void SetSettings(Settings settings)
    {
        var distance = DistanceRadioButtons.Children
            .OfType<RadioButton>()
            .FirstOrDefault(r => int.Parse((string)r.Content) == settings.Distance);
        if (distance != null) distance.IsChecked = true;

        if (settings.IsRed)
        {
            var isRed = IsRedRadioButtons.Children.OfType<RadioButton>().First();
            isRed.IsChecked = true;
        }
        else
        {
            var isNotRed = IsRedRadioButtons.Children.OfType<RadioButton>().Last();
            isNotRed.IsChecked = true;
        }

        FrequencyFlickerBox.SelectedIndex = settings.Frequency / 10 - 1;
        TypeFlickerBox.SelectedIndex = settings.FlickerMode;
        BrightRedColorSlider.Value = settings.RedBrightness;
        BrightBlueColorSlider.Value = settings.BlueBrightness;
        LevelSlider.Value = settings.Level;
        TimerTextBox.Text = (settings.ExerciseDuration / 60.0).ToString(CultureInfo.InvariantCulture);
    }

    private void NextGame(object? sender, EventArgs e)
    {
        NextGameTimer.Stop();

        _currentGame++;

        if (_currentGame >= _gamesSettings.Count) return;

        var game = _gamesSettings[_currentGame];
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

        _currentGame = 0;

        if (_gamesSettings.Count == 0) return;

        var game = _gamesSettings[_currentGame];
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
        _gamesSettings = _gamesSettings.Where(setting => setting.GameId != 0).ToList();

        _currentGame = 0;

        if (_gamesSettings.Count == 0) return;
        new ContentDisplay(_currentGame % 5 + 1, StartGameClick).Show();
    }

    private void PreNextGame(object? sender, EventArgs e)
    {
        if (_currentGame >= _gamesSettings.Count) return;
        new ContentDisplay((_currentGame + 1) % 5 + 1, NextGame).Show();
    }

    private void SaveGameHandle(object? sender, EventArgs e)
    {
        if (_gameNumber == -1) return;

        var settings = GetCurrentSettings();
        if (_gameNumber < _gamesSettings.Count)
        {
            _gamesSettings[_gameNumber] = settings;
        }
        else
        {
            var count = _gameNumber - _gamesSettings.Count;
            for (var i = 0; i < count; i++)
            {
                _gamesSettings.Add(new Settings());
            }

            _gamesSettings.Add(settings);
        }
    }

    private void DeleteGameHandle(object? sender, EventArgs e)
    {
        if (Games.Children.Count == 0) return;
        var game = Games.Children[_gameNumber];
        if (_gameNumber > -1 && _gameNumber < _gamesSettings.Count)
        {
            _gamesSettings.RemoveAt(_gameNumber);
        }

        _gameNumber -= 1;
        Games.Children.Remove(game);
        if (Games.Children.Count > 0)
        {
            for (var i = 0; i < Games.Children.Count; i++)
            {
                Grid.SetRow((Control)Games.Children[i], i);
            }

            if (_gameNumber == -1)
            {
                _gameNumber = 0;
            }

            if (_gameNumber < _gamesSettings.Count)
            {
                SetSettings(_gamesSettings[_gameNumber]);
            }

            ((ComboBox)Games.Children[_gameNumber]).FontWeight = FontWeight.Bold;
            Header2.Text = "Настройки игры №" + (_gameNumber + 1);
        }
        else
        {
            Header2.Text = "Настройки игры";
            ResetSettings();
        }
    }

    private void AddGameToGrid()
    {
        var game = new ComboBox
        {
            PlaceholderText = DefaultGame.PlaceholderText,
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

        game.Tapped += ClickGame;
        if (Games.RowDefinitions.Count == Games.Children.Count)
        {
            Games.RowDefinitions.Add(new RowDefinition());
        }

        Grid.SetRow(game, Games.Children.Count);
        Games.Children.Add(game);
    }

    private void AddGameHandle(object? sender, RoutedEventArgs e)
    {
        if (_gameNumber > -1)
        {
            ((ComboBox)Games.Children[_gameNumber]).FontWeight = FontWeight.Normal;
        }

        _gameNumber = Games.Children.Count;
        AddGameToGrid();
        ((ComboBox)Games.Children[_gameNumber]).FontWeight = FontWeight.Bold;
        ResetSettings();
        Header2.Text = "Настройки игры №" + (_gameNumber + 1);
    }

    private void ClickGame(object? sender, RoutedEventArgs e)
    {
        if (sender == null) return;
        var newGame = (ComboBox)sender;
        var oldGame = (ComboBox)Games.Children[_gameNumber];

        if (newGame.Equals(oldGame)) return;

        newGame.FontWeight = FontWeight.Bold;
        oldGame.FontWeight = FontWeight.Normal;

        _gameNumber = Games.Children.IndexOf(newGame);
        if (_gameNumber < _gamesSettings.Count)
        {
            SetSettings(_gamesSettings[_gameNumber]);
        }
        else
        {
            ResetSettings();
        }

        Header2.Text = "Настройки игры №" + (_gameNumber + 1);
    }

    private void AdaptToScreen()
    {
        var buttonWidth = 2 * this.ClientSize.Width / 9;
        var fontSize = 32 * (this.ClientSize.Width / 1920);

        foreach (var comboBox in Games.Children.OfType<ComboBox>())
        {
            comboBox.Width = buttonWidth;
            comboBox.FontSize = fontSize;
        }

        foreach (var radioButton in DistanceRadioButtons.Children.OfType<RadioButton>())
        {
            radioButton.FontSize = fontSize;
        }

        foreach (var radioButton in IsRedRadioButtons.Children.OfType<RadioButton>())
        {
            radioButton.FontSize = fontSize;
        }

        DefaultGame.Width = buttonWidth;
        MainMenu.Width = buttonWidth;
        DeleteGame.Width = buttonWidth;
        StartGame.Width = buttonWidth;
        FrequencyFlickerBox.Width = buttonWidth;
        TypeFlickerBox.Width = buttonWidth;
        AddGame.Width = buttonWidth;
        SaveGame.Width = buttonWidth;

        DefaultGame.FontSize = fontSize;
        MainMenu.FontSize = fontSize;
        DeleteGame.FontSize = fontSize;
        StartGame.FontSize = fontSize;
        Distance.FontSize = fontSize;
        LeftFilterColor.FontSize = fontSize;
        FrequencyFlickerText.FontSize = fontSize;
        FrequencyFlickerBox.FontSize = fontSize;
        TypeFlickerText.FontSize = fontSize;
        TypeFlickerBox.FontSize = fontSize;
        BrightRedColor.FontSize = fontSize;
        BrightBlueColor.FontSize = fontSize;
        Level.FontSize = fontSize;
        Timer.FontSize = fontSize;
        TimerTextBox.FontSize = fontSize;
        AddGame.FontSize = fontSize;
        SaveGame.FontSize = fontSize;

        Header1.FontSize = 48 * (this.ClientSize.Width / 1920);
        Header2.FontSize = 48 * (this.ClientSize.Width / 1920);
    }
}