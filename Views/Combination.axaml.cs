using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using MedEye.Consts;
using MedEye.Controls;
using System;

namespace MedEye.Views
{
    public partial class Combination : Window
    {
        // Random generator for target pos.
        static Random rnd = new Random();

        public Combination()
        {
            InitializeComponent();

            #if DEBUG
                this.AttachDevTools();
            #endif

            DragControl first = this.Get<DragControl>("FirstObject");
            double first_height = first.Height;
            double first_width = first.Width;
            Canvas.SetTop(first, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - first_height)));
            Canvas.SetLeft(first, rnd.Next(0, Convert.ToInt32(600 - first_width)));

            DragControl second = this.Get<DragControl>("SecondObject");
            double second_height = first.Height;
            double second_width = first.Width;
            Canvas.SetTop(second, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - second_height)));
            Canvas.SetLeft(second, rnd.Next(800, Convert.ToInt32(this.ClientSize.Width - second_width)));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            base.OnKeyDown(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            DragControl first = this.Get<DragControl>("FirstObject");

            var currentPosition = e.GetPosition(Parent);
            var offsetX = currentPosition.X;
            var offsetY = currentPosition.Y;

            DragControl second = this.Get<DragControl>("SecondObject");
            double second_left = Canvas.GetLeft(second);
            double second_top = Canvas.GetTop(second);
            double second_right = second_left + second.Width;
            double second_bottom = second_top + second.Height;

            Label log = this.Get<Label>("Log");

            if (offsetX > second_left && offsetX < second_right &&
                offsetY > second_top && offsetY < second_bottom)
            {
                first.Background = Brushes.Yellow;
                log.Content = "You win!";
                double first_height = first.Height;
                double first_width = first.Width;
                Canvas.SetTop(first, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - first_height)));
                Canvas.SetLeft(first, rnd.Next(0, Convert.ToInt32(600 - first_width)));

                double second_height = first.Height;
                double second_width = first.Width;
                Canvas.SetTop(second, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - second_height)));
                Canvas.SetLeft(second, rnd.Next(800, Convert.ToInt32(this.ClientSize.Width - second_width)));
            }
            else
                first.Background = ColorConst.STRABISMUS_FIRST_COLOR;

            base.OnPointerReleased(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            Label log = this.Get<Label>("Log");
            log.Content = "";

            DragControl first = this.Get<DragControl>("FirstObject");
            first.Background = ColorConst.STRABISMUS_FIRST_COLOR;

            base.OnPointerPressed(e);
        }
    }
}
