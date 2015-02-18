using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Eto.WinForms.Forms
{
	public class SelectFolderDialogHandler : WidgetHandler<CommonOpenFileDialog, SelectFolderDialog>, SelectFolderDialog.IHandler
	{
		public SelectFolderDialogHandler ()
		{
			Control = new CommonOpenFileDialog();

            Control.EnsurePathExists = true;
            Control.EnsureFileExists = true;
            Control.EnsureValidNames = true;
            Control.Multiselect = false;
            Control.AllowNonFileSystemItems = true;
            Control.Title = "Open Folder";
            Control.RestoreDirectory = true;
            Control.ShowPlacesList = true;
            Control.IsFolderPicker = true;
        }

		public DialogResult ShowDialog (Window parent)
		{
            CommonFileDialogResult dr;
            if (parent != null)
                dr = Control.ShowDialog(((SWF.Control)parent.ControlObject).Handle);
            else
                dr = Control.ShowDialog();

            if (dr == CommonFileDialogResult.Ok)
                return DialogResult.Ok;
            else if (dr == CommonFileDialogResult.Cancel)
                return DialogResult.Cancel;
            else
                return DialogResult.None;
        }

		public string Title {
			get {
				return Control.Title;
			}
			set {
				Control.Title = value;
			}
		}

		public string Directory {
			get {
				return Control.FileName;
			}
			set {
				Control.DefaultFileName = value;
			}
		}
}
}

