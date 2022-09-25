using Avalonia.Media;

namespace MedEye.Consts
{
    static class ColorConst
    {
        public static readonly IBrush[] AMBLYOPIA_COLORS = { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.Purple };
        public static readonly IBrush AMBLYOPIA_MOVE_COLOR = Brushes.Aqua;
        public static readonly IBrush STRABISMUS_FIRST_COLOR = Brushes.Red;
        public static readonly IBrush STRABISMUS_SECOND_COLOR = Brushes.Blue;
        public static readonly IBrush STRABISMUS_MOVE_COLOR = Brushes.Magenta;
    }
}