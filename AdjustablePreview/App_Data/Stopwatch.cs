using System;
using System.Runtime.InteropServices; //for API call

namespace ElfCore
{
	/// <summary>
	/// Provides a set of methods and properties that you can use to accurately measure elapsed time.
	/// Ported from .Net Framework 2.0 to .Net Framework 1.1
	/// </summary>
	public class Stopwatch
	{

		#region [ SafeNativeMethods ]

		/// <summary>
		/// API Call to Kernel32 DLL
		/// </summary>
		/// <param name="lpFrequency"></param>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(out long lpFrequency);

		/// <summary>
		/// API Call to Kernel32 DLL
		/// </summary>
		/// <param name="lpPerformanceCount"></param>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		#endregion  [ SafeNativeMethods ]
		
		#region [ Public Fields ]

		/// <summary>
		/// Returns the frequency of the timer as the number of ticks per second. This field is read-only.
		/// </summary>
		public static readonly long Frequency;

		/// <summary>
		/// Indicates whether the timer is based on a high-resolution performance counter. This field is read-only.
		/// </summary>
		public static readonly bool IsHighResolution;

		#endregion [ Public Fields ]

		#region [ Private Variables ]

		private long _elapsed;
		private bool _isRunning;
		private long _startTimeStamp;
		private static readonly double _tickFrequency;

		#endregion [ Private Variables ]

		#region [ Properties ]

		/// <summary>
		/// Gets the total elapsed time measured by the current instance.
		/// </summary>
		/// <value>
		/// A read-only System.TimeSpan representing the total elapsed time measured by the current instance.
		/// </value>
		public TimeSpan Elapsed
		{
			get { return new TimeSpan(GetElapsedDateTimeTicks()); }
		}

		/// <summary>
		/// Gets the total elapsed time measured by the current instance, in milliseconds.
		/// </summary>
		public long ElapsedMilliseconds
		{
			get { return (GetElapsedDateTimeTicks() / TimeSpan.TicksPerMillisecond); }
		}

		/// <summary>
		/// Gets the total elapsed time measured by the current instance, in timer ticks.
		/// </summary>
		public long ElapsedTicks
		{
			get { return GetRawElapsedTicks(); }
		}

		/// <summary>
		/// Gets a value indicating whether the Stopwatch timer is running.
		/// </summary>
		public bool IsRunning
		{
			get { return _isRunning; }
		}

		#endregion [ Properties ]
		
		#region [ Constructors ]

		/// <summary>
		/// static instance of the StopWatch
		/// </summary>
		static Stopwatch()
		{
			if (!QueryPerformanceFrequency(out Stopwatch.Frequency))
			{
				Stopwatch.IsHighResolution = false;
				Stopwatch.Frequency = TimeSpan.TicksPerSecond;
				Stopwatch._tickFrequency = 1;
			}
			else
			{
				Stopwatch.IsHighResolution = true;
				Stopwatch._tickFrequency = TimeSpan.TicksPerSecond;
				Stopwatch._tickFrequency /= ((double)Stopwatch.Frequency);
			}
		}

		/// <summary>
		/// Initializes a new instance of the Stopwatch class.
		/// </summary>
		public Stopwatch()
		{
			Reset();
		}

		#endregion  [ Constructors ]

		#region [ Private Methods ]

		/// <summary>
		/// Returns the elapsed time of the StopWatch
		/// </summary>
		/// <returns>Number of Ticks</returns>
		private long GetElapsedDateTimeTicks()
		{
			long ticks = GetRawElapsedTicks();
			if (Stopwatch.IsHighResolution)
			{
				double highTicks = ticks;
				highTicks *= Stopwatch._tickFrequency;
				return (long)highTicks;
			}
			return ticks;
		}

		/// <summary>
		/// Returns the raw Elapsed Time Ticks
		/// </summary>
		/// <returns></returns>
		private long GetRawElapsedTicks()
		{
			long elapsedTimestamp = _elapsed;
			if (_isRunning)
			{
				long currentTimestamp = Stopwatch.GetTimestamp();
				long endTimestamp = currentTimestamp - _startTimeStamp;
				elapsedTimestamp += endTimestamp;
			}
			return elapsedTimestamp;
		}

		#endregion [ Private Methods ]

		#region [ Public Methods ]

		/// <summary>
		/// Gets the current number of ticks in the timer mechanism.
		/// </summary>
		/// <returns>
		/// A long integer representing the tick counter value of the underlying timer mechanism.
		/// </returns>
		public static long GetTimestamp()
		{
			if (Stopwatch.IsHighResolution)
			{
				long ticks = 0;
				QueryPerformanceCounter(out ticks);
				return ticks;
			}
			return DateTime.UtcNow.Ticks;
		}

		/// <summary>
		/// Returns the Elapsed Ticks and Milliseconds
		/// </summary>
		public string Report()
		{
			return this.ElapsedTicks + " ticks, " + this.ElapsedMilliseconds + " ms";
		}

		/// <summary>
		/// Stops time interval measurement and resets the elapsed time to zero.
		/// </summary>
		public void Reset()
		{
			_elapsed = 0;
			_isRunning = false;
			_startTimeStamp = 0;
		}

		/// <summary>
		/// Starts, or resumes, measuring elapsed time for an interval.
		/// </summary>
		public void Start()
		{
			if (!_isRunning)
			{
				_startTimeStamp = Stopwatch.GetTimestamp();
				_isRunning = true;
			}
		}

		/// <summary>
		/// Initializes a new Stopwatch instance, sets the elapsed time property to zero, 
		/// and starts measuring elapsed time.
		/// </summary>
		/// <returns>A Stopwatch that has just begun measuring elapsed time.</returns>
		public static Stopwatch StartNew()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		/// <summary>
		/// Stops measuring elapsed time for an interval.
		/// </summary>
		public void Stop()
		{
			if (_isRunning)
			{
				long currentTimestamp = Stopwatch.GetTimestamp();
				long endTimestamp = currentTimestamp - _startTimeStamp;
				_elapsed += endTimestamp;
				_isRunning = false;
			}
		}
		
		#endregion [ Public Methods ]

	}
}

