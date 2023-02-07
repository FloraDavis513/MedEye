using Avalonia;

namespace MedEye.Services;

public interface IDeviceService
{
	Size GetSize();
	void SetSize(Size clientSize);
}