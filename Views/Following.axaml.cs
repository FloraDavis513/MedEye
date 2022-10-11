using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using MedEye.Consts;
using SkiaSharp;

namespace MedEye.Views;

public partial class Following : Window
{
    // Random generator for target pos.
    private static readonly Random Rnd = new Random();

    private static readonly DispatcherTimer MoveTargetTimer = new DispatcherTimer();
    private static readonly DispatcherTimer DirectRotationTimer = new DispatcherTimer();
    private static readonly DispatcherTimer TargetBlinkTimer = new DispatcherTimer();
    private static readonly DispatcherTimer StalkerBlinkTimer = new DispatcherTimer();

    // Counter for current color.
    private int _currentColor = 0;

    private int _score = 0;

    //For moving target.
    private int _direct = 0;
    private double _lenD = 2;
    private double _dx = 0;
    private double _dy = 0;

    // Alias for colors amblyopia array.
    private readonly IBrush[] _colors = ColorConst.AMBLYOPIA_COLORS;

    public Following()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        Canvas.SetTop(Target, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - Target.Height)));
        Canvas.SetLeft(Target, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - Target.Width)));

        _direct = Rnd.Next(360);
        _dx = _lenD * Math.Cos(_direct);
        _dy = _lenD * Math.Sin(_direct);
        StartTimer();
    }

    private void StartTimer()
    {
        MoveTargetTimer.Tick += MoveTarget;
        MoveTargetTimer.Interval = new TimeSpan(10000);

        DirectRotationTimer.Tick += DirectRotation;
        DirectRotationTimer.Interval = new TimeSpan(0, 0, 0, 1);

        TargetBlinkTimer.Tick += TargetBlink;
        TargetBlinkTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

        StalkerBlinkTimer.Tick += StalkerBlink;
        StalkerBlinkTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

        MoveTargetTimer.Start();
        DirectRotationTimer.Start();
        TargetBlinkTimer.Start();
        StalkerBlinkTimer.Start();
    }

    private void StopTimer()
    {
        MoveTargetTimer.Stop();
        DirectRotationTimer.Stop();
        TargetBlinkTimer.Stop();
        StalkerBlinkTimer.Stop();
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

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            StopTimer();
            Close();
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
        
        

        base.OnPointerMoved(e);
    }

    private void CheckTargetInStalker()
    {
        var targetX = Canvas.GetLeft(Target);
        var targetY = Canvas.GetTop(Target);

        var stalkerX = Canvas.GetLeft(Stalker);
        var stalkerY = Canvas.GetTop(Stalker);

        if ((targetX > stalkerX
             && targetX < stalkerX + Stalker.Width
             && targetY >= stalkerY
             && targetY <= stalkerY + Stalker.Height) ||
            (targetX + Target.Width > stalkerX
             && targetX + Target.Width < stalkerX + Stalker.Width
             && targetY + Target.Height >= stalkerY
             && targetY + Target.Height <= stalkerY + Stalker.Height))
        {
            _score += _score < 10 ? 1 : -10;
        }
        else
        {
            _score -= _score > 0 ? 1 : 0;
        }

        if (_score == 10 && Target.Height > 50 && Stalker.Height > 100)
        {
            Target.Height -= 5;
            Target.Width -= 5;
            Stalker.Height -= 10;
            Stalker.Width -= 10;
            _currentColor = (_currentColor + 1) % ColorConst.AMBLYOPIA_COLORS.Length;
        }
    }
}