using System.Reactive;
using MedEye.DB;
using ReactiveUI;

namespace MedEye.ViewModels;

public class GamerCardViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, IRoutableViewModel> GoRegistry { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> Save { get; }
    public Gamer CurrentGamer { get; set; }

    #region Fields

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }

    private string _secondName = string.Empty;
    public string SecondName
    {
        get => _secondName;
        set => this.RaiseAndSetIfChanged(ref _secondName, value);
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set => this.RaiseAndSetIfChanged(ref _lastName, value);
    }

    private string _birthDate = string.Empty;
    public string BirthDate
    {
        get => _birthDate;
        set => this.RaiseAndSetIfChanged(ref _birthDate, value);
    }

    private string _sex = string.Empty;
    public string Sex
    {
        get => _sex;
        set => this.RaiseAndSetIfChanged(ref _sex, value);
    }

    #endregion

    public GamerCardViewModel(IScreen screen)
    {
        HostScreen = screen;
        GoRegistry = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new RegistryViewModel(screen)));
        Save = ReactiveCommand.CreateFromObservable(DoSave);
    }

    public GamerCardViewModel(IScreen screen, int gamerId)
    {
        CurrentGamer = Users.GetUserById(gamerId);
        FirstName = CurrentGamer.first_name;
        SecondName = CurrentGamer.second_name;
        LastName = CurrentGamer.last_name;
        BirthDate = CurrentGamer.birth_date;
        Sex = CurrentGamer.sex;
        HostScreen = screen;
        GoRegistry = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new RegistryViewModel(screen)));
        Save = ReactiveCommand.CreateFromObservable(DoSave);
    }

    IObservable<IRoutableViewModel> DoSave()
    {
        var gamer = new Gamer
        {
            first_name = _firstName,
            second_name = _secondName,
            last_name = _lastName,
            birth_date = _birthDate,
            sex = _sex
        };
        if (CurrentGamer.id == 0)
            Users.AddUser(_firstName, _secondName, _lastName, _birthDate, _sex);
        else
        {
            gamer.id = CurrentGamer.id;
            Users.UpdateUser(CurrentGamer.id, _firstName, _secondName,_lastName, _birthDate, _sex);
        }
        return HostScreen!.Router.Navigate.Execute(new RegistryViewModel(HostScreen));
    }
}