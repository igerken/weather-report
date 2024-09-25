using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.WinApp.Views
{
    /// <summary>
    /// Interaction logic for Screw.xaml
    /// </summary>
    public partial class ScrewView : UserControl
    {
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(double), typeof(ScrewView),
                new PropertyMetadata(new PropertyChangedCallback(Size_PropertyChanged)));
        public static readonly DependencyProperty SlitDirectionProperty =
            DependencyProperty.Register("SlitDirection", typeof(double), typeof(ScrewView),
                new PropertyMetadata(new PropertyChangedCallback(SlitDirection_PropertyChanged)));

        ScrewViewModel _viewModel;

        public ScrewView()
        {
           //InitializeComponent();
            _viewModel = new ScrewViewModel(Size);
            DataContext = _viewModel;
        }

        public double SlitDirection
        {
            get { return (double)GetValue(SlitDirectionProperty); }
            set { SetValue(SlitDirectionProperty, value); }
        }

        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private static void Size_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ScrewView? screw = obj as ScrewView;
            if (screw != null)
            {
                screw._viewModel.SetSize(screw.Size);
                screw._viewModel.RecalculateSlitCoordinates(screw.SlitDirection);
            }
        }

        private static void SlitDirection_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ScrewView? screw = obj as ScrewView;
            if (screw != null)
                screw._viewModel.RecalculateSlitCoordinates(screw.SlitDirection);
        }
    }
}
