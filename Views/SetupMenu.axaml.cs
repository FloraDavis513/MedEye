using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace MedEye.Views;

public partial class SetupMenu : Window
{
    private static readonly DispatcherTimer closeTimer = new DispatcherTimer();
    
    public SetupMenu()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

        MainMenu.Click += MainMenuClick;
        StartGame.Click += StartGameClick;
        
        closeTimer.Tick += CloseAfterRoute;
        closeTimer.Interval = new TimeSpan(1000000);
    }

    protected override void OnOpened(EventArgs e)
    {
        AdaptToScreen();

        base.OnOpened(e);
    }

    private void MainMenuClick(object? sender, RoutedEventArgs e)
    {
        new MainMenu().Show();
        closeTimer.Start();
    }
    
    private void CloseAfterRoute(object? sender, EventArgs e)
    {
        Close();
        closeTimer.Stop();
    }
    
    private void StartGameClick(object? sender, RoutedEventArgs e)
    {
        var game = Game1.SelectedIndex;
        switch (game)
        {
            case 0:
                new Tyr().Show();
                break;
            case 1:
                new Following().Show();
                break;
            case 2:
                new Combination().Show();
                break;
            case 3:
                new Merger().Show();
                break;
        }
    }

    private void AdaptToScreen()
    {
        var buttonWidth = 2 * this.ClientSize.Width / 9;

        Game1.Width = buttonWidth;
        MainMenu.Width = buttonWidth;
        DeleteGame.Width = buttonWidth;
        StartGame.Width = buttonWidth;
        FrequencyFlickerBox.Width = buttonWidth;
        TypeFlickerBox.Width = buttonWidth;

        Game1.FontSize = 32 * (this.ClientSize.Width / 1920);
        MainMenu.FontSize =  32 * (this.ClientSize.Width / 1920);
        DeleteGame.FontSize =  32 * (this.ClientSize.Width / 1920);
        StartGame.FontSize =  32 * (this.ClientSize.Width / 1920);
        Distance.FontSize =  32 * (this.ClientSize.Width / 1920);
        LeftFilterColor.FontSize =  32 * (this.ClientSize.Width / 1920);
        FrequencyFlickerText.FontSize =  32 * (this.ClientSize.Width / 1920);
        FrequencyFlickerBox.FontSize =  32 * (this.ClientSize.Width / 1920);
        TypeFlickerText.FontSize =  32 * (this.ClientSize.Width / 1920);
        TypeFlickerBox.FontSize =  32 * (this.ClientSize.Width / 1920);
        BrightRedColor.FontSize =  32 * (this.ClientSize.Width / 1920);
        BrightBlueColor.FontSize =  32 * (this.ClientSize.Width / 1920);
        Level.FontSize =  32 * (this.ClientSize.Width / 1920);
        Timer.FontSize =  32 * (this.ClientSize.Width / 1920);

        Header1.FontSize = 48 * (this.ClientSize.Width / 1920);
        Header2.FontSize = 48 * (this.ClientSize.Width / 1920);
    }
}