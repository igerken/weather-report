﻿using System.Windows;
using System.Windows.Input;

namespace WeatherReport.WinApp.Views;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
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
