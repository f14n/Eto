using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class RadioMenuItemHandler : MenuHandler<Gtk.RadioMenuItem, RadioMenuItem>, IRadioMenuItem
	{
		string tooltip;
		string text = string.Empty;
		Key shortcut = Key.None;
		Gtk.AccelLabel label;
		Gtk.Label accelLabel;
		
		public void Create(RadioMenuItem controller)
		{
			if (controller != null) Control = new Gtk.RadioMenuItem((Gtk.RadioMenuItem)controller.ControlObject);
			else
			{
				Control = new Gtk.RadioMenuItem(string.Empty);
				foreach (Gtk.Widget w in Control.Children)
				{
					Control.Remove(w);
				}
			}
			Control.Toggled += control_Activated;

			Gtk.HBox hbox = new Gtk.HBox(false, 4);
			label = new Gtk.AccelLabel(string.Empty);
			label.Xalign = 0;
			label.UseUnderline = true;
			label.AccelWidget = Control;
			hbox.Add(label);
			accelLabel = new Gtk.Label();
			accelLabel.Xalign = 1;
			accelLabel.Visible = false;
			hbox.Add(accelLabel);
			Control.Add(hbox);
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				label.TextWithMnemonic = text;
			}
		}
		
		public string ToolTip
		{
			get { return tooltip; }
			set
			{
				tooltip = value;
				//label.TooltipText = value;
			}
		}
		

		public Key Shortcut
		{
			get { return shortcut; }
			set
			{
				shortcut = value;
				accelLabel.Text = KeyMap.KeyToString(value);
				accelLabel.Visible = accelLabel.Text.Length > 0;
			}
		}

		public bool Checked
		{
			get { return Control.Active; }
			set { Control.Active = value; }
		}

		public bool Enabled
		{
			get { return Control.Sensitive; }
			set { Control.Sensitive = value; }
		}

		public override void AddMenu(int index, MenuItem item)
		{
			if (Control.Submenu == null) Control.Submenu = new Gtk.Menu();
			((Gtk.Menu)Control.Submenu).Insert((Gtk.Widget)item.ControlObject, index);
		}

		public override void RemoveMenu(MenuItem item)
		{
			if (Control.Submenu == null) return;
			Gtk.Menu menu = (Gtk.Menu)Control.Submenu;
			menu.Remove((Gtk.Widget)item.ControlObject);
			if (menu.Children.Length == 0)
			{
				Control.Submenu = null;
			}
		}

		public override void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}
		private void control_Activated(object sender, EventArgs e)
		{
			Widget.OnClick(e);
		}
	}
}