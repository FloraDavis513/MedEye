using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MedEye.ViewModels;
using MedEye.Views;

namespace MedEye
{
    public partial class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            AppBootstrapper.RegisterService();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new StartView()
                {
                    DataContext = new StartViewModel()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
