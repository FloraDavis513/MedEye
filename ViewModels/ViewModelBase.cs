using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace MedEye.ViewModels
{
	public class ViewModelBase : ReactiveObject, IRoutableViewModel
	{
		public string? UrlPathSegment { get; set; }
		public IScreen? HostScreen { get; set; }
		public Window? HostWindow { get; set; }
		protected ViewModelBase()
		{
			HostWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
				? desktop.MainWindow
				: null;
		}
	}
}