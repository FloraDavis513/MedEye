using MedEye.Views;

namespace MedEye.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public void OnClickCommand()
        {
            new Tyr().Show();
        }
    }
}
