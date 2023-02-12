using System.Reactive;
using MedEye.DB;
using ReactiveUI;

namespace MedEye.ViewModels;

public class StatTableViewModel : ViewModelBase
{
    private ReactiveCommand<Unit, IRoutableViewModel> GoRegistry { get; }
    public Gamer CurrentGamer { get; }
    public StatTableViewModel(IScreen screen, int gamerId)
    {
        CurrentGamer = Users.GetUserById(gamerId);
        HostScreen = screen;
        GoRegistry = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new RegistryViewModel(screen)));
    }
}