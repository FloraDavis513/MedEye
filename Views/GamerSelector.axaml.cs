using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using MedEye.ViewModels;
using ReactiveUI;

namespace MedEye.Views
{
    public partial class GamerSelector : ReactiveWindow<GamerSelectorViewModel>
    {
        public GamerSelector()
        {
            this.WhenActivated(d => { });
            InitializeComponent();
            AdaptToScreen();
            Next.Click += NextClick;
        }

        private void NextClick(object? sender, RoutedEventArgs e) =>
            Close(ViewModel!.GetSelectedUserId());

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
                Close();

            base.OnKeyDown(e);
        }

        private void AdaptToScreen()
        {
            Width = 1285 * (ClientSize.Width / 1920);
            Height = 690 * (ClientSize.Height / 1080);
            var buttonWidth = Width / 3;

            Next.Width = buttonWidth;
        }
    }
}