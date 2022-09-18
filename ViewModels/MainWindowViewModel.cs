using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MedEye.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedEye.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // public string Greeting => "Welcome to Avalonia!";
        public void OnClickCommand()
        {
            new Tyr().Show();
        }

    }
}
