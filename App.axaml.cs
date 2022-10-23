using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MedEye.ViewModels;
using MedEye.Views;

namespace MedEye
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainMenu
                {
                    DataContext = new MainMenuViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
