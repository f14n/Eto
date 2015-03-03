using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Button))]
	public class ButtonSection : Scrollable, INotifyPropertyChanged
	{
		Bitmap smallImage = new Bitmap(TestIcons.TestImage, 16, 16);
		Bitmap largeImage = TestIcons.TestImage;
		ButtonImagePosition imagePosition;

		public ButtonImagePosition ImagePosition
		{
			get { return imagePosition; }
			set
			{
				imagePosition = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ImagePosition"));
			}
		}

		public ButtonSection()
		{
			var layout = new DynamicLayout();

			layout.AddAutoSized(NormalButton(), centered: true);
			layout.AddAutoSized(LongerButton(), centered: true);
			layout.AddAutoSized(DefaultSizeButton(), centered: true);
			layout.AddAutoSized(ColourButton(), centered: true);
			layout.AddAutoSized(DisabledButton(), centered: true);
			layout.Add(StretchedButton());
			layout.AddSeparateRow(null, new Label { Text = "Image Position:", VerticalAlignment = VerticalAlignment.Center }, ImagePositionControl(), null);
			layout.AddSeparateRow(null, TableLayout.AutoSized(ImageButton(smallImage)), TableLayout.AutoSized(ImageTextButton(smallImage)), null);
			layout.AddSeparateRow(null, TableLayout.AutoSized(ImageButton(largeImage)), TableLayout.AutoSized(ImageTextButton(largeImage)), null);

			layout.Add(null);

			Content = layout;
		}

		Control NormalButton()
		{
			var control = new Button { Text = "Click Me" };
			LogEvents(control);
			return control;
		}

		Control StretchedButton()
		{
			var control = new Button { Text = "A stretched button" };
			LogEvents(control);
			return control;
		}

		Control LongerButton()
		{
			var control = new Button { Text = "This is a long(er) button title" };
			LogEvents(control);
			return control;
		}

		Control DefaultSizeButton()
		{
			var control = new Button { Text = "B", Size = new Size(50, 50) };
			LogEvents(control);
			var control2 = new Button { Text = "Button With Text", Size = new Size(-1, 50) };
			LogEvents(control2);

			var layout = new DynamicLayout { Padding = Padding.Empty };
			layout.AddRow(new Label { Text = "With Default Size of 50x50:", VerticalAlignment = VerticalAlignment.Center }, control, control2);
			return layout;
		}

		Control ColourButton()
		{
			var control = new Button { Text = "Button with Color", BackgroundColor = Colors.Lime };
			LogEvents(control);
			return control;
		}

		Control DisabledButton()
		{
			var control = new Button { Text = "Disabled Button", Enabled = false };
			LogEvents(control);
			return control;
		}

		Control ImageButton(Image image)
		{
			var control = new Button { Image = image };
			control.Bind(r => r.ImagePosition, this, r => r.ImagePosition);
			LogEvents(control);
			return control;
		}

		Control ImageTextButton(Image image)
		{
			var control = new Button { Text = "Image && Text Button", Image = image };
			control.Bind(r => r.ImagePosition, this, r => r.ImagePosition);
			LogEvents(control);
			return control;
		}

		Control ImagePositionControl()
		{
			var control = new EnumDropDown<ButtonImagePosition>();
			control.Bind(r => r.SelectedValue, this, r => r.ImagePosition);
			return control;
		}

		void LogEvents(Button button)
		{
			button.Click += delegate
			{
				Log.Write(button, "Click");
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}
}

