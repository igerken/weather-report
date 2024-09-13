using System.Windows;
using System.Windows.Input;

namespace WeatherReport;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        //InitializeComponent();
    }

    private void Shutdown_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }
}
