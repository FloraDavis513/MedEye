using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using MedEye.ViewModels;

namespace MedEye.Views
{
    public partial class Registry : ReactiveUserControl<RegistryViewModel>
    {
        public Registry()
        {
            InitializeComponent();
            Delete.Click += DeleteClick;
        }

        private async void DeleteClick(object? sender, RoutedEventArgs e)
        {
            if (ViewModel!.SelectedIndex == -1)
                return;
            await new ConfirmAction($"Вы уверены, что хотите удалить игрока:{ViewModel.SelectedUser} ?", DoDelete)
                .ShowDialog(ViewModel.HostWindow);
        }

        private void DoDelete(object? sender, RoutedEventArgs e) => ViewModel!.DeleteUser();
    }
}
