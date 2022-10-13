using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using MedEye.Consts;

namespace MedEye.Controls
{
    public class DragControl : Border
    {
        private bool _isPressed;
        private Point _positionInBlock;

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            _isPressed = true;
            _positionInBlock = e.GetPosition(this);
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            _isPressed = false;
            base.OnPointerReleased(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (!_isPressed)
                return;

            if (Parent == null)
                return;
            
            if(this.Name == "PartOne")
            {
                Border first_1 = Parent.Parent.Get<Border>("BigOne");
                Border first_2 = Parent.Parent.Get<Border>("SmallOne");
                first_1.Background = ColorConst.STRABISMUS_MOVE_COLOR;
                first_2.Background = ColorConst.STRABISMUS_MOVE_COLOR;
            }
            else if (this.Name == "PartTwo")
            {
                Border second_1 = Parent.Parent.Get<Border>("BigTwo");
                Border second_2 = Parent.Parent.Get<Border>("SmallTwo");
                second_1.Background = ColorConst.STRABISMUS_MOVE_COLOR;
                second_2.Background = ColorConst.STRABISMUS_MOVE_COLOR;
            }
            else
                this.Background = ColorConst.STRABISMUS_MOVE_COLOR;

            var currentPosition = e.GetPosition(Parent);

            var offsetX = currentPosition.X - _positionInBlock.X;
            var offsetY = currentPosition.Y - _positionInBlock.Y;

            Canvas.SetLeft(this, offsetX);
            Canvas.SetTop(this, offsetY);

            base.OnPointerMoved(e);
        }
    }
}