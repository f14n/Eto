using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Eto.WinForms.Forms
{
	public class OpenFileDialogHandler : WindowsFileDialog<CommonOpenFileDialog, OpenFileDialog>, OpenFileDialog.IHandler
	{
		public OpenFileDialogHandler()
		{
            Control = new CommonOpenFileDialog();

            Control.EnsurePathExists = true;
            Control.EnsureFileExists = true;
            Control.EnsureValidNames = true;
            Control.AllowNonFileSystemItems = true;
            Control.Title = "Open File";
            Control.RestoreDirectory = true;
            Control.ShowPlacesList = true;
        }
        
        public bool MultiSelect
		{
			get { return Control.Multiselect; }
			set { Control.Multiselect = value; }
		}

		public IEnumerable<string> Filenames
		{
			get { return Control.FileNames; }
		}
	}
}
