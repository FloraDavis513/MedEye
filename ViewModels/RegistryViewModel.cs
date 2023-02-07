using System.Collections.ObjectModel;
using System.Reactive;
using MedEye.DB;
using ReactiveUI;

namespace MedEye.ViewModels
{
    public class RegistryViewModel : ViewModelBase
    {
        private ReactiveCommand<Unit, IRoutableViewModel> GoGameCard { get; }
        private ReactiveCommand<Unit, IRoutableViewModel> GoMainMenu { get; }
        private ReactiveCommand<Unit, IRoutableViewModel> GoChangeGameCard { get; }
        private ReactiveCommand<Unit, IRoutableViewModel> GoStatTable { get; }

        #region Fields

        private readonly SortedDictionary<string, int> _actualUsers;
        private ObservableCollection<string> ShowUsers { get; set; } = new();

        private string _selectedUser = string.Empty;
        public string SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }

        private bool _isButtonEnabled = false;
        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref _isButtonEnabled, value);
        }

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedIndex, value);
                IsButtonEnabled = value != -1;
            }
        }

        #endregion

        public RegistryViewModel(IScreen screen)
        {
            HostScreen = screen;
            _actualUsers = Users.GetUserList();
            ActualizeUserList();
            GoGameCard = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new GamerCardViewModel(screen)));
            GoMainMenu = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new MainMenuViewModel(screen)));
            GoChangeGameCard = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new GamerCardViewModel(screen, GetSelectedUserId())));
            GoStatTable = ReactiveCommand.CreateFromObservable(() =>
                HostScreen.Router.Navigate.Execute(new StatTableViewModel(screen, GetSelectedUserId())));
        }

        private void ActualizeUserList() => 
            ShowUsers = new ObservableCollection<string>(_actualUsers.Select(u => u.Key).ToList());

        public void DeleteUser()
        {
            Users.DeleteUserById(_actualUsers[_selectedUser]);
            ShowUsers.Remove(_selectedUser);
            SelectedIndex = -1;
            SelectedUser = string.Empty;
        }

        private int GetSelectedUserId() => 
            _actualUsers[_selectedUser];
    }
}
