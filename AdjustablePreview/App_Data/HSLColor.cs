using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ElfCore
{
	/// <summary>
	/// https://rootsocket.svn.codeplex.com/svn/RootSocket/Controls/HSLColor.cs
	/// http://richnewman.wordpress.com/about/code-listings-and-diagrams/hslcolor-class/
	/// </summary>
    public class HSLColor
	{
		#region [ Constants ]

		private const double SCALE = 240.0;
		
		#endregion [ Constants ]

		#region [ Private Variables ]

		// Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private double _hue = 1.0;
        private double _saturation = 1.0;
        private double _luminosity = 1.0;

		#endregion [ Private Variables ]

		#region [ Properties ]

		public double Hue
        {
            get { return _hue * SCALE; }
            set { _hue = CheckRange(value / SCALE); }
        }

        public double Saturation
        {
            get { return _saturation * SCALE; }
            set { _saturation = CheckRange(value / SCALE); }
        }

        public double Luminosity
        {
            get { return _luminosity * SCALE; }
            set { _luminosity = CheckRange(value / SCALE); }
        }

        public int Alpha { get; set; }

		#endregion [ Properties ]
		
		#region [ Constructors ]
		
		public HSLColor() { }
        
		public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }
        
		public HSLColor(int red, int green, int blue)
        {
            SetRGB(red, green, blue);
        }
        
		public HSLColor(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		private double CheckRange(double value)
		{
			if (value < 0.0)
				value = 0.0;
			else if (value > 1.0)
				value = 1.0;
			return value;
		}
				
		public void SetRGB(int red, int green, int blue)
		{
			HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
			this._hue = hslColor._hue;
			this._saturation = hslColor._saturation;
			this._luminosity = hslColor._luminosity;
			this.Alpha = hslColor.Alpha;
		}

		public string ToRGBString()
		{
			Color color = (Color)this;
			return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
		}

		public override string ToString()
		{
			return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
		}

		#region [ Conversion ]

		public static implicit operator Color(HSLColor hslColor)
		{
			double r = 0, g = 0, b = 0;
			if (hslColor._luminosity != 0)
			{
				if (hslColor._saturation == 0)
					r = g = b = hslColor._luminosity;
				else
				{
					double temp2 = GetTemp2(hslColor);
					double temp1 = 2.0 * hslColor._luminosity - temp2;

					r = GetColorComponent(temp1, temp2, hslColor._hue + 1.0 / 3.0);
					g = GetColorComponent(temp1, temp2, hslColor._hue);
					b = GetColorComponent(temp1, temp2, hslColor._hue - 1.0 / 3.0);
				}
			}
			return Color.FromArgb(hslColor.Alpha, (int)(255 * r), (int)(255 * g), (int)(255 * b));
		}

		public static implicit operator HSLColor(Color color)
		{
			HSLColor hslColor = new HSLColor();
			hslColor._hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
			hslColor._luminosity = color.GetBrightness();
			hslColor._saturation = color.GetSaturation();
			hslColor.Alpha = color.A;
			return hslColor;
		}

		private static double GetColorComponent(double temp1, double temp2, double temp3)
		{
			temp3 = MoveIntoRange(temp3);
			if (temp3 < 1.0 / 6.0)
				return temp1 + (temp2 - temp1) * 6.0 * temp3;
			else if (temp3 < 0.5)
				return temp2;
			else if (temp3 < 2.0 / 3.0)
				return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
			else
				return temp1;
		}

		private static double MoveIntoRange(double temp3)
		{
			if (temp3 < 0.0)
				temp3 += 1.0;
			else if (temp3 > 1.0)
				temp3 -= 1.0;
			return temp3;
		}

		private static double GetTemp2(HSLColor hslColor)
		{
			double temp2;
			if (hslColor._luminosity < 0.5)  //<=??
				temp2 = hslColor._luminosity * (1.0 + hslColor._saturation);
			else
				temp2 = hslColor._luminosity + hslColor._saturation - (hslColor._luminosity * hslColor._saturation);
			return temp2;
		}

		#endregion [ Conversion ]

		
		#endregion [ Methods ]
	}
}