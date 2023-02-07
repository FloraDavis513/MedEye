using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MedEye.ViewModels;
using ReactiveUI;

namespace MedEye.Views
{
    public partial class MainMenu : ReactiveUserControl<MainMenuViewModel>
    {
        public MainMenu()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);

            InitializeComponent();

            this.FindControl<Button>("Exit").
                Click += ClickExit;

            this.FindControl<Button>("Cabinet")
                .Click += ToCabinet;

            var button3 = this.FindControl<Button>("Registry");
            button3.Click += RegistryClick;
            //
            // Info.Click += InfoClick;
            //
            // close_timer.Tick += CloseAfterRoute;
            // close_timer.Interval = new TimeSpan(1000000);
        }

        private async void ToCabinet(object? s, RoutedEventArgs e)
        {
            var modalResult = await new ModalWindow().ShowDialog<string>(ViewModel!.HostWindow);

            switch (modalResult)
            {
                case "With":
                {
                    ViewModel.SelectedUserId = await new GamerSelector
                    {
                        DataContext = new GamerSelectorViewModel()
                    }.ShowDialog<int>(ViewModel!.HostWindow);
                    ViewModel.GoSetup.Execute();
                    break;
                }
                case "Without":
                {
                    ViewModel.GoSetup.Execute();
                    break;
                }
            }
        }

        private async void ClickExit(object? s, RoutedEventArgs e) =>
            await new ConfirmAction("Вы уверены, что хотите выйти?", ExitClick)
                .ShowDialog(ViewModel!.HostWindow);

        protected override void OnInitialized()
        {
            var logo = this.Get<Image>("Logo");
        
            logo.Source = new Bitmap("..\\..\\..\\Assets\\logo.png");
            // logo.Source = new Bitmap(".\\logo.png");
            base.OnInitialized();
        }

        private void ExitClick(object? e, EventArgs es)
        {
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
            mainWindow?.Close();
        }

        private void RegistryClick(object? sender, RoutedEventArgs e) => ViewModel!.GoRegistry.Execute();

        // private void InfoClick(object? sender, RoutedEventArgs e)
        // {
        //     var p = new Process();
        //     p.StartInfo = new ProcessStartInfo("..\\..\\..\\Docs\\Справка.docx")
        //     {
        //         UseShellExecute = true
        //     };
        //     p.Start();
        // }
    }
}
