using ReactiveUI;

namespace MedEye.ViewModels
{
    public class SetupMenuViewModel : ViewModelBase
    {
        public SetupMenuViewModel(IScreen screen, int id)
        {
            HostScreen = screen;
        }
    }
}