using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MedEye.Views;

public partial class SetupMenu : Window
{
    public SetupMenu()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
}