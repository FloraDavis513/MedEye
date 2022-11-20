using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using MedEye.Consts;
using MedEye.Controls;
using System;
using MedEye.DB;

namespace MedEye.Views
{
    public partial class Combination : Window
    {
        // Random generator for target pos.
        static Random rnd = new Random();

        private static readonly DispatcherTimer log_timer = new DispatcherTimer();

        private static readonly DispatcherTimer after_move_reset_timer = new DispatcherTimer();

        // Blink
        private static readonly DispatcherTimer FirstBlinkTimer = new DispatcherTimer();
        private static readonly DispatcherTimer SecondBlinkTimer = new DispatcherTimer();

        // Close game
        private static readonly DispatcherTimer CloseGameTimer = new DispatcherTimer();

        public Combination()
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

            log_timer.Tick += CloseLog;
            log_timer.Interval = new TimeSpan(20000000);

            after_move_reset_timer.Tick += ResetColor;
            after_move_reset_timer.Interval = new TimeSpan(500000);

            DragControl first = this.Get<DragControl>("FirstObject");
            double first_height = first.Height;
            double first_width = first.Width;
            Canvas.SetTop(first, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - first_height)));
            Canvas.SetLeft(first, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - first_width)));

            DragControl second = this.Get<DragControl>("SecondObject");
            double second_height = second.Height;
            double second_width = second.Width;
            Canvas.SetTop(second, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - second_height)));
            Canvas.SetLeft(second, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - second_width)));

            CloseGameTimer.Tick += CloseGame;
            CloseGameTimer.Interval = new TimeSpan(0, 5, 0);
            StartBlink(4);
            CloseGameTimer.Start();
        }

        public Combination(Settings settings)
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

            log_timer.Tick += CloseLog;
            log_timer.Interval = new TimeSpan(20000000);

            after_move_reset_timer.Tick += ResetColor;
            after_move_reset_timer.Interval = new TimeSpan(500000);

            Canvas.SetTop(FirstObject, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - FirstObject.Height)));
            Canvas.SetLeft(FirstObject, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - FirstObject.Width)));

            Canvas.SetTop(SecondObject, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - SecondObject.Height)));
            Canvas.SetLeft(SecondObject, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - SecondObject.Width)));

            CloseGameTimer.Tick += CloseGame;
            CloseGameTimer.Interval = new TimeSpan(0, settings.ExerciseDuration, 0);
            StartBlink(settings.FlickerMode, settings.Frequency);
            CloseGameTimer.Start();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (after_move_reset_timer.IsEnabled)
                    after_move_reset_timer.Stop();
                if (log_timer.IsEnabled)
                    log_timer.Stop();
                this.Close();
            }

            base.OnKeyDown(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            DragControl first = this.Get<DragControl>("FirstObject");

            int first_center_x = (int)(Canvas.GetLeft(first) + first.Width / 2);
            int first_center_y = (int)(Canvas.GetTop(first) + first.Height / 2);

            var currentPosition = e.GetPosition(Parent);
            var offsetX = currentPosition.X;
            var offsetY = currentPosition.Y;

            DragControl second = this.Get<DragControl>("SecondObject");
            double second_left = Canvas.GetLeft(second);
            double second_top = Canvas.GetTop(second);
            double second_right = second_left + second.Width;
            double second_bottom = second_top + second.Height;
            int second_center_x = (int)(Canvas.GetLeft(second) + second.Width / 2);
            int second_center_y = (int)(Canvas.GetTop(second) + second.Height / 2);

            if (offsetX > second_left && offsetX < second_right &&
                offsetY > second_top && offsetY < second_bottom)
            {
                double first_height = first.Height;
                double first_width = first.Width;

                Canvas.SetTop(first,
                    rnd.Next(Convert.ToInt32(first_height), Convert.ToInt32(this.ClientSize.Height - first_height)));
                Canvas.SetLeft(first,
                    rnd.Next(Convert.ToInt32(first_width), Convert.ToInt32(this.ClientSize.Width - first_width)));

                double second_height = second.Height;
                double second_width = second.Width;
                Canvas.SetTop(second,
                    rnd.Next(Convert.ToInt32(second_height), Convert.ToInt32(this.ClientSize.Height - second_height)));
                Canvas.SetLeft(second,
                    rnd.Next(Convert.ToInt32(this.ClientSize.Width / 2),
                        Convert.ToInt32(this.ClientSize.Width - second_width)));

                Border log = this.Get<Border>("Log");
                Label text = (Label)log.Child;
                text.Content = string.Format("Отклонение:\nПо вертикали: {0}\nПо горизонтали: {1}",
                    first_center_x - second_center_x, first_center_y - second_center_y);
                log.Opacity = 1;
                if (log_timer.IsEnabled)
                    log_timer.Stop();
                log_timer.Start();
                Canvas.SetTop(log, this.ClientSize.Height / 2 - log.Height / 2);
                Canvas.SetLeft(log, this.ClientSize.Width / 2 - log.Width / 2);
            }
            else
                first.Background = ColorConst.STRABISMUS_FIRST_COLOR;

            base.OnPointerReleased(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            DragControl first = this.Get<DragControl>("FirstObject");
            first.Background = ColorConst.STRABISMUS_FIRST_COLOR;

            base.OnPointerPressed(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (after_move_reset_timer.IsEnabled)
                after_move_reset_timer.Stop();
            after_move_reset_timer.Start();
            base.OnPointerMoved(e);
        }

        private void CloseLog(object? sender, EventArgs e)
        {
            Border log = this.Get<Border>("Log");
            log.Opacity = 0;
            log_timer.Stop();
        }

        private void ResetColor(object? sender, EventArgs e)
        {
            DragControl first = this.Get<DragControl>("FirstObject");
            DragControl second = this.Get<DragControl>("SecondObject");
            first.Background = ColorConst.STRABISMUS_FIRST_COLOR;
            second.Background = ColorConst.STRABISMUS_SECOND_COLOR;
            after_move_reset_timer.Stop();
        }

        private void StartBlink(int mode = 2, int frequency = 10)
        {
            FirstBlinkTimer.Tick += FirstBlink;
            FirstBlinkTimer.Interval = new TimeSpan(0, 0, 0, (int)(1.0 / frequency));

            SecondBlinkTimer.Tick += SecondBlink;
            SecondBlinkTimer.Interval = new TimeSpan(0, 0, 0, (int)(1.0 / frequency));

            switch (mode)
            {
                case 0:
                    FirstBlinkTimer.Start();
                    break;
                case 1:
                    SecondBlinkTimer.Start();
                    break;
                case 2:
                    FirstBlinkTimer.Start();
                    SecondBlinkTimer.Start();
                    break;
                case 4:
                    FirstBlinkTimer.Stop();
                    SecondBlinkTimer.Stop();
                    break;
            }
        }

        private void FirstBlink(object? sender, EventArgs e)
        {
            FirstObject.Background = Equals(FirstObject.Background, ColorConst.STRABISMUS_FIRST_COLOR)
                ? ColorConst.STRABISMUS_MOVE_COLOR
                : ColorConst.STRABISMUS_FIRST_COLOR;
        }

        private void SecondBlink(object? sender, EventArgs e)
        {
            FirstObject.Background = Equals(FirstObject.Background, ColorConst.STRABISMUS_SECOND_COLOR)
                ? ColorConst.STRABISMUS_MOVE_COLOR
                : ColorConst.STRABISMUS_SECOND_COLOR;
        }

        private void CloseGame(object? sender, EventArgs e)
        {
            FirstBlinkTimer.Stop();
            SecondBlinkTimer.Stop();
            CloseGameTimer.Stop();
            Close();
        }
    }
}