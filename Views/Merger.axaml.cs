using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using MedEye.Consts;
using MedEye.Controls;
using System;

namespace MedEye.Views
{
    public partial class Merger : Window
    {
        // Random generator for target pos.
        static Random rnd = new Random();

        private static readonly DispatcherTimer after_move_reset_timer = new DispatcherTimer();

        private static readonly DispatcherTimer log_timer = new DispatcherTimer();

        public Merger()
        {
            InitializeComponent();

            after_move_reset_timer.Tick += ResetColor;
            after_move_reset_timer.Interval = new TimeSpan(500000);

            log_timer.Tick += CloseLog;
            log_timer.Interval = new TimeSpan(20000000);

            DragControl first = this.Get<DragControl>("PartOne");
            double first_height = first.Height;
            double first_width = first.Width;
            Canvas.SetTop(first, rnd.Next(Convert.ToInt32(first_height), Convert.ToInt32(this.ClientSize.Height - first_height)));
            Canvas.SetLeft(first, rnd.Next(Convert.ToInt32(first_width), Convert.ToInt32(this.ClientSize.Width - first_width)));

            DragControl second = this.Get<DragControl>("PartTwo");
            double second_height = second.Height;
            double second_width = second.Width;
            Canvas.SetTop(second, rnd.Next(Convert.ToInt32(second_height), Convert.ToInt32(this.ClientSize.Height - second_height)));
            Canvas.SetLeft(second, rnd.Next(Convert.ToInt32(this.ClientSize.Width / 2), Convert.ToInt32(this.ClientSize.Width - second_width)));
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

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (after_move_reset_timer.IsEnabled)
                after_move_reset_timer.Stop();
            after_move_reset_timer.Start();
            base.OnPointerMoved(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            DragControl first = this.Get<DragControl>("PartOne");

            int first_center_x = (int)(Canvas.GetLeft(first) + first.Width / 2);
            int first_center_y = (int)(Canvas.GetTop(first) + first.Height / 2) - 150;

            var currentPosition = e.GetPosition(Parent);
            var offsetX = currentPosition.X;
            var offsetY = currentPosition.Y - 150;

            DragControl second = this.Get<DragControl>("PartTwo");
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

                Canvas.SetTop(first, rnd.Next(Convert.ToInt32(first_height), Convert.ToInt32(this.ClientSize.Height - first_height)));
                Canvas.SetLeft(first, rnd.Next(Convert.ToInt32(first_width), Convert.ToInt32(this.ClientSize.Width - first_width)));

                double second_height = second.Height;
                double second_width = second.Width;
                Canvas.SetTop(second, rnd.Next(Convert.ToInt32(second_height), Convert.ToInt32(this.ClientSize.Height - second_height)));
                Canvas.SetLeft(second, rnd.Next(Convert.ToInt32(this.ClientSize.Width / 2), Convert.ToInt32(this.ClientSize.Width - second_width)));

                Border log = this.Get<Border>("Log");
                Label text = (Label)log.Child;
                text.Content = string.Format("Отклонение:\nПо вертикали: {0}\nПо горизонтали: {1}", first_center_x - second_center_x, first_center_y - second_center_y);
                log.Opacity = 1;
                if (log_timer.IsEnabled)
                    log_timer.Stop();
                log_timer.Start();
                Canvas.SetTop(log, this.ClientSize.Height / 2 - log.Height / 2);
                Canvas.SetLeft(log, this.ClientSize.Width / 2 - log.Width / 2);
            }
            base.OnPointerReleased(e);
        }

        private void ResetColor(object? sender, EventArgs e)
        {
            Border first_1 = this.Get<Border>("BigOne");
            Border first_2 = this.Get<Border>("SmallOne");
            first_1.Background = ColorConst.STRABISMUS_FIRST_COLOR;
            first_2.Background = ColorConst.STRABISMUS_FIRST_COLOR;

            Border second_1 = this.Get<Border>("BigTwo");
            Border second_2 = this.Get<Border>("SmallTwo");
            second_1.Background = ColorConst.STRABISMUS_SECOND_COLOR;
            second_2.Background = ColorConst.STRABISMUS_SECOND_COLOR;

            after_move_reset_timer.Stop();
        }

        private void CloseLog(object? sender, EventArgs e)
        {
            Border log = this.Get<Border>("Log");
            log.Opacity = 0;
            log_timer.Stop();
        }
    }
}
