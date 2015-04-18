using System;
using Eto.Forms;
using System.Threading.Tasks;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms
{
	public class DialogHandler : MacWindow<MyWindow, Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		Button button;
		ModalEventArgs session;

		protected override bool DisposeControl { get { return false; } }

		class DialogWindow : MyWindow
		{
			public new DialogHandler Handler
			{
				get { return base.Handler as DialogHandler; }
				set { base.Handler = value; }
			}

			public DialogWindow()
				: base(new CGRect(0, 0, 200, 200), NSWindowStyle.Closable | NSWindowStyle.Titled, NSBackingStore.Buffered, false)
			{
			}

			[Export("cancelOperation:")]
			public void CancelOperation(IntPtr sender)
			{
				if (Handler.AbortButton != null)
				{
					var handler = Handler.AbortButton.Handler as IMacViewHandler;
					if (handler != null)
					{
						var callback = handler.Callback as Button.ICallback;
						if (callback != null)
							callback.OnClick(Handler.AbortButton, EventArgs.Empty);
					}
				}
			}
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public Button AbortButton { get; set; }

		public Button DefaultButton
		{
			get { return button; }
			set
			{
				button = value;
				
				if (button != null)
				{
					var b = button.ControlObject as NSButton;
					Control.DefaultButtonCell = b == null ? null : b.Cell;
				}
				else
					Control.DefaultButtonCell = null;
			}
		}

		public DialogHandler()
		{
			var dlg = new DialogWindow();
			dlg.Handler = this;
			Control = dlg;
			ConfigureWindow();
		}

		public virtual void ShowModal(Control parent)
		{
			session = null;
			if (parent != null && parent.ParentWindow != null)
			{
				var nswindow = parent.ParentWindow.ControlObject as NSWindow;
				if (nswindow != null)
					Control.ParentWindow = nswindow;
			}
			Callback.OnShown(Widget, EventArgs.Empty);

			Widget.Closed += HandleClosed;
			if (DisplayMode.HasFlag(DialogDisplayMode.Attached))
				MacModal.RunSheet(Widget, Control, out session);
			else
			{
				Control.MakeKeyWindow();
				MacModal.Run(Widget, Control, out session);
			}
		}

		public virtual Task ShowModalAsync(Control parent)
		{
			var tcs = new TaskCompletionSource<bool>();
			session = null;
			if (parent != null && parent.ParentWindow != null)
			{
				var nswindow = parent.ParentWindow.ControlObject as NSWindow;
				if (nswindow != null)
					Control.ParentWindow = nswindow;
			}
			Callback.OnShown(Widget, EventArgs.Empty);

			Widget.Closed += HandleClosed;
			if (DisplayMode.HasFlag(DialogDisplayMode.Attached))
			{
				MacModal.BeginSheet(Widget, Control, out session, () => tcs.SetResult(true));
			}
			else
			{
				Control.MakeKeyWindow();
				Application.Instance.AsyncInvoke(() =>
				{
					MacModal.Run(Widget, Control, out session);
					tcs.SetResult(true);
				});

			}
			return tcs.Task;
		}

		void HandleClosed(object sender, EventArgs e)
		{
			if (session != null)
				session.Stop();
			Widget.Closed -= HandleClosed;
		}

		public override void Close()
		{
			if (session != null && session.IsSheet)
				session.Stop();
			else
				base.Close();
		}
		
	}
}
