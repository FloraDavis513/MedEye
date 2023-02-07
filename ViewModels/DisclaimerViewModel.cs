using System.Reactive;
using ReactiveUI;

namespace MedEye.ViewModels
{
    public class DisclaimerViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, IRoutableViewModel> GoMainMenu { get; }
        public DisclaimerViewModel(IScreen screen)
        {
            HostScreen = screen;
            UrlPathSegment = nameof(DisclaimerViewModel);
            GoMainMenu = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new MainMenuViewModel(screen)));
        }
    }
}
