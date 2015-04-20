using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class SplitterHandler : WindowsControl<swf.SplitContainer, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		Control panel1;
		Control panel2;
		int? position;
		double relative = double.NaN;
		int suppressSplitterMoved;

		public bool RecurseToChildren { get { return true; } }

		public override Size? DefaultSize
		{
			get
			{
				var size = base.DefaultSize;
				if (size == null && Control.AutoSize)
				{
					var min = Control.MinimumSize;
					Control.MinimumSize = sd.Size.Empty;
					size = Control.GetPreferredSize(sd.Size.Empty).ToEto();
					Control.MinimumSize = min;
				}
				return size;
			}
		}

		public class EtoSplitContainer : swf.SplitContainer
		{
			public SplitterHandler Handler { get; set; }

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				return Handler._prefer();
			}

			// corrections (when too small, or not fixed)
			protected override void OnLayout(swf.LayoutEventArgs e)
			{
				var it = Handler;
				if (it == null || double.IsNaN(it.relative))
				{
					base.OnLayout(e);
					return;
				}
				it.suppressSplitterMoved++;
				base.OnLayout(e);
				it.SetRelative(it.relative);
				it.suppressSplitterMoved--;
			}

		}

		private sd.Size _prefer()
		{
			var size = new sd.Size();
			var size1 = panel1 == null ? Size.Empty : panel1.GetPreferredSize();
			var size2 = panel2 == null ? Size.Empty : panel2.GetPreferredSize();
			if (Orientation == SplitterOrientation.Horizontal)
			{
				if (position.HasValue)
					size1.Width = position.Value;
				else if (!double.IsNaN(relative))
				{
					if (fixedPanel == SplitterFixedPanel.Panel1)
						size1.Width = (int)Math.Round(relative);
					else if (fixedPanel == SplitterFixedPanel.Panel2)
						size2.Width = (int)Math.Round(relative);
					else if (relative <= 0.0)
						size1.Width = 0;
					else if (relative >= 1.0)
						size2.Width = 0;
					else
					{
						// both get at least the preferred size
						size1.Width = (int)Math.Round(Math.Max(
							size1.Width/relative, size2.Width/(1-relative)));
						size2.Width = 0;
					}
				}
				size.Width = size1.Width + size2.Width + SplitterWidth;
				size.Height = Math.Max(size1.Height, size2.Height);
			}
			else
			{
				if (position.HasValue)
					size1.Height = position.Value;
				else if (!double.IsNaN(relative))
				{
					if (fixedPanel == SplitterFixedPanel.Panel1)
						size1.Height = (int)Math.Round(relative);
					else if (fixedPanel == SplitterFixedPanel.Panel2)
						size2.Height = (int)Math.Round(relative);
					else if (relative <= 0.0)
						size1.Height = 0;
					else if (relative >= 1.0)
						size2.Height = 0;
					else
					{
						// both get at least the preferred size
						size1.Height = (int)Math.Round(Math.Max(
							size1.Height/relative, size2.Height/(1-relative)));
						size2.Height = 0;
					}
				}
				size.Height = size1.Height + size2.Height + SplitterWidth;
				size.Width = Math.Max(size1.Width, size2.Width);
			}
			return size;
		}

		public SplitterHandler()
		{
			Control = new EtoSplitContainer {
				Handler = this,
				AutoSize = true,
				FixedPanel = swf.FixedPanel.Panel1,
				Panel1MinSize = 0,
				Panel2MinSize = 0,
			};
			Control.HandleCreated += (sender, e) =>
			{
				SetInitialPosition();
				HookEvents();
				SetFixedPanel();
			};
		}

		void HookEvents()
		{
			Control.SplitterMoved += (sender, e) =>
			{
				if (Widget.ParentWindow == null || !Widget.Loaded || suppressSplitterMoved > 0)
					return;
				// keep track of the desired position (for removing/re-adding/resizing the control)
				UpdateRelative();
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			};
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Splitter.PositionChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public int Position
		{
			get { return Control.SplitterDistance; }
			set
			{
				if (value != position)
				{
					position = value;
					relative = double.NaN;
					suppressSplitterMoved++;
					if (Control.IsHandleCreated)
						SetPosition(value);
					suppressSplitterMoved--;
					Callback.OnPositionChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public int SplitterWidth
		{
			get { return Control.SplitterWidth; }
			set { Control.SplitterWidth = value; }
		}

		int GetAvailableSize(bool desired = false)
		{
			if (desired)
			{
				var size = UserDesiredSize;
				var pick = Orientation == SplitterOrientation.Horizontal ?
					size.Width : size.Height;
				if (pick >= 0)
					return pick - Control.SplitterWidth;
			}
			return (Orientation == SplitterOrientation.Horizontal ?
				Control.Width : Control.Height) - Control.SplitterWidth;
		}

		void UpdateRelative()
		{
			var pos = Position;
			if (fixedPanel == SplitterFixedPanel.Panel1)
				relative = pos;
			else
			{
				var sz = GetAvailableSize();
				if (fixedPanel == SplitterFixedPanel.Panel2)
					relative = sz <= 0 ? 0 : sz - pos;
				else
					relative = sz <= 0 ? 0.5 : pos / (double)sz;
			}
		}

		public double RelativePosition
		{
			get
			{
				if (double.IsNaN(relative))
					UpdateRelative();
				return relative;

			}
			set
			{
				if (relative == value)
					return;
				relative = value;
				position = null;
				suppressSplitterMoved++;
				if (Control.IsHandleCreated)
					SetRelative(value);
				suppressSplitterMoved--;
				Callback.OnPositionChanged(Widget, EventArgs.Empty);
			}
		}

		void SetPosition(int newPosition)
		{
			position = null;
			var size = GetAvailableSize();
			relative = fixedPanel == SplitterFixedPanel.Panel1 ? Math.Max(0, newPosition)
				: fixedPanel == SplitterFixedPanel.Panel2 ? Math.Max(0, size - newPosition)
				: size <= 0 ? 0.5 : Math.Max(0.0, Math.Min(1.0, newPosition / (double)size));
			if (size > 0)
				Control.SplitterDistance = Math.Max(0, Math.Min(size, newPosition));
		}
		void SetRelative(double newRelative)
		{
			position = null;
			relative = newRelative;
			var size = GetAvailableSize();
			if (size <= 0)
				return;
			switch (fixedPanel)
			{
				case SplitterFixedPanel.Panel1:
					Control.SplitterDistance = Math.Max(0, Math.Min(size, (int)Math.Round(relative)));
					break;
				case SplitterFixedPanel.Panel2:
					Control.SplitterDistance = Math.Max(0, Math.Min(size, size - (int)Math.Round(relative)));
					break;
				case SplitterFixedPanel.None:
					Control.SplitterDistance = Math.Max(0, Math.Min(size, (int)Math.Round(size * relative)));
					break;
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			suppressSplitterMoved++;
			if (Control.IsHandleCreated)
				SetInitialPosition();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			suppressSplitterMoved--;
		}

		void SetInitialPosition()
		{
			suppressSplitterMoved++;
			try
			{
				Control.Panel1Collapsed = panel1 == null || !(panel1.GetWindowsHandler()).InternalVisible;
				Control.Panel2Collapsed = panel2 == null || !(panel2.GetWindowsHandler()).InternalVisible;
				if (position != null)
				{
					var pos = position.Value;
					if (fixedPanel != SplitterFixedPanel.Panel1)
					{
						var size = GetAvailableSize(false);
						var want = GetAvailableSize(true);
						if (size != want)
						{
							if (FixedPanel == SplitterFixedPanel.Panel2)
								pos += size - want;
							else
							{
								SetRelative(pos / (double)want);
								return;
							}
						}

					}
					SetPosition(pos);
				}
				else if (!double.IsNaN(relative))
				{
					SetRelative(relative);
				}
				else if (fixedPanel == SplitterFixedPanel.Panel1)
				{
					var size1 = panel1.GetPreferredSize();
					SetRelative(Control.Orientation == swf.Orientation.Vertical ? size1.Width : size1.Height);
				}
				else if (fixedPanel == SplitterFixedPanel.Panel2)
				{
					var size2 = panel2.GetPreferredSize();
					SetRelative(Control.Orientation == swf.Orientation.Vertical ? size2.Width : size2.Height);
				}
				else
				{
					var size1 = panel1.GetPreferredSize();
					var size2 = panel2.GetPreferredSize();
					SetRelative(Control.Orientation == swf.Orientation.Vertical
						? size1.Width / (double)(size1.Width + size2.Width)
						: size1.Height / (double)(size1.Height + size2.Height));
				}
			}
			finally
			{
				suppressSplitterMoved--;
			}
		}

		SplitterFixedPanel fixedPanel;
		public SplitterFixedPanel FixedPanel
		{
			get { return fixedPanel; }
			set
			{
				fixedPanel = value;
				if (Control.IsHandleCreated)
					SetFixedPanel();
			}
		}

		void SetFixedPanel()
		{
			switch (fixedPanel)
			{
				case SplitterFixedPanel.None:
					Control.FixedPanel = swf.FixedPanel.None;
					break;
				case SplitterFixedPanel.Panel1:
					Control.FixedPanel = swf.FixedPanel.Panel1;
					break;
				case SplitterFixedPanel.Panel2:
					Control.FixedPanel = swf.FixedPanel.Panel2;
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public SplitterOrientation Orientation
		{
			get
			{
				switch (Control.Orientation)
				{
					case swf.Orientation.Horizontal:
						return SplitterOrientation.Vertical;
					case swf.Orientation.Vertical:
						return SplitterOrientation.Horizontal;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case SplitterOrientation.Horizontal:
						Control.Orientation = swf.Orientation.Vertical;
						break;
					case SplitterOrientation.Vertical:
						Control.Orientation = swf.Orientation.Horizontal;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				if (panel1 != value)
				{
					if (Widget.Loaded)
						Control.SuspendLayout();
					var old = panel1.GetContainerControl();
					if (old != null)
					{
						old.VisibleChanged -= c1_VisibleChanged;
					}
					panel1 = value;
					Control.Panel1.Controls.Clear();
					if (panel1 != null)
					{
						var controlHandler = panel1.GetWindowsHandler();
						var control = controlHandler.ContainerControl;
						control.Dock = swf.DockStyle.Fill;
						control.VisibleChanged += c1_VisibleChanged;
						controlHandler.BeforeAddControl(Widget.Loaded);
						Control.Panel1.Controls.Add(control);
					}
					if (Widget.Loaded)
					{
						Control.Panel1Collapsed = panel1 == null || !(panel1.GetWindowsHandler()).InternalVisible;
						Control.ResumeLayout();
					}
				}
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set
			{
				if (panel2 != value)
				{
					if (Widget.Loaded)
						Control.SuspendLayout();
					var old = panel2.GetContainerControl();
					if (old != null)
					{
						old.VisibleChanged -= c2_VisibleChanged;
					}
					panel2 = value;
					Control.Panel2.Controls.Clear();
					if (panel2 != null)
					{
						var controlHandler = panel2.GetWindowsHandler();
						var control = controlHandler.ContainerControl;
						control.Dock = swf.DockStyle.Fill;
						control.VisibleChanged += c2_VisibleChanged;
						controlHandler.BeforeAddControl(Widget.Loaded);
						Control.Panel2.Controls.Add(control);
					}
					if (Widget.Loaded)
					{
						Control.Panel2Collapsed = panel2 == null || !(panel2.GetWindowsHandler()).InternalVisible;
						Control.ResumeLayout();
					}
				}
			}
		}

		void c1_VisibleChanged(object sender, EventArgs e)
		{
			if (panel1 != null && (panel1.GetWindowsHandler()).InternalVisible)
				Control.Panel1Collapsed = false;
			else
				Control.Panel1Collapsed = true;
		}

		void c2_VisibleChanged(object sender, EventArgs e)
		{
			if (panel2 != null && (panel2.GetWindowsHandler()).InternalVisible)
				Control.Panel2Collapsed = false;
			else
				Control.Panel2Collapsed = true;
		}
	}
}
