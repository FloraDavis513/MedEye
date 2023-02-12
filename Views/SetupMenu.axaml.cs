using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using MedEye.DB;
using MedEye.ViewModels;
using ReactiveUI;

namespace MedEye.Views;

public partial class SetupMenu : ReactiveUserControl<SetupMenuViewModel>
{
    private int _currentGame = 0;
    private int _userId = -1;
    private int _gameNumber = -1;
    private List<Settings> _gamesSettings;

    private readonly Thickness _unselectedThickness = new(2.5);
    private readonly SolidColorBrush _unselectedBackground = new(Color.Parse("#CFE0F2"));

    private readonly Thickness _selectedThickness = new(5);
    private readonly SolidColorBrush _selectedBackground = new(Color.Parse("#93B9E2"));

    public SetupMenu()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            _userId = ViewModel!.UserId;
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

                SelectGame((ComboBox)Games.Children[_gameNumber]);

                SetSettings(_gamesSettings[_gameNumber]);
                GameSetupHeader.Text = "Настройки игры №" + (_gameNumber + 1);
            }
        });
        
        MainMenu.Click += async (s, e) =>
        {
            await new ConfirmAction("Вы уверены что хотите выйти в главное меню? Настройки приложения не будут сохранены!",
                (sender, e) => ViewModel!.GoMainMenu.Execute())
                .ShowDialog(ViewModel!.HostWindow);
            SaveSettings();
        };

        StartGame.Click += PreStartGameClick;
        AddGame.Click += AddGameHandle;
        SaveGame.Click += async (s, e) =>
        {
            await new ConfirmAction("Вы уверены, что хотите сохранить изменения?", SaveGameHandle)
                .ShowDialog(ViewModel!.HostWindow);
        };
        DeleteGame.Click += async (s, e) =>
        {
            if (Games.Children.Count == 0) return;
            await new ConfirmAction("Вы уверены, что хотите удалить игру?", DeleteGameHandle).ShowDialog(ViewModel!.HostWindow);
        };

        Games.Children.Remove(DefaultGame);
    }

    private void SaveSettings()
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
        _currentGame++;

        if (_currentGame >= _gamesSettings.Count) return;

        var game = _gamesSettings[_currentGame];
        switch (game.GameId)
        {
            case 1:
                new Tyr(game, PreNextGame).Show();
                break;
            case 2:
                new Following(game, PreNextGame).Show();
                break;
            case 3:
                new Combination(game, PreNextGame).Show();
                break;
            case 4:
                new Merger(game, PreNextGame).Show();
                break;
        }
    }

    private void StartGameClick(object? sender, RoutedEventArgs e)
    {
        _currentGame = 0;

        if (_gamesSettings.Count == 0) return;

        var game = _gamesSettings[_currentGame];
        switch (game.GameId)
        {
            case 1:
                new Tyr(game, PreNextGame).Show();
                break;
            case 2:
                new Following(game, PreNextGame).Show();
                break;
            case 3:
                new Combination(game, PreNextGame).Show();
                break;
            case 4:
                new Merger(game, PreNextGame).Show();
                break;
        }
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

            SelectGame((ComboBox)Games.Children[_gameNumber]);
            GameSetupHeader.Text = "Настройки игры №" + (_gameNumber + 1);
        }
        else
        {
            GameSetupHeader.Text = "Настройки игры";
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
            }
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
            UnselectGame((ComboBox)Games.Children[_gameNumber]);
        }

        _gameNumber = Games.Children.Count;
        AddGameToGrid();
        SelectGame((ComboBox)Games.Children[_gameNumber]);
        ResetSettings();
        GameSetupHeader.Text = "Настройки игры №" + (_gameNumber + 1);
    }

    private void ClickGame(object? sender, RoutedEventArgs e)
    {
        if (sender == null) return;
        var newGame = (ComboBox)sender;
        var oldGame = (ComboBox)Games.Children[_gameNumber];

        if (newGame.Equals(oldGame)) return;

        SelectGame(newGame);
        UnselectGame(oldGame);

        _gameNumber = Games.Children.IndexOf(newGame);
        if (_gameNumber < _gamesSettings.Count)
        {
            SetSettings(_gamesSettings[_gameNumber]);
        }
        else
        {
            ResetSettings();
        }

        GameSetupHeader.Text = "Настройки игры №" + (_gameNumber + 1);
    }

    private void SelectGame(TemplatedControl game)
    {
        game.BorderThickness = _selectedThickness;
        game.Background = _selectedBackground;
    }

    private void UnselectGame(TemplatedControl game)
    {
        game.BorderThickness = _unselectedThickness;
        game.Background = _unselectedBackground;
    }
}