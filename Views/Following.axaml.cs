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
        
        Canvas.SetTop(Angle, 10);
        Canvas.SetLeft(Angle, 10);
        
        Canvas.SetTop(DX, 30);
        Canvas.SetLeft(DX, 10);
        
        Canvas.SetTop(DY, 50);
        Canvas.SetLeft(DY, 10);
        
        _direct = Rnd.Next(90);
        _dx = _lenD * Math.Cos(_direct);
        _dy = _lenD * Math.Sin(_direct);
        StartTimer();
    }

    private void StartTimer()
    {
        MoveTargetTimer.Tick += MoveTarget;
        MoveTargetTimer.Interval = new TimeSpan(20000);
        
        DirectRotationTimer.Tick += DirectRotation;
        DirectRotationTimer.Interval = new TimeSpan(0, 0, 0, 1);
        
        TargetBlinkTimer.Tick += TargetBlink;
        TargetBlinkTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
        
        StalkerBlinkTimer.Tick += StalkerBlink;
        StalkerBlinkTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
        
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

        if (targetX + _dx <= 0 || targetX + _dx >= this.ClientSize.Width - Target.Width)
        {
            _dx = -_dx;
            _direct += 90;
        }
 
        if (targetY + _dy <= 0 || targetY + _dy  >= ClientSize.Height - Target.Height)
        {
            _dy = -_dy;
            _direct += 90;
        }
        
        Canvas.SetLeft(Target, targetX + _dx);
        Canvas.SetTop(Target, targetY + _dy);
        Angle.Text = _direct.ToString();
        DX.Text = _dx.ToString();
        DY.Text = _dy.ToString();
    }

    private void DirectRotation(object? sender, EventArgs e)
    {
        _direct = _direct - 15 + (Rnd.NextDouble() > 0.5 ? 0 : 30);
        _direct %= 360;
        _dx = _lenD * Math.Cos(_direct * Math.PI / 180);
        _dy = _lenD * Math.Sin(_direct * Math.PI / 180);
        Angle.Text = _direct.ToString();
        DX.Text = _dx.ToString();
        DY.Text = _dy.ToString();
    }
    
    

    private void TargetBlink(object? sender, EventArgs e)
    {
        Target.Background = Equals(Target.Background, _colors[_currentColor]) 
            ? ColorConst.AMBLYOPIA_MOVE_COLOR 
            : _colors[_currentColor];
    }
    private void StalkerBlink(object? sender, EventArgs e) {
        Stalker.BorderBrush = Equals(Stalker.BorderBrush, _colors[_currentColor]) 
            ? ColorConst.AMBLYOPIA_MOVE_COLOR 
            : _colors[_currentColor];
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            StopTimer();
            this.Close();
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

        Canvas.SetTop(Stalker, offsetY - (Stalker.Height / 2));
        Canvas.SetLeft(Stalker, offsetX - (Stalker.Width / 2));
        base.OnPointerMoved(e);
    }
}