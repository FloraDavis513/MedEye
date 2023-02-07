using System.Collections.ObjectModel;
using MedEye.DB;
using ReactiveUI;

namespace MedEye.ViewModels;

public class GamerSelectorViewModel : ViewModelBase
{
	private readonly SortedDictionary<string, int> _actualUsers;
	private ObservableCollection<string>? ShowUsers { get; set; }

	private string? _selectedUser;
	public string? SelectedUser
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

	private int _selectedIndex;
	public int SelectedIndex
	{
		get => _selectedIndex;
		set
		{
			this.RaiseAndSetIfChanged(ref _selectedIndex, value);
			IsButtonEnabled = value != -1;
		}
	}

	public GamerSelectorViewModel()
	{
		_actualUsers = Users.GetUserList();
		ActualizeUserList();
	}

	private void ActualizeUserList() => 
		ShowUsers = new ObservableCollection<string>(_actualUsers.Select(u => u.Key).ToList());

	public int GetSelectedUserId()
	{
		return _actualUsers[_selectedUser!];
	}
}