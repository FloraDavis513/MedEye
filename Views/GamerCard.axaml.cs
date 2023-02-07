using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using MedEye.ViewModels;

namespace MedEye.Views;

public partial class GamerCard : ReactiveUserControl<GamerCardViewModel>
{
    public GamerCard()
    {
        InitializeComponent();
        var button2 = this.FindControl<Button>("Save");
        button2.Click += SaveClick;
    }

    private void SaveClick(object? sender, RoutedEventArgs e)
    {
        var converter = new BrushConverter();
        var baseColor = (Brush)converter.ConvertFrom("#1372B7");
        FirstName.BorderBrush = baseColor;
        SecondName.BorderBrush = baseColor;
        LastName.BorderBrush = baseColor;
        BirthDate.BorderBrush = baseColor;
        Sex.BorderBrush = baseColor;
        if (string.IsNullOrEmpty(FirstName.Text) ||
            string.IsNullOrEmpty(SecondName.Text) ||
            string.IsNullOrEmpty(LastName.Text) ||
            string.IsNullOrEmpty(BirthDate.Text) ||
            string.IsNullOrEmpty(Sex.Text))
        {
            if (string.IsNullOrEmpty(FirstName.Text))
                FirstName.BorderBrush = Brushes.Red;
            if (string.IsNullOrEmpty(SecondName.Text))
                SecondName.BorderBrush = Brushes.Red;
            if (string.IsNullOrEmpty(LastName.Text))
                LastName.BorderBrush = Brushes.Red;
            if (string.IsNullOrEmpty(BirthDate.Text))
                BirthDate.BorderBrush = Brushes.Red;
            if (string.IsNullOrEmpty(Sex.Text))
                Sex.BorderBrush = Brushes.Red;
            Caption.IsVisible = true;
            return;
        }
        Caption.IsVisible = false;

        FirstName.BorderBrush = baseColor;
        SecondName.BorderBrush = baseColor;
        LastName.BorderBrush = baseColor;
        BirthDate.BorderBrush = baseColor;
        Sex.BorderBrush = baseColor;
        ViewModel!.Save.Execute();
    }
}