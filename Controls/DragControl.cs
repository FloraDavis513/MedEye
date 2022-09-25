using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using MedEye.Consts;
using System;
using static System.Formats.Asn1.AsnWriter;

namespace MedEye.Controls
{
    public class DragControl : Border
    {
        private bool _isPressed;
        private Point _positionInBlock;
        private TranslateTransform _transform = null!;

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            _isPressed = true;

            if (this.Background == ColorConst.STRABISMUS_MOVE_COLOR && this.Name == "FirstObject")
                this.Background = ColorConst.STRABISMUS_FIRST_COLOR;
            else if (this.Background == ColorConst.STRABISMUS_MOVE_COLOR && this.Name == "SecondObject")
                this.Background = ColorConst.STRABISMUS_SECOND_COLOR;

            _positionInBlock = e.GetPosition(Parent);

            if (_transform != null!)
                _positionInBlock = new Point(
                    _positionInBlock.X - _transform.X,
                    _positionInBlock.Y - _transform.Y);

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            _isPressed = false;

            if (this.Background == ColorConst.STRABISMUS_MOVE_COLOR && this.Name == "FirstObject")
                this.Background = ColorConst.STRABISMUS_FIRST_COLOR;
            else if (this.Background == ColorConst.STRABISMUS_MOVE_COLOR && this.Name == "SecondObject")
                this.Background = ColorConst.STRABISMUS_SECOND_COLOR;

            base.OnPointerReleased(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (!_isPressed)
                return;

            if (this.Background == ColorConst.STRABISMUS_MOVE_COLOR && this.Name == "FirstObject")
                this.Background = ColorConst.STRABISMUS_FIRST_COLOR;
            else if(this.Background == ColorConst.STRABISMUS_MOVE_COLOR && this.Name == "SecondObject")
                this.Background = ColorConst.STRABISMUS_SECOND_COLOR;
            else
                this.Background = ColorConst.STRABISMUS_MOVE_COLOR;

            if (Parent == null)
                return;

            var currentPosition = e.GetPosition(Parent);

            var offsetX = currentPosition.X - _positionInBlock.X;
            var offsetY = currentPosition.Y - _positionInBlock.Y;

            _transform = new TranslateTransform(offsetX, offsetY);
            RenderTransform = _transform;

            base.OnPointerMoved(e);
        }
    }
}