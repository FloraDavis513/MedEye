using Avalonia;

namespace MedEye.Services;

public class DeviceService : IDeviceService
{
	private static Size _clientSize;

	public Size GetSize() => _clientSize;

	public void SetSize(Size clientSize) => _clientSize = clientSize;
}