using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
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

        public Tyr()
        {
            InitializeComponent();

            #if DEBUG
                this.AttachDevTools();
            #endif

            // With init generate first pos for target.
            Avalonia.Controls.Shapes.Rectangle target = this.Get<Avalonia.Controls.Shapes.Rectangle>("Target");
            double height = target.Height;
            double width = target.Width;
            Canvas.SetTop(target, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - height)));
            Canvas.SetLeft(target, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - width)));
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
                this.Background = Avalonia.Media.Brushes.White;
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
                this.Close();
            base.OnKeyDown(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            // For each mouse move correct scope pos.
            Border scope = this.Get<Border>("Scope");

            // Honestly, crutch. Need a better solution. 
            if (scope.Background == colors[currentColor])
                scope.Background = ColorConst.AMBLYOPIA_MOVE_COLOR;
            else
                scope.Background = colors[currentColor];

            var currentPosition = e.GetPosition(Parent);
            var offsetX = currentPosition.X;
            var offsetY = currentPosition.Y;

            Canvas.SetTop(scope, offsetY - (scope.Height / 2));
            Canvas.SetLeft(scope, offsetX - (scope.Width / 2));
            base.OnPointerMoved(e);
        }

    }
}
