using System;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface ITableLayout : IPositionalLayout
	{
		void CreateControl(int cols, int rows);
		void SetColumnScale(int column, bool scale);
		void SetRowScale(int row, bool scale);
		Size Spacing { get; set; }
		Padding Padding { get; set; }
	}
	
	public class TableLayout : Layout
	{
		ITableLayout inner;
		Control[,] controls;
		
		public static Size DefaultSpacing = new Size(5, 5);
		public static Padding DefaultPadding = new Padding(5);
		
		public override IEnumerable<Control> Controls {
			get {
				return controls.OfType<Control>();
			}
		}
		
		public TableLayout(Container container, Size size)
			: this(container, size.Width, size.Height)
		{
		}

		public TableLayout(Container container, int cols, int rows)
			: base(container.Generator, container, typeof(ITableLayout), false)
		{
			inner = (ITableLayout)Handler;
			controls = new Control[cols, rows];
			inner.CreateControl(cols, rows);
			Initialize();
			this.Container.SetLayout(this);
		}

		public void SetColumnScale(int column, bool scale = true)
		{
			inner.SetColumnScale(column, scale);
		}
		
		public void SetRowScale(int row, bool scale = true)
		{
			inner.SetRowScale(row, scale);
		}
		
		public void Add(Control control, int x, int y)
		{
			//var old = controls[x, y];
			controls[x, y] = control;
			control.SetParentLayout(this);
			inner.Add(control, x, y);
			if (Loaded) {
				control.OnLoad (EventArgs.Empty);
				control.OnLoadComplete (EventArgs.Empty);
			}
		}

		public void Add(Control child, int x, int y, bool xscale, bool yscale)
		{
			SetColumnScale(x, xscale);
			SetRowScale(y, yscale);
			Add(child, x, y);
		}
		
		public void Add(Control child, Point p)
		{
			Add(child, p.X, p.Y);
		}

		public void Move(Control child, int x, int y)
		{
			inner.Move(child, x, y);
		}
		
		public void Move(Control child, Point p)
		{
			Move(child, p.X, p.Y);
		}
		
		public Size Spacing
		{
			get { return inner.Spacing; }
			set { inner.Spacing = value; }
		}
		
		public Padding Padding
		{
			get { return inner.Padding; }
			set { inner.Padding = value; }
		}
	}
}