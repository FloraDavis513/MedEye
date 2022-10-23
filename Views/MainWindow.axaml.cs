using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Controls;


namespace MedEye.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var tyr = this.FindControl<Button>("Tyr");
            tyr.Click += TyrClick;

            var following = this.FindControl<Button>("Following");
            following.Click += FollowingClick;

            var merger = this.FindControl<Button>("Merger");
            merger.Click += MergerClick;

            var combination = this.FindControl<Button>("Combination");
            combination.Click += CombinationClick;

            var exit = this.FindControl<Button>("Exit");
            exit.Click += ExitClick;

            #if DEBUG
                this.AttachDevTools();
            #endif
        }

        private void TyrClick(object? sender, RoutedEventArgs e)
        {
            new Tyr().Show();
        }

        private void FollowingClick(object? sender, RoutedEventArgs e)
        {
            new Following().Show();
        }

        private void MergerClick(object? sender, RoutedEventArgs e)
        {
            new Merger().Show();
        }

        private void CombinationClick(object? sender, RoutedEventArgs e)
        {
            new Combination().Show();
        }

        private void ExitClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

