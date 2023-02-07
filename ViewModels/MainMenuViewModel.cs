using System.Diagnostics;
using System.Reactive;
using ReactiveUI;

namespace MedEye.ViewModels;

public class MainMenuViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, IRoutableViewModel> GoSetup { get; }
    private ReactiveCommand<Unit, IRoutableViewModel> GoRegistry { get; }
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

    public void ShowInfo()
    {
        var p = new Process();
        p.StartInfo = new ProcessStartInfo("..\\..\\..\\Docs\\Справка.docx")
        {
            UseShellExecute = true
        };
        p.Start();
    }
}