using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;

#if XAMMAC2
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#elif OSX
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

#if IOS
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using Eto.iOS;

using NSResponder = UIKit.UIResponder;
using NSView = UIKit.UIView;
using Eto.iOS.Forms;
#endif

namespace Eto.Mac.Forms
{
	public interface IMacContainer : IMacControlHandler
	{
		void SetContentSize(CGSize contentSize);

		void LayoutParent(bool updateSize = true);

		void LayoutChildren();

		void LayoutAllChildren();

		bool InitialLayout { get; }
	}

	public abstract class MacContainer<TControl, TWidget, TCallback> : 
		MacView<TControl, TWidget, TCallback>,
		Container.IHandler, IMacContainer
		where TControl: NSObject
		where TWidget: Container
		where TCallback: Container.ICallback
	{
		public bool RecurseToChildren { get { return true; } }

		public virtual Size ClientSize { get { return Size; } set { Size = value; } }

		public override bool Enabled { get; set; }

		public bool InitialLayout { get; private set; }

		protected override void Initialize()
		{
			base.Initialize();
			Enabled = true;
		}

		public virtual void Update()
		{
			LayoutChildren();	
		}

		public bool NeedsQueue(Action update = null)
		{
			#if OSX
			if (ApplicationHandler.QueueResizing)
			{
				ApplicationHandler.Instance.AsyncInvoke(update ?? Update);
				return true;
			}
			#endif
			return false;
		}

		public virtual void SetContentSize(CGSize contentSize)
		{
		}

		public virtual void LayoutChildren()
		{
		}

		public void LayoutAllChildren()
		{
			//Console.WriteLine("Layout all children: {0}\n {1}", this.GetType().Name, new StackTrace());
			LayoutChildren();
			foreach (var child in Widget.Controls.Select (r => r.GetMacContainer()).Where(r => r != null))
			{
				child.LayoutAllChildren();
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			var parent = Widget.Parent.GetMacContainer();
			if (parent == null || parent.InitialLayout)
			{
				InitialLayout = true;
				LayoutAllChildren();
			}
		}

		public void LayoutParent(bool updateSize = true)
		{
			if (NeedsQueue(() => LayoutParent(updateSize)))
				return;
			var container = Widget.Parent.GetMacContainer();
			if (container != null)
			{
				// traverse up the tree to update everything we own
				container.LayoutParent(updateSize);
				return;
			} 
			if (updateSize && !Widget.Loaded && AutoSize)
			{
				var size = GetPreferredSize(Size.MaxValue);
				SetContentSize(size.ToNS());
			}

			// layout everything!
			LayoutAllChildren();
		}
	}
}
