using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MedEye.Views
{
    public partial class ModalWindow : Window
    {
        public ModalWindow()
        {
            InitializeComponent();

            var button = this.FindControl<Button>("With");
            button.Click += WithClick;

            var button2 = this.FindControl<Button>("Without");
            button2.Click += WithoutClick;
        }

        private void WithClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WithoutClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
