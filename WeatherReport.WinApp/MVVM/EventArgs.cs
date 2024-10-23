using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherReport.MVVM
{
	public class EventArgs<T> : System.EventArgs
	{
		private T m_data;

		public T Data { get { return m_data; } }

		public EventArgs(T data)
		{
			m_data = data;
		}
	}

    public class EventArgs<T1, T2> : System.EventArgs
    {
        private T1 m_data1;
        private T2 m_data2;

        public T1 Data1 { get { return m_data1; } }
        public T2 Data2 { get { return m_data2; } }

        public EventArgs(T1 data1, T2 data2)
        {
            m_data1 = data1;
            m_data2 = data2;
        }
    }
}
