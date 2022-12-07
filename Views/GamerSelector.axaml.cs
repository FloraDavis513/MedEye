using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MedEye.DB;

namespace MedEye.Views
{
    public partial class GamerSelector : Window
    {
        private SortedDictionary<string, int> actual_users;
        private static readonly DispatcherTimer close_timer = new DispatcherTimer();
        private double windowWidth;
        private double windowHeight;

        public GamerSelector()
        {
            InitializeComponent();
            ActualizeUserList();

            Next.Click += NextClick;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);
        }

        public GamerSelector(double main_window_width, double main_window_height, EventHandler<RoutedEventArgs> act)
        {
            InitializeComponent();
            ActualizeUserList();

            windowWidth = 1285 * (main_window_width / 1920);
            windowHeight = 690 * (main_window_height / 1080);

            Next.Click += NextClick;
            Next.Click += act;

            close_timer.Tick += CloseAfterRoute;
            close_timer.Interval = new TimeSpan(1000000);
        }

        private void CloseAfterRoute(object? sender, EventArgs e)
        {
            this.Close();
            close_timer.Stop();
        }

        private void NextClick(object? sender, RoutedEventArgs e)
        {
            if (UserList.SelectedIndex == -1)
                return;
            string selected_user = (string)UserList.SelectedItem;

            new SetupMenu(actual_users[selected_user]).Show();
            close_timer.Start();
        }

        private void ActualizeUserList()
        {
            actual_users = Users.GetUserList();
            var users = new List<string>();
            foreach (var item in actual_users)
                users.Add(item.Key);

            UserList.Items = users;
        }

        protected override void OnOpened(EventArgs e)
        {
            AdaptToScreen();
            base.OnOpened(e);
        }

        private void AdaptToScreen()
        {
            this.Width = windowWidth;
            this.Height = windowHeight;
            var buttonWidth = this.Width / 3;

            Next.Width = buttonWidth;
            Next.FontSize = 32 * (windowWidth / 1920);
            Header.FontSize = 48 * (windowWidth / 1920);
        }
    }
}