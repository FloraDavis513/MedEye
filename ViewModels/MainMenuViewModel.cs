using System.Reactive;
using ReactiveUI;

namespace MedEye.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, IRoutableViewModel> GoSetup { get; }
        public ReactiveCommand<Unit, IRoutableViewModel> GoRegistry { get; }
        private int _selectedUserId = -1;

        public int SelectedUserId
        {
            get => _selectedUserId;
            set => this.RaiseAndSetIfChanged(ref _selectedUserId, value);
        }
        public MainMenuViewModel(IScreen screen)
        {
            HostScreen = screen;
            GoSetup = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new SetupMenuViewModel(screen, SelectedUserId)));
            GoRegistry = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new RegistryViewModel(screen)));
        }
    }
}
