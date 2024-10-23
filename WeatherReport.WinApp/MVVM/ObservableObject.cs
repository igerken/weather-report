using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace WeatherReport.MVVM
{
	/// <summary>
    /// This is the abstract base class for any object that provides property change notifications.  
    /// </summary>
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		private bool _isNotificationActive = true;
		private HashSet<string> _changedPropertyNames = new HashSet<string>();

		public event PropertyChangedEventHandler PropertyChanged;

		#region Constructor

		protected ObservableObject()
		{
		}

		#endregion // Constructor

		#region RaisePropertyChanged

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		protected void RaisePropertyChanged(string propertyName)
		{
			this.VerifyPropertyName(propertyName);

			if (_isNotificationActive)
			{
				PropertyChangedEventHandler handler = this.PropertyChanged;
				if (handler != null)
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
			else
				_changedPropertyNames.Add(propertyName);
		}

		#endregion // RaisePropertyChanged

		#region Debugging Aides

		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This 
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// If you raise PropertyChanged and do not specify a property name,
			// all properties on the object are considered to be changed by the binding system.
			if (String.IsNullOrEmpty(propertyName))
				return;

			// Verify that the property name matches a real,  
			// public, instance property on this object.
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string msg = "Invalid property name: " + propertyName;

				if (this.ThrowOnInvalidPropertyName)
					throw new ArgumentException(msg);
				else
					Debug.Fail(msg);
			}
		}

		/// <summary>
		/// Returns whether an exception is thrown, or if a Debug.Fail() is used
		/// when an invalid property name is passed to the VerifyPropertyName method.
		/// The default value is false, but subclasses used by unit tests might 
		/// override this property's getter to return true.
		/// </summary>
		protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

		#endregion // Debugging Aides


		public void SuspendNotifications()
		{
			_isNotificationActive = false;
		}

		public void ResumeNotifications()
		{
			_isNotificationActive = true;
			foreach (string propertyName in _changedPropertyNames)
				RaisePropertyChanged(propertyName);
			_changedPropertyNames.Clear();
		}



	}
}
