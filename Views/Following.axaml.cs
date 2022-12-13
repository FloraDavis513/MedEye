using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using MedEye.Consts;
using MedEye.DB;

namespace MedEye.Views;

public partial class Following : Window
{
    // Random generator for target pos.
    private static readonly Random Rnd = new Random();

    // Moving Target
    private static readonly DispatcherTimer MoveTargetTimer = new DispatcherTimer();
    private static readonly DispatcherTimer DirectRotationTimer = new DispatcherTimer();

    // Blink
    private static readonly DispatcherTimer TargetBlinkTimer = new DispatcherTimer();

    private static readonly DispatcherTimer StalkerBlinkTimer = new DispatcherTimer();
    //private static readonly DispatcherTimer ChangeBlinkTimer = new DispatcherTimer();

    // Close game
    private static readonly DispatcherTimer CloseGameTimer = new DispatcherTimer();
    private bool _isClosing = false;

    // Counter for current color.
    private int _currentColor = 0;


    private int _level = 0;
    private int _score = 0;

    //For moving target.
    private int _direct = 0;
    private double _lenD = 2;
    private double _dx = 0;
    private double _dy = 0;

    // Alias for colors amblyopia array.
    private readonly IBrush[] _colors = ColorConst.AMBLYOPIA_COLORS;

    // Scores
    private long _countScores = 0;
    private Scores _scores = new Scores();

    private readonly EventHandler<EventArgs> _nextGame = (sender, args) => { };

