using MedEye.Views;

namespace MedEye.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public void StartTyr()
        {
            Tyr tyr = new Tyr();
            tyr.SetDifficultLevel(0);
            tyr.Show();
        }

        public void StartCombination()
        {
            new Combination().Show();
        }

        public void StartFollowing()
        {
            new Following().Show();
        }

        public void StartMerger()
        {
            new Merger().Show();
        }
    }
}
