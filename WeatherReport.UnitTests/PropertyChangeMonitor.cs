using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace WeatherReport.UnitTests
{
    public class PropertyChangeMonitor : IDisposable
    {
        private const int DEFAULT_TIMEOUT_MS = 500;

        private readonly INotifyPropertyChanged _observableObject;
        private readonly int _timeout;

        private bool _isPropertyChangedEventClosed = false;
        private bool _isTimeoutElapsed = false;
        private ManualResetEvent _propertyChangedEvent = new ManualResetEvent(false);

        public PropertyChangeMonitor(INotifyPropertyChanged observableObject, string propertyName)
            :this(observableObject, propertyName, DEFAULT_TIMEOUT_MS)
        {
        }

        public PropertyChangeMonitor(INotifyPropertyChanged observableObject, string propertyName, int timeoutMilliseconds)
        {
            _timeout = timeoutMilliseconds;
            _observableObject = observableObject;
            _observableObject.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == propertyName && !_isPropertyChangedEventClosed)
                    _propertyChangedEvent.Set();
            };
        }

        public void WaitForPropertyChange()
        {
            _propertyChangedEvent.WaitOne(_timeout);
        }

        public void Dispose()
        {
            try
            {
                _isPropertyChangedEventClosed = true;
                _propertyChangedEvent.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