    public Following()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        Canvas.SetTop(Target, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - Target.Height)));
        Canvas.SetLeft(Target, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - Target.Width)));

        Canvas.SetTop(Stalker, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - Target.Height)));
        Canvas.SetLeft(Stalker, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - Target.Width)));

        _direct = Rnd.Next(360);
        _dx = _lenD * Math.Cos(_direct);
        _dy = _lenD * Math.Sin(_direct);

        CloseGameTimer.Tick += CloseGame;
        CloseGameTimer.Interval = new TimeSpan(0, 5, 0);

        SetDefaultScores(0, 2, 1);
        SetDifficultLevel(0);
        StartBlink();
        StartMoving();
        CloseGameTimer.Start();
    }

    public Following(Settings settings, EventHandler<EventArgs> nextGame)
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        Canvas.SetTop(Target, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - Target.Height)));
        Canvas.SetLeft(Target, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - Target.Width)));

        Canvas.SetTop(Stalker, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - Target.Height)));
        Canvas.SetLeft(Stalker, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - Target.Width)));

        _direct = Rnd.Next(360);
        _dx = _lenD * Math.Cos(_direct);
        _dy = _lenD * Math.Sin(_direct);

        CloseGameTimer.Tick += CloseGame;
        CloseGameTimer.Interval = new TimeSpan(0, 0, settings.ExerciseDuration);

        SetDefaultScores(settings.UserId, settings.GameId, settings.Level);
        SetDifficultLevel(settings.Level - 1);

        StartBlink(settings.FlickerMode, settings.Frequency);
        StartMoving();

        _nextGame = nextGame;
        CloseGameTimer.Start();
    }

    private void StartBlink(int mode = 2, int frequency = 10)
    {
        TargetBlinkTimer.Tick += TargetBlink;
        TargetBlinkTimer.Interval = new TimeSpan(0, 0, 0, (int)(1.0 / frequency));

        StalkerBlinkTimer.Tick += StalkerBlink;
        StalkerBlinkTimer.Interval = new TimeSpan(0, 0, 0, (int)(1.0 / frequency));

        switch (mode)
        {
            case 0:
                TargetBlinkTimer.Start();
                break;
            case 1:
                StalkerBlinkTimer.Start();
                break;
            case 2:
                TargetBlinkTimer.Start();
                StalkerBlinkTimer.Start();
                break;
            case 4:
                TargetBlinkTimer.Stop();
                StalkerBlinkTimer.Stop();
                break;
        }
    }

    private void StartMoving()
    {
        MoveTargetTimer.Tick += MoveTarget;
        MoveTargetTimer.Interval = new TimeSpan(10000);

        DirectRotationTimer.Tick += DirectRotation;
        DirectRotationTimer.Interval = new TimeSpan(0, 0, 0, 1);

        MoveTargetTimer.Start();
        DirectRotationTimer.Start();
    }

    private void StopTimer()
    {
        MoveTargetTimer.Stop();
        DirectRotationTimer.Stop();
        TargetBlinkTimer.Stop();
        StalkerBlinkTimer.Stop();
        CloseGameTimer.Stop();
    }

    private void MoveTarget(object? sender, EventArgs e)
    {
        var targetX = Canvas.GetLeft(Target);
        var targetY = Canvas.GetTop(Target);

        if (targetX + _dx <= 0 || targetX + _dx >= ClientSize.Width - Target.Width)
        {
            _dx = -_dx;
            _direct += 180;
        }

        if (targetY + _dy <= 0 || targetY + _dy >= ClientSize.Height - Target.Height)
        {
            _dy = -_dy;
            _direct += 180;
        }

        Canvas.SetLeft(Target, targetX + _dx);
        Canvas.SetTop(Target, targetY + _dy);
    }

    private void DirectRotation(object? sender, EventArgs e)
    {
        _direct = _direct - 15 + (Rnd.NextDouble() > 0.5 ? 0 : 30);
        _direct %= 360;
        _dx = _lenD * Math.Cos(_direct * Math.PI / 180);
        _dy = _lenD * Math.Sin(_direct * Math.PI / 180);
        CheckTargetInStalker();
    }

    private void TargetBlink(object? sender, EventArgs e)
    {
        Target.Background = Equals(Target.Background, _colors[_currentColor])
            ? ColorConst.AMBLYOPIA_MOVE_COLOR
            : _colors[_currentColor];
    }

    private void StalkerBlink(object? sender, EventArgs e)
    {
        Stalker.BorderBrush = Equals(Stalker.BorderBrush, _colors[_currentColor])
            ? ColorConst.AMBLYOPIA_MOVE_COLOR
            : _colors[_currentColor];
    }

    private void CloseGame(object? sender, EventArgs e)
    {
        _isClosing = true;

        StopTimer();

        var trackerResult = Tracker.Tracker.GetResult();
        _scores.Involvement = Math.Round(double.Parse(trackerResult.Replace(",", "."),
            CultureInfo.InvariantCulture) / CloseGameTimer.Interval.TotalSeconds, 2);

        _scores.DateCompletion = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        ScoresWrap.AddScores(_scores);

        ShowResult();

        CloseGameTimer.Tick -= CloseGame;
        CloseGameTimer.Tick += CloseGameAfterShowResult;
        CloseGameTimer.Interval = new TimeSpan(0, 0, NumberConst.ResultsDisplayTime);
        CloseGameTimer.Start();
    }

    private void ShowResult()
    {
        Result.Content = "Результат игры:\n" + _scores;
        Result.FontSize = 32 * (ClientSize.Width / 1920);
        Result.Height = ClientSize.Height / 2.5 - 25;
        Result.Width = ClientSize.Width / 2 - 25;
        Log.Height = ClientSize.Height / 2.5;
        Log.Width = ClientSize.Width / 2;
        Log.CornerRadius = new CornerRadius(15);
        Log.Opacity = 1;
        Canvas.SetTop(Log, ClientSize.Height / 2 - Log.Height / 2);
        Canvas.SetLeft(Log, ClientSize.Width / 2 - Log.Width / 2);
    }

    private void CloseGameAfterShowResult(object? sender, EventArgs e)
    {
        CloseGameTimer.Stop();
        _nextGame(sender, e);
        Close();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && !_isClosing)
        {
            _isClosing = true;
            StopTimer();
            CloseGame(this, e);
        }

        base.OnKeyDown(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        this.Background = Avalonia.Media.Brushes.Black;
        base.OnPointerReleased(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        var currentPosition = e.GetPosition(Parent);
        var offsetX = currentPosition.X;
        var offsetY = currentPosition.Y;

        if (offsetY - (Stalker.Height / 2) >= 0 && offsetY + (Stalker.Height / 2) <= ClientSize.Height)
        {
            Canvas.SetTop(Stalker, offsetY - (Stalker.Height / 2));
        }

        if (offsetX - (Stalker.Width / 2) >= 0 && offsetX + (Stalker.Width / 2) <= ClientSize.Width)
        {
            Canvas.SetLeft(Stalker, offsetX - (Stalker.Width / 2));
        }

        CalculateScore();

        base.OnPointerMoved(e);
    }

    private void CheckTargetInStalker()
    {
        var targetCenterX = Canvas.GetLeft(Target) + Target.Width / 2;
        var targetCenterY = Canvas.GetTop(Target) + Target.Height / 2;

        if (targetCenterX >= Canvas.GetLeft(Stalker)
            && targetCenterX <= Canvas.GetLeft(Stalker) + Stalker.Width
            && targetCenterY >= Canvas.GetTop(Stalker)
            && targetCenterY <= Canvas.GetTop(Stalker) + Stalker.Height)
        {
            _score += _score < 10 ? 1 : -10;
            _scores.Score += 1;
            _scores.Score %= 100;
        }
        else
        {
            _score -= _score > 0 ? 1 : 0;
            _scores.Score -= 1;
            _scores.Score %= 100;
        }

        if (_score == 10 && _level < DifficultConst.TYR_SIZES.Length - 1)
        {
            SetDifficultLevel(_level + 1);
            _currentColor = (_currentColor + 1) % ColorConst.AMBLYOPIA_COLORS.Length;
        }
    }

    private void SetDefaultScores(int userId, int gameId, int level)
    {
        _scores.UserId = userId;
        _scores.GameId = gameId;
        _scores.Level = level;
        _scores.MaxDeviationsX = 0;
        _scores.MaxDeviationsY = 0;
        _scores.MinDeviationsX = double.MaxValue;
        _scores.MinDeviationsY = double.MaxValue;
    }

    private void CalculateScore()
    {
        var targetCenterX = Canvas.GetLeft(Target) + Target.Width / 2;
        var targetCenterY = Canvas.GetTop(Target) + Target.Height / 2;

        var stalkerCenterX = Canvas.GetLeft(Stalker) + Stalker.Width / 2;
        var stalkerCenterY = Canvas.GetTop(Stalker) + Stalker.Height / 2;

        var offsetX = targetCenterX - stalkerCenterX;
        var offsetY = targetCenterY - stalkerCenterY;

        if (Math.Abs(offsetX) <= Math.Abs(_scores.MinDeviationsX))
        {
            _scores.MinDeviationsX = offsetX;
        }

        if (Math.Abs(offsetY) <= Math.Abs(_scores.MinDeviationsY))
        {
            _scores.MinDeviationsY = offsetY;
        }

        if (Math.Abs(offsetX) >= Math.Abs(_scores.MaxDeviationsX))
        {
            _scores.MaxDeviationsX = offsetX;
        }

        if (Math.Abs(offsetY) >= Math.Abs(_scores.MaxDeviationsY))
        {
            _scores.MaxDeviationsY = offsetY;
        }

        _scores.MeanDeviationsX = (_scores.MeanDeviationsX * _countScores + offsetX) / (_countScores + 1);
        _scores.MeanDeviationsY = (_scores.MeanDeviationsY * _countScores + offsetY) / (_countScores + 1);
        _countScores++;
    }

    public void SetDifficultLevel(int level)
    {
        _level = level;
        Stalker.Height = DifficultConst.TYR_SIZES[level];
        Stalker.Width = DifficultConst.TYR_SIZES[level];
        Target.Height = DifficultConst.TYR_SIZES[level] / 2.0;
        Target.Width = DifficultConst.TYR_SIZES[level] / 2.0;
        Target.CornerRadius = CornerRadius.Parse((DifficultConst.TYR_SIZES[level] / 2).ToString());
    }
}