﻿using System.Collections.ObjectModel;
using System.Windows;
using Caliburn.Micro;

using WeatherReport.WinApp.Events;

namespace WeatherReport.WinApp.ViewModels;

public class WindDirectionViewModel : PropertyChangedBase, IHandle<WeatherUpdated>
{
    private const double BEAUFORT_HURRICANE = 12.0;
    private const double BEAUFORT_GALE = 8.0;
    private const double BEAUFORT_DISPLAY_LIMIT = BEAUFORT_GALE;

    private const float NORMAL_HUE = 211.0F;
    private const float NORMAL_SAT = 0.4F;
    private const float NORMAL_VAL = 0.65F;
    private const float NORMAL_SAT_DIFF = 0.1F;
    private const float NORMAL_VAL_DIFF = 0.22F;

	private readonly IEventAggregator _eventAggregator;

    private double _centerX;
    private double _centerY;
    private double _radius;

    private HsvColor _arrowLeftWingColor;
    private HsvColor _arrowRightWingColor;

    private ObservableCollection<Point> _arrowLeftWingPoints;
    private ObservableCollection<Point> _arrowRightWingPoints;

    private object _polygonLock = new object();

    public HsvColor ArrowLeftWingColor
    {
        get { return _arrowLeftWingColor; }
    }
    public HsvColor ArrowRightWingColor
    {
        get { return _arrowRightWingColor; }
    }

    public ObservableCollection<Point> ArrowLeftWingPoints
    {
        get { return _arrowLeftWingPoints; }
    }
    public ObservableCollection<Point> ArrowRightWingPoints
    {
        get { return _arrowRightWingPoints; }
    }

    public WindDirectionViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _eventAggregator.SubscribeOnPublishedThread(this);

        _centerX = 50;
        _centerY = 50;
        _radius = 40;

        _arrowLeftWingColor = GetColorForNormalDirection(-0.25*Math.PI);
        _arrowRightWingColor = GetColorForNormalDirection(0.75 * Math.PI);

        _arrowLeftWingPoints = new ObservableCollection<Point>{ new Point(_centerX, _centerY) };
        _arrowRightWingPoints = new ObservableCollection<Point>{ new Point(_centerX, _centerY) };
    }

    public Task HandleAsync(WeatherUpdated message, CancellationToken cancellationToken)
    {
        RecalculateArrowData(message.Weather.WindSpeed, message.Weather.WindDirection);
        return Task.CompletedTask;
    }

    private void RecalculateArrowData(double? windSpeed, double? windDirection)
    {
        if (windSpeed.HasValue && windDirection.HasValue)
        {
            double arrowTipDirection = windDirection.Value + Math.PI;
            double len = _radius * GetBeaufortFromMetersPerSecond(windSpeed.Value) / BEAUFORT_DISPLAY_LIMIT;
            double arrowTipX = _centerX + len * Math.Sin(arrowTipDirection);
            double arrowTipY = _centerY - len * Math.Cos(arrowTipDirection);

            double da = 5.0 * Math.PI / 6.0;
            double arrowLeftWingDirection = arrowTipDirection - da;
            double arrowRightWingDirection = arrowTipDirection + da;
            double wingLen = len / 3.0;
            double arrowLeftWingX = _centerX + wingLen * Math.Sin(arrowLeftWingDirection);
            double arrowLeftWingY = _centerY - wingLen * Math.Cos(arrowLeftWingDirection);

            double arrowRightWingX = _centerX + wingLen * Math.Sin(arrowRightWingDirection);
            double arrowRightWingY = _centerY - wingLen * Math.Cos(arrowRightWingDirection);

            lock (_polygonLock)
            {
                if (_arrowLeftWingPoints.Count > 1)
                {
                    _arrowLeftWingPoints.RemoveAt(2);
                    _arrowLeftWingPoints.RemoveAt(1);
                }
                _arrowLeftWingPoints.Add(new Point(arrowLeftWingX, arrowLeftWingY));
                _arrowLeftWingPoints.Add(new Point(arrowTipX, arrowTipY));

                if (_arrowRightWingPoints.Count > 1)
                {
                    _arrowRightWingPoints.RemoveAt(2);
                    _arrowRightWingPoints.RemoveAt(1);
                }
                _arrowRightWingPoints.Add(new Point(arrowRightWingX, arrowRightWingY));
                _arrowRightWingPoints.Add(new Point(arrowTipX, arrowTipY));
            }

            _arrowLeftWingColor = GetColorForNormalDirection(windDirection.Value - 1.5 * Math.PI);
            _arrowRightWingColor = GetColorForNormalDirection(windDirection.Value - .5 * Math.PI);
            NotifyOfPropertyChange(() => ArrowLeftWingColor);
            NotifyOfPropertyChange(() => ArrowRightWingColor);
            NotifyOfPropertyChange(() => ArrowLeftWingPoints);
            NotifyOfPropertyChange(() => ArrowRightWingPoints);
        }
        else
        {
            lock (_polygonLock)
            {
                if (_arrowLeftWingPoints.Count > 1)
                {
                    _arrowLeftWingPoints.RemoveAt(2);
                    _arrowLeftWingPoints.RemoveAt(1);
                }

                if (_arrowRightWingPoints.Count > 1)
                {
                    _arrowRightWingPoints.RemoveAt(2);
                    _arrowRightWingPoints.RemoveAt(1);
                }
            }
            NotifyOfPropertyChange(() => ArrowLeftWingPoints);
            NotifyOfPropertyChange(() => ArrowRightWingPoints);
        }
    }

    private double GetBeaufortFromMetersPerSecond(double mps)
    {
        const double TWO_THIRDS = 2.0/3.0;
        return Math.Pow(mps / 0.836, TWO_THIRDS);
    }

    private HsvColor GetColorForNormalDirection(double normalDirection)
    {
        double sat = NORMAL_SAT - NORMAL_SAT_DIFF * Math.Cos(normalDirection + 0.25 * Math.PI);
        double val = NORMAL_VAL + NORMAL_VAL_DIFF * Math.Cos(normalDirection + 0.25 * Math.PI);

        return new HsvColor(NORMAL_HUE, (float)sat, (float)val);
    }
}