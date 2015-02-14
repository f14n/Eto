using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Eto.WinForms.Forms
{
	public class SaveFileDialogHandler : WindowsFileDialog<CommonFileDialog, SaveFileDialog>, SaveFileDialog.IHandler
	{

		public SaveFileDialogHandler()
		{
			Control = new CommonSaveFileDialog();
		}
	}
}
