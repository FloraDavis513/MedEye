using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using MedEye.Consts;
using MedEye.Controls;
using System;
using Avalonia;
using MedEye.DB;

namespace MedEye.Views
{
    public partial class Merger : Window
    {
        // Random generator for target pos.
        static Random rnd = new Random();

        private static readonly DispatcherTimer after_move_reset_timer = new DispatcherTimer();

        // Blink
        private static readonly DispatcherTimer PartOneBlinkTimer = new DispatcherTimer();
        private static readonly DispatcherTimer PartTwoBlinkTimer = new DispatcherTimer();

        // Close game
        private static readonly DispatcherTimer CloseGameTimer = new DispatcherTimer();

        // Scores
        private long _countScores = 0;
        private Scores _scores = new Scores();

        public Merger()
        {
            InitializeComponent();

            after_move_reset_timer.Tick += ResetColor;
            after_move_reset_timer.Interval = new TimeSpan(500000);

            DragControl first = this.Get<DragControl>("PartOne");
            double first_height = first.Height;
            double first_width = first.Width;
            Canvas.SetTop(first, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - first_height)));
            Canvas.SetLeft(first, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - first_width)));

            DragControl second = this.Get<DragControl>("PartTwo");
            double second_height = second.Height;
            double second_width = second.Width;
            Canvas.SetTop(second, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - second_height)));
            Canvas.SetLeft(second, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - second_width)));

            CloseGameTimer.Tick += CloseGame;
            CloseGameTimer.Interval = new TimeSpan(0, 5, 0);

            SetDefaultScores(0, 4, 1);

            StartBlink(4);
            CloseGameTimer.Start();
        }

        public Merger(Settings settings)
        {
            InitializeComponent();

            after_move_reset_timer.Tick += ResetColor;
            after_move_reset_timer.Interval = new TimeSpan(500000);

            Canvas.SetTop(PartOne, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - PartOne.Height)));
            Canvas.SetLeft(PartOne, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - PartOne.Width)));

            Canvas.SetTop(PartTwo, rnd.Next(0, Convert.ToInt32(this.ClientSize.Height - PartTwo.Height)));
            Canvas.SetLeft(PartTwo, rnd.Next(0, Convert.ToInt32(this.ClientSize.Width - PartTwo.Width)));

            CloseGameTimer.Tick += CloseGame;
            CloseGameTimer.Interval = new TimeSpan(0, 0, settings.ExerciseDuration);

            SetDefaultScores(settings.UserId, settings.GameId, settings.Level);

            StartBlink(settings.FlickerMode, settings.Frequency);
            CloseGameTimer.Start();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (after_move_reset_timer.IsEnabled)
                    after_move_reset_timer.Stop();
                Close();
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
            }

            CalculateScore();

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

        private void StartBlink(int mode = 2, int frequency = 10)
        {
            PartOneBlinkTimer.Tick += PartOneBlink;
            PartOneBlinkTimer.Interval = new TimeSpan(0, 0, 0, (int)(1.0 / frequency));

            PartTwoBlinkTimer.Tick += PartTwoBlink;
            PartTwoBlinkTimer.Interval = new TimeSpan(0, 0, 0, (int)(1.0 / frequency));

            switch (mode)
            {
                case 0:
                    PartOneBlinkTimer.Start();
                    break;
                case 1:
                    PartTwoBlinkTimer.Start();
                    break;
                case 2:
                    PartOneBlinkTimer.Start();
                    PartTwoBlinkTimer.Start();
                    break;
                case 4:
                    PartOneBlinkTimer.Stop();
                    PartTwoBlinkTimer.Stop();
                    break;
            }
        }

        private void PartOneBlink(object? sender, EventArgs e)
        {
            BigOne.Background = Equals(BigOne.Background, ColorConst.STRABISMUS_FIRST_COLOR)
                ? ColorConst.STRABISMUS_MOVE_COLOR
                : ColorConst.STRABISMUS_FIRST_COLOR;

            SmallTwo.Background = Equals(SmallTwo.Background, ColorConst.STRABISMUS_FIRST_COLOR)
                ? ColorConst.STRABISMUS_MOVE_COLOR
                : ColorConst.STRABISMUS_FIRST_COLOR;
        }

        private void PartTwoBlink(object? sender, EventArgs e)
        {
            BigTwo.Background = Equals(BigTwo.Background, ColorConst.STRABISMUS_SECOND_COLOR)
                ? ColorConst.STRABISMUS_MOVE_COLOR
                : ColorConst.STRABISMUS_SECOND_COLOR;

            SmallTwo.Background = Equals(SmallTwo.Background, ColorConst.STRABISMUS_SECOND_COLOR)
                ? ColorConst.STRABISMUS_MOVE_COLOR
                : ColorConst.STRABISMUS_SECOND_COLOR;
        }

        private void CloseGame(object? sender, EventArgs e)
        {
            PartOneBlinkTimer.Stop();
            PartTwoBlinkTimer.Stop();
            CloseGameTimer.Stop();

            _scores.DateCompletion = DateTime.Now;
            ScoresWrap.AddScores(_scores);

            ShowResult();

            CloseGameTimer.Tick -= CloseGame;
            CloseGameTimer.Tick += CloseGameAfterShowResult;
            CloseGameTimer.Interval = new TimeSpan(0, 0, 5);
            CloseGameTimer.Start();
        }

        private void SetDefaultScores(int userId, int gameId, int level)
        {
            _scores.UserId = userId;
            _scores.GameId = gameId;
            _scores.Level = level;
            _scores.MaxDeviationsX = 0;
            _scores.MaxDeviationsY = 0;
            _scores.MinDeviationsX = double.MaxValue;
            _scores.MinDeviationsY = double.MaxValue;
        }

        private void CalculateScore()
        {
            var targetCenterX = Canvas.GetLeft(PartOne) + PartOne.Width / 2;
            var targetCenterY = Canvas.GetTop(PartOne) + PartOne.Height / 2 - 150;

            var stalkerCenterX = Canvas.GetLeft(PartTwo) + PartTwo.Width / 2;
            var stalkerCenterY = Canvas.GetTop(PartTwo) + PartTwo.Height / 2;

            var offsetX = targetCenterX - stalkerCenterX;
            var offsetY = targetCenterY - stalkerCenterY;

            if (Math.Abs(offsetX) <= Math.Abs(_scores.MinDeviationsX))
            {
                _scores.MinDeviationsX = offsetX;
            }

            if (Math.Abs(offsetY) <= Math.Abs(_scores.MinDeviationsY))
            {
                _scores.MinDeviationsY = offsetY;
            }

            if (Math.Abs(offsetX) >= Math.Abs(_scores.MaxDeviationsX))
            {
                _scores.MaxDeviationsX = offsetX;
            }

            if (Math.Abs(offsetY) >= Math.Abs(_scores.MaxDeviationsY))
            {
                _scores.MaxDeviationsY = offsetY;
            }

            _scores.MeanDeviationsX = (_scores.MeanDeviationsX * _countScores + offsetX) / (_countScores + 1);
            _scores.MeanDeviationsY = (_scores.MeanDeviationsY * _countScores + offsetY) / (_countScores + 1);
            _countScores++;

            if (targetCenterX >= Canvas.GetLeft(PartTwo)
                && targetCenterX <= Canvas.GetLeft(PartTwo) + PartTwo.Width
                && targetCenterY >= Canvas.GetTop(PartTwo)
                && targetCenterY <= Canvas.GetTop(PartTwo) + PartTwo.Height)
            {
                _scores.Score += 1;
            }
            else
            {
                _scores.Score -= 1;
            }
        }

        private void ShowResult()
        {
            Result.Content = "Результат игры:\n" + _scores;
            Result.FontSize = 32 * (ClientSize.Width / 1920);
            Result.Height = ClientSize.Height / 3 - 25;
            Result.Width = ClientSize.Width / 2 - 25;
            Log.Height = ClientSize.Height / 3;
            Log.Width = ClientSize.Width / 2;
            Log.CornerRadius = new CornerRadius(15);
            Log.Opacity = 1;
            Canvas.SetTop(Log, ClientSize.Height / 2 - Log.Height / 2);
            Canvas.SetLeft(Log, ClientSize.Width / 2 - Log.Width / 2);
        }

        private void CloseGameAfterShowResult(object? sender, EventArgs e)
        {
            CloseGameTimer.Stop();
            Close();
        }
    }
}