using MedEye.Services;

namespace MedEye;

/// <summary>
/// Решала зависимостей зависимостей.
/// </summary>
internal static class AppBootstrapper
{
	/// <summary>
	/// Регистрирует зависимости.
	/// </summary>
	internal static void RegisterService()
	{
		//Сюда добавлять новые зависимости.
		Splat.Locator.CurrentMutable.Register(() => new DeviceService(), typeof(IDeviceService));
	}
}