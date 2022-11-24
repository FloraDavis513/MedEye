using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using MedEye.Consts;
using MedEye.Tracker;
using System;
using System.Globalization;
using MedEye.DB;

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

        // Blink
        private static readonly DispatcherTimer TargetBlinkTimer = new DispatcherTimer();
        private static readonly DispatcherTimer ScopeBlinkTimer = new DispatcherTimer();

        // Close game
        private static readonly DispatcherTimer CloseGameTimer = new DispatcherTimer();

        // Scores
        private long _countScores = 0;
        private Scores _scores = new Scores();

        public Tyr()
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
            
            Tracker.Tracker.StartTracking();

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

            CloseGameTimer.Tick += CloseGame;
            CloseGameTimer.Interval = new TimeSpan(0, 5, 0);

            SetDefaultScores(0, 1, 1);

            StartBlink(4);
            CloseGameTimer.Start();
        }

        public Tyr(Settings settings)
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
            
            Tracker.Tracker.StartTracking();

            after_move_reset_timer.Tick += ResetColor;
            after_move_reset_timer.Interval = new TimeSpan(500000);

            flash_timer.Tick += WinFlash;
            flash_timer.Interval = new TimeSpan(300000);

            Canvas.SetTop(Target, rnd.Next(0, Convert.ToInt32(ClientSize.Height - Target.Height)));
            Canvas.SetLeft(Target, rnd.Next(0, Convert.ToInt32(ClientSize.Width - Target.Width)));

            CloseGameTimer.Tick += CloseGame;
            CloseGameTimer.Interval = new TimeSpan(0, 0, settings.ExerciseDuration);

            SetDefaultScores(settings.UserId, settings.GameId, settings.Level);

            StartBlink(settings.FlickerMode, settings.Frequency);
            CloseGameTimer.Start();
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
            scope.CornerRadius = CornerRadius.Parse((DifficultConst.TYR_SIZES[level] / 2).ToString());
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
                if (++success_counter >= 5 && level < DifficultConst.TYR_SIZES.Length - 1)
                {
                    success_counter = 0;
                    SetDifficultLevel(++level);
                }
            }

            CalculateScore();

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
                Close();
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
            if (flash_count == 6)
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

        private void StartBlink(int mode = 2, int frequency = 10)
        {
            TargetBlinkTimer.Tick += TargetBlink;
            TargetBlinkTimer.Interval = new TimeSpan(0, 0, 0, (int)(1.0 / frequency));

            ScopeBlinkTimer.Tick += ScopeBlink;
            ScopeBlinkTimer.Interval = new TimeSpan(0, 0, 0, (int)(1.0 / frequency));

            switch (mode)
            {
                case 0:
                    TargetBlinkTimer.Start();
                    break;
                case 1:
                    ScopeBlinkTimer.Start();
                    break;
                case 2:
                    TargetBlinkTimer.Start();
                    ScopeBlinkTimer.Start();
                    break;
                case 4:
                    TargetBlinkTimer.Stop();
                    ScopeBlinkTimer.Stop();
                    break;
            }
        }

        private void TargetBlink(object? sender, EventArgs e)
        {
            Target.Stroke = Equals(Target.Stroke, colors[currentColor])
                ? ColorConst.AMBLYOPIA_MOVE_COLOR
                : colors[currentColor];

            Target.Fill = Equals(Target.Fill, colors[currentColor])
                ? ColorConst.AMBLYOPIA_MOVE_COLOR
                : colors[currentColor];
        }

        private void ScopeBlink(object? sender, EventArgs e)
        {
            Scope.Background = Equals(Scope.Background, colors[currentColor])
                ? ColorConst.AMBLYOPIA_MOVE_COLOR
                : colors[currentColor];
        }

        private void CloseGame(object? sender, EventArgs e)
        {
            TargetBlinkTimer.Stop();
            ScopeBlinkTimer.Stop();
            CloseGameTimer.Stop();
            
            var trackerResult = Tracker.Tracker.GetResult();
            _scores.Involvement = Math.Round(double.Parse(trackerResult.Replace(",", "."),
                CultureInfo.InvariantCulture) / CloseGameTimer.Interval.TotalSeconds, 2);

            _scores.DateCompletion = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
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
            var targetCenterX = Canvas.GetLeft(Scope) + Scope.Width / 2;
            var targetCenterY = Canvas.GetTop(Scope) + Scope.Height / 2;

            var stalkerCenterX = Canvas.GetLeft(Target) + Target.Width / 2;
            var stalkerCenterY = Canvas.GetTop(Target) + Target.Height / 2;

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

            if (targetCenterX >= Canvas.GetLeft(Target)
                && targetCenterX <= Canvas.GetLeft(Target) + Target.Width
                && targetCenterY >= Canvas.GetTop(Target)
                && targetCenterY <= Canvas.GetTop(Target) + Target.Height)
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