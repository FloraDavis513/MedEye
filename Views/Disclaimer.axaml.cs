using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MedEye.ViewModels;
using ReactiveUI;

namespace MedEye.Views
{
    public partial class Disclaimer : ReactiveUserControl<DisclaimerViewModel>
    {
        public Disclaimer()
        {
            this.WhenActivated(_ => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
