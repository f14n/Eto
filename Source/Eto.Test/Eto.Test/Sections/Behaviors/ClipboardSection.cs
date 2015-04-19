﻿using System;
using Eto.Forms;
using System.Text;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Clipboard")]
	public class ClipboardSection : Panel
	{
		Clipboard clipboard = new Clipboard();
		Scrollable pasteData = new Scrollable();

		public ClipboardSection()
		{
			var copyTextButton = new Button { Text = "Copy Text" };
			copyTextButton.Click += (sender, e) =>
			{
				clipboard.Text = "Some text";
				Update();
			};
			var copyImageButton = new Button { Text = "Copy Image" };
			copyImageButton.Click += (sender, e) =>
			{
				clipboard.Image = TestIcons.TestImage;
				Update();
			};

			var pasteTextButton = new Button { Text = "Paste" };
			pasteTextButton.Click += (sender, e) => Update();

			var clearButton = new Button { Text = "Clear" };
			clearButton.Click += (sender, e) =>
			{
				clipboard.Clear();
				Update();
			};

			Content = new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					new StackLayout
					{ 
						Orientation = Orientation.Horizontal, 
						Items = { copyTextButton, copyImageButton, pasteTextButton, clearButton }
					},
					new StackLayoutItem(pasteData, expand: true)
				}
			};
		}

		void Update()
		{
			var panel = new StackLayout();
			if (clipboard.Text != null)
			{
				panel.Items.Add("\nText:");
				panel.Items.Add(clipboard.Text);
			}
			if (clipboard.Image != null)
			{
				panel.Items.Add("\nImage:");
				panel.Items.Add(new ImageView
					{
						Image = clipboard.Image
					});
			}
			if (clipboard.Html != null)
			{
				panel.Items.Add("\nHtml:");
				panel.Items.Add(clipboard.Html);
			}
			var types = clipboard.Types;
			if (types != null)
			{
				foreach (var type in types)
				{
					panel.Items.Add(string.Format("\n{0}:", type));
					var data = clipboard.GetData(type);
					if (data != null)
					{
						panel.Items.Add(string.Format("- Data, Length: {0}", data.Length));
						var hexString = BitConverter.ToString(data);
						panel.Items.Add(hexString.Substring(0, Math.Min(hexString.Length, 1000)));
					}
					var str = clipboard.GetString(type);
					if (str != null)
					{
						panel.Items.Add(string.Format("- String, Length: {0}", str.Length));
						panel.Items.Add(str);
					}
				}
			}
			pasteData.Content = panel;
		}
	}
}

