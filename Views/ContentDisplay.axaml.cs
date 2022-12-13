using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace MedEye.Views
{
    public partial class ContentDisplay : Window
    {
        private static readonly DispatcherTimer close_timer = new DispatcherTimer();
        private static readonly DispatcherTimer audio_timer = new DispatcherTimer();

        public ContentDisplay()
        {
            InitializeComponent();
            Comics.Source = new Bitmap("..\\..\\..\\Assets\\1_1.png");
            // Comics.Source = new Bitmap(".\\1_1.png");
            Next.Click += ExitClick;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);
        }

        public ContentDisplay(int comics_number, EventHandler<RoutedEventArgs> act)
        {
            InitializeComponent();
            // Comics.Source = new Bitmap($".\\1_{comics_number}.png");
            Comics.Source = new Bitmap($"..\\..\\..\\Assets\\1_{comics_number}.png");
            Next.Click += act;
            Next.Click += ExitClick;
            Tracker.Tracker.StartTracking();

            audio_timer.Tick += EnableButton;
            audio_timer.Interval = new TimeSpan(0, 0, 30);

            if(comics_number < 3)
            {
                Next.IsEnabled = false;
                System.Media.SoundPlayer player = new System.Media.SoundPlayer($".\\audio_{comics_number}.wav");
                player.Play();
                audio_timer.Start();
            }
            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);
        }

        private void CloseAfterRoute(object? sender, EventArgs e)
        {
            this.Close();
            close_timer.Stop();
        }

        private void EnableButton(object? sender, EventArgs e)
        {
            Next.IsEnabled = true;
            audio_timer.Stop();
        }

        private void ExitClick(object? sender, RoutedEventArgs e)
        {
            close_timer.Start();
        }

        protected override void OnOpened(EventArgs e)
        {
            AdaptToScreen();

            base.OnOpened(e);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            if (!this.IsInitialized)
                return;

            var reg = this.Get<Button>("Next");
            var cab = this.Get<Image>("Comics");
            if (!reg.IsInitialized || !cab.IsInitialized)
                return;

            if (change.Property.Name == "WindowState" || change.Property.Name == "Width")
            {
                AdaptToScreen();
            }
            base.OnPropertyChanged(change);
        }

        private void AdaptToScreen()
        {
            var buttonWidth = 2 * this.ClientSize.Width / 9;

            Next.Width = buttonWidth;
            Next.FontSize = 32 * (this.ClientSize.Width / 1920);
        }
    }
}
