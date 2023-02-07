using System.Reactive;
using ReactiveUI;

namespace MedEye.ViewModels
{
    public class SetupMenuViewModel : ViewModelBase
    {
        public int UserId = 0;
        public ReactiveCommand<Unit, IRoutableViewModel> GoMainMenu { get; }
        public SetupMenuViewModel(IScreen screen, int userId)
        {
            UserId = userId;
            HostScreen = screen;
            GoMainMenu = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new MainMenuViewModel(screen)));
        }
    }
}