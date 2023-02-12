using MedEye.ViewModels;
using MedEye.Views;
using ReactiveUI;

namespace MedEye;

/// <summary>
/// Класс для роутинга по прилжению.
/// </summary>
public class AppViewLocator : IViewLocator
{
	public IViewFor ResolveView<T>(T viewModel, string? contract = null) => viewModel switch
	{
		DisclaimerViewModel context => new Disclaimer() { DataContext = context },
		MainMenuViewModel context => new MainMenu() {DataContext = context},
		SetupMenuViewModel context => new SetupMenu() { DataContext = context},
		RegistryViewModel context => new Registry() { DataContext = context},
		GamerCardViewModel context => new GamerCard() { DataContext = context},
		StatTableViewModel context => new StatTable() { DataContext = context},
		

		_ => throw new ArgumentOutOfRangeException(nameof(viewModel))
	};
}