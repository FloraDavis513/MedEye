using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MedEye.ViewModels;

namespace MedEye.Views
{
    public partial class MainMenu : ReactiveUserControl<MainMenuViewModel>
    {
        public MainMenu()
        {
            InitializeComponent();
            AvaloniaXamlLoader.Load(this);
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
    }
}
