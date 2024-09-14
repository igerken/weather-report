using System.Windows;
using System.Windows.Controls;

using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.WinApp.Views;

public partial class WindDirectionView : UserControl
{
    public static readonly DependencyProperty WindSpeedProperty =
        DependencyProperty.Register("WindSpeed", typeof(double?), typeof(WindDirectionView),
            new PropertyMetadata(new PropertyChangedCallback(Wind_PropertyChanged)));
    public static readonly DependencyProperty WindDirectionProperty =
        DependencyProperty.Register("WindDirection", typeof(double?), typeof(WindDirectionView),
            new PropertyMetadata(new PropertyChangedCallback(Wind_PropertyChanged)));

    WindDirectionViewModel _viewModel;

    public WindDirectionView()
    {
        //InitializeComponent();
        _viewModel = new WindDirectionViewModel(50.0, 50.0, 40.0);
        DataContext = _viewModel;
    }

    public double? WindSpeed
    {
        get { return (double?)GetValue(WindSpeedProperty); }
        set { SetValue(WindSpeedProperty, value); }
    }

    public double? WindDirection
    {
        get { return (double?)GetValue(WindDirectionProperty); }
        set { SetValue(WindDirectionProperty, value); }
    }

    private static void Wind_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        WindDirectionView? display = obj as WindDirectionView;
        if(display != null)
        {
            display._viewModel.RecalculateArrowData(display.WindSpeed, display.WindDirection);
        }
    }
}