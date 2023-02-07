using ReactiveUI;

namespace MedEye.ViewModels;

public class StartViewModel : ReactiveObject, IScreen
{
	public RoutingState Router { get; } = new();

	public StartViewModel()
	{
		Router.Navigate.Execute(new DisclaimerViewModel(this));
	}
}