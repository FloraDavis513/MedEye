using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using MedEye.Consts;
using System;

namespace MedEye.Views
{
    public partial class Tyr : Window
    {
        // Random generator for target pos.
        static Random rnd = new Random();

        // Counter for current color.
        private int currentColor = 0;

        // Alias for colors amblyopia array.
        private readonly IBrush[] colors = ColorConst.AMBLYOPIA_COLORS;

        private static readonly DispatcherTimer after_move_reset_timer = new DispatcherTimer();

        private static readonly DispatcherTimer flash_timer = new DispatcherTimer();

        private int flash_count = 0;

        private int level = 0;

        private int success_counter = 0;

        public Tyr()
        {
            InitializeComponent();

            #if DEBUG
                this.AttachDevTools();
            #endif

            after_move_reset_timer.Tick += ResetColor;
            after_move_reset_timer.Interval = new TimeSpan(500000);

            flash_timer.Tick += WinFlash;
            flash_timer.Interval = new TimeSpan(300000);

            // With init generate first pos for target.
            Avalonia.Controls.Shapes.Rectangle target = this.Get<Avalonia.Controls.Shapes.Rectangle>("Target");
            double height = target.Height;
            double width = target.Width;
            Canvas.SetTop(target, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - height)));
            Canvas.SetLeft(target, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - width)));
        }

        public void SetDifficultLevel(int level)
        {
            this.level = level;
            Avalonia.Controls.Shapes.Rectangle target = this.Get<Avalonia.Controls.Shapes.Rectangle>("Target");
            Border scope = this.Get<Border>("Scope");
            
            target.Height = DifficultConst.TYR_SIZES[level];
            target.Width = DifficultConst.TYR_SIZES[level];
            scope.Height = DifficultConst.TYR_SIZES[level];
            scope.Width = DifficultConst.TYR_SIZES[level];
            scope.CornerRadius = CornerRadius.Parse((DifficultConst.TYR_SIZES[level]/2).ToString());
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            Avalonia.Controls.Shapes.Rectangle target = this.Get<Avalonia.Controls.Shapes.Rectangle>("Target");
            double height = target.Height;
            double width = target.Width;
            var currentPosition = e.GetPosition(Parent);

            double targetX = Canvas.GetLeft(target);
            double targetY = Canvas.GetTop(target);

            // When pressed, check the hit on the target.
            if (currentPosition.X >= targetX && currentPosition.X <= targetX + height &&
                currentPosition.Y >= targetY && currentPosition.Y <= targetY + width)
            {
                // And generate new pos for target, change scope color and generate background flash.
                Canvas.SetTop(target, rnd.Next(0, Convert.ToInt32(this.Height - height)));
                Canvas.SetLeft(target, rnd.Next(0, Convert.ToInt32(this.Width - width)));
                ++currentColor;
                if (currentColor >= colors.Length)
                    currentColor = 0;
                if (flash_timer.IsEnabled)
                    flash_timer.Stop();
                flash_timer.Start();
                if(++success_counter >= 5 && level < DifficultConst.TYR_SIZES.Length - 1)
                {
                    success_counter = 0;
                    SetDifficultLevel(++level);
                }
            }

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            this.Background = Avalonia.Media.Brushes.Black;
            base.OnPointerReleased(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (after_move_reset_timer.IsEnabled)
                    after_move_reset_timer.Stop();
                if (flash_timer.IsEnabled)
                    flash_timer.Stop();
                this.Close();
            }
            base.OnKeyDown(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            // For each mouse move correct scope pos.
            Border scope = this.Get<Border>("Scope");
            scope.Background = ColorConst.AMBLYOPIA_MOVE_COLOR;
            scope.Opacity = 0.5;

            if (after_move_reset_timer.IsEnabled)
                after_move_reset_timer.Stop();
            after_move_reset_timer.Start();

            var currentPosition = e.GetPosition(Parent);
            var offsetX = currentPosition.X;
            var offsetY = currentPosition.Y;

            Canvas.SetTop(scope, offsetY - (scope.Height / 2));
            Canvas.SetLeft(scope, offsetX - (scope.Width / 2));
            base.OnPointerMoved(e);
        }

        private void ResetColor(object? sender, EventArgs e)
        {
            Border scope = this.Get<Border>("Scope");
            scope.Background = colors[currentColor];
            scope.Opacity = 1;
            after_move_reset_timer.Stop();
        }

        private void WinFlash(object? sender, EventArgs e)
        {
            ++flash_count;
            if(flash_count == 6)
            {
                this.Background = Avalonia.Media.Brushes.Black;
                flash_count = 0;
                flash_timer.Stop();
                return;
            }
            if (this.Background == Avalonia.Media.Brushes.Black)
                this.Background = Avalonia.Media.Brushes.Red;
            else
                this.Background = Avalonia.Media.Brushes.Black;
        }

    }
}
