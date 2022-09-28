using MedEye.Views;

namespace MedEye.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public void StartTyr()
        {
            new Tyr().Show();
        }

        public void StartCombination()
        {
            new Combination().Show();
        }

        public void StartFollowing()
        {
            new Following().Show();
        }
    }
}
