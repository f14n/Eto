using System;
using Eto.Drawing;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;

namespace Eto.Wpf.Forms.Controls
{
	public class NumericUpDownHandler : WpfControl<mwc.DoubleUpDown, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		protected override Size DefaultSize { get { return new Size(80, -1); } }

		protected override bool PreventUserResize { get { return true; } }

		public NumericUpDownHandler()
		{
			Control = new mwc.DoubleUpDown();
			Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
			DecimalPlaces = 0;
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool ReadOnly
		{
			get { return !Control.IsEnabled; }
			set { Control.IsEnabled = !value; }
		}

		public double Value
		{
			get { return Control.Value ?? 0; }
			set { Control.Value = value; }
		}

		public double MinValue
		{
			get { return Control.Minimum ?? double.MinValue; }
			set { Control.Minimum = value; }
		}

		public double MaxValue
		{
			get { return Control.Maximum ?? double.MaxValue; }
			set { Control.Maximum = value; }
		}

		int decimalPlaces = 0;

		public int DecimalPlaces
		{
			get { return decimalPlaces; }
			set
			{
				if (value != decimalPlaces)
				{
					decimalPlaces = value;
					Control.FormatString = "0." + new string('0', decimalPlaces);
				}
			}
		}

		public double Increment
		{
			get { return Control.Increment ?? 1; }
			set { Control.Increment = value; }
		}
	}
}