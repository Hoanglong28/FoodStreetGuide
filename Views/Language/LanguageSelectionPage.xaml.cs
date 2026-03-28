using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using doanC_.Views;

namespace doanC_.Views.Language;

public partial class LanguageSelectionPage : ContentPage
{
    public LanguageSelectionPage()
    {
        InitializeComponent();
    }

    private async void OnSelectVietnamese(object sender, EventArgs e)
    {
        Preferences.Set("AppLanguage", "vi");
        await NavigateToMainAsync();
    }

    private async void OnSelectEnglish(object sender, EventArgs e)
    {
        Preferences.Set("AppLanguage", "en");
        await NavigateToMainAsync();
    }

    private async Task NavigateToMainAsync()
    {
        Application.Current.MainPage = new AppShell();
        await Task.CompletedTask;
    }
}
