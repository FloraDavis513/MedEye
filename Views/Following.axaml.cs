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

    private static readonly DispatcherTimer Tmr = new DispatcherTimer();

    // Counter for current color.
    private int _currentColor = 0;

    private int _dx = 3;
    private int _dy = 3;

    // Alias for colors amblyopia array.
    private readonly IBrush[] _colors = ColorConst.AMBLYOPIA_COLORS;
    public Following()
    {
        InitializeComponent();
        #if DEBUG
                this.AttachDevTools();
        #endif
        Tmr.Tick += MoveTarget;
        Tmr.Interval = new TimeSpan(10000);
        Canvas.SetTop(Target, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - Target.Height)));
        Canvas.SetLeft(Target, Rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - Target.Width)));
        Tmr.Start();
    }

    private void MoveTarget(object? sender, EventArgs e)
    {
        var targetX = Canvas.GetLeft(Target);
        var targetY = Canvas.GetTop(Target);

        if (targetX + _dx <= 0 || targetX + _dx >= this.ClientSize.Width - Target.Width)
        {
            _dx = -_dx;
        }
 
        if (targetY + _dy <= 0 || targetY + _dy  >= ClientSize.Height - Target.Height)
        {
            _dy = -_dy;
        }
        
        Canvas.SetLeft(Target, targetX + _dx);
        Canvas.SetTop(Target, targetY + _dy);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Tmr.Stop();
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
        if (Target.BorderBrush == _colors[_currentColor])
            Target.BorderBrush = ColorConst.AMBLYOPIA_MOVE_COLOR;
        else
            Target.BorderBrush = _colors[_currentColor];
        
        
        // Honestly, crutch. Need a better solution. 
        if (Stalker.Background == _colors[_currentColor])
            Stalker.Background = ColorConst.AMBLYOPIA_MOVE_COLOR;
        else
            Stalker.Background = _colors[_currentColor];
        

        var currentPosition = e.GetPosition(Parent);
        var offsetX = currentPosition.X;
        var offsetY = currentPosition.Y;

        Canvas.SetTop(Stalker, offsetY - (Stalker.Height / 2));
        Canvas.SetLeft(Stalker, offsetX - (Stalker.Width / 2));
        base.OnPointerMoved(e);
    }
}