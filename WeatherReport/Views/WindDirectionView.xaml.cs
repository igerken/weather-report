using System.Windows.Controls;
using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.WinApp.Views;

public partial class WindDirectionView : UserControl
{
    private WindDirectionViewModel _viewModel;
    public WindDirectionView()
    {
        InitializeComponent();
        _viewModel = (WindDirectionViewModel)root.DataContext;
    } 
}