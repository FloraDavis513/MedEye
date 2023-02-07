using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;

namespace MedEye.Views;

public class BaseDialogWindow<T> : ReactiveWindow<T> where T : class
{
	public new readonly Size ClientSize;

	public BaseDialogWindow()
	{
		var desktopMainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
			? desktop.MainWindow
			: null;

		ClientSize = desktopMainWindow!.ClientSize;
	}
}