using System.Windows;
using System.Windows.Controls;

using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.WinApp.Views
{
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
           InitializeComponent();
            _viewModel = (ScrewViewModel)this.root.DataContext;
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
            if (screw != null && screw._viewModel != null)
            {
                screw._viewModel.SetSize(screw.Size);
                screw._viewModel.RecalculateSlitCoordinates(screw.SlitDirection);
            }
        }

        private static void SlitDirection_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ScrewView? screw = obj as ScrewView;
            if (screw != null && screw._viewModel != null)
                screw._viewModel.RecalculateSlitCoordinates(screw.SlitDirection);
        }
    }
}
