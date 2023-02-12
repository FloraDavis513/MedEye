using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MedEye.Services;
using MedEye.ViewModels;
using ReactiveUI;
using Splat;

namespace MedEye.Views;

public partial class StartView : ReactiveWindow<StartViewModel>
{
	public StartView()
	{
		var deviceService = (IDeviceService)Locator.Current.GetService(typeof(IDeviceService));
		this.WhenActivated(_ => deviceService.SetSize(new Size(Width, Height)));
		AvaloniaXamlLoader.Load(this);
		this.AttachDevTools();
	}
}