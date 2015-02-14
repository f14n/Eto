using System;
using System.IO;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Eto.WinForms.Forms
{
	public abstract class WindowsFileDialog<TControl, TWidget> : WidgetHandler<TControl, TWidget>, FileDialog.IHandler
		where TControl : CommonFileDialog
        where TWidget : FileDialog
	{
		public string FileName
		{
			get { return Control.FileName; }
			set
			{
				var dir = Path.GetDirectoryName(value);
				if (!string.IsNullOrEmpty(dir))
					Control.InitialDirectory = dir;
                
				Control.DefaultFileName = Path.GetFileName(value);
			}
		}

		public Uri Directory
		{
			get { return new Uri(Control.InitialDirectory); }
			set { Control.InitialDirectory = value.AbsoluteUri; }
		}

		public void InsertFilter(int index, FileDialogFilter filter)
		{
		}

		public void RemoveFilter(int index)
		{
		}

		public void ClearFilters()
		{
		}

		public void SetFilters()
		{
			var filterValues = from f in Widget.Filters
							   select string.Format("{0}|{1}",
								   f.Name.Replace("|", " "),
								   string.Join(";",
									   from ex in f.Extensions
									   select "*" + ex.Replace(";", " ")
								   )
							   );

            foreach (var f in Widget.Filters)
            {
                Control.Filters.Add(new CommonFileDialogFilter(f.Name, string.Join(";", f.Extensions)));
            }
		}

		public FileDialogFilter CurrentFilter
		{
			get
			{
				if (CurrentFilterIndex == -1) return null;
				return Widget.Filters[CurrentFilterIndex];
			}
			set
			{
				CurrentFilterIndex = Widget.Filters.IndexOf(value);
			}
		}

		public int CurrentFilterIndex
		{
			get { return (Control.SelectedFileTypeIndex > 0) ? Control.SelectedFileTypeIndex - 1 : 0; }
			set { /*no op*/ }
		}

		public bool CheckFileExists
		{
			get { return Control.EnsureFileExists; }
			set { Control.EnsureFileExists = value; }
		}

		public string Title
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		public DialogResult ShowDialog(Window parent)
		{
			SetFilters();

			CommonFileDialogResult dr;
			if (parent != null)
				dr = Control.ShowDialog(((swf.Control)parent.ControlObject).Handle);
            else
				dr = Control.ShowDialog();

            if (dr == CommonFileDialogResult.Ok)
                return DialogResult.Ok;
            else if (dr == CommonFileDialogResult.Cancel)
                return DialogResult.Cancel;
            else
                return DialogResult.None;
		}
	}
}
