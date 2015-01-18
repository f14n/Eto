using System;
using Eto.Drawing;
using System.Linq;
using System.Text;

namespace Eto.GtkSharp.Drawing
{
	public static class FontHandlerExtensions
	{
		public static void Apply(this Font font, Gtk.TextTag tag)
		{
			if (tag != null)
			{
				if (font != null)
				{
					tag.Underline = font.FontDecoration.HasFlag(FontDecoration.Underline) ? Pango.Underline.Single : Pango.Underline.None;
					tag.Strikethrough = font.FontDecoration.HasFlag(FontDecoration.Strikethrough);
				}
				else
				{
					tag.Underline = Pango.Underline.None;
					tag.Strikethrough = false;
				}
			}
		}

		public static void Apply(this Font font, Pango.Layout layout)
		{
			if (font != null)
			{
				var handler = (FontHandler)font.Handler;
				layout.FontDescription = handler.Control;
				layout.Attributes = handler.Attributes;
			}
		}
	}

	public class FontHandler : WidgetHandler<Pango.FontDescription, Font>, Font.IHandler
	{
		FontFamily family;
		FontTypeface typeface;
		FontStyle? style;
		Pango.AttrList attributes;
		string familyName;

		public FontHandler()
		{
		}

		public FontHandler(Gtk.Widget widget)
			: this (widget.Style.FontDescription)
		{
		}

		public FontHandler(Pango.FontDescription fontDescription, string familyName = null, FontDecoration? decorations = null)
		{
			Control = fontDescription;
			this.familyName = familyName;
			if (decorations != null)
				FontDecoration = decorations.Value;
		}

		public FontHandler(string fontName)
		{
			Control = Pango.FontDescription.FromString(fontName);
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			switch (systemFont)
			{
				case SystemFont.Default:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				case SystemFont.Bold:
					Control = new Pango.FontDescription();
					Control.Merge(Gtk.Widget.DefaultStyle.FontDescription, true);
					Control.Weight = Pango.Weight.Bold;
					break;
				case SystemFont.TitleBar:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				case SystemFont.ToolTip:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				case SystemFont.Label:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				case SystemFont.MenuBar:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				case SystemFont.Menu:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				case SystemFont.Message:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				case SystemFont.Palette:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				case SystemFont.StatusBar:
					Control = Gtk.Widget.DefaultStyle.FontDescription;
					break;
				default:
					throw new NotSupportedException();
			}
			if (size != null)
			{
				var old = Control;
				Control = new Pango.FontDescription();
				Control.Merge(old, true);
				if (size != null)
					Control.Size = (int)(size * Pango.Scale.PangoScale);
			}
			FontDecoration = decoration;
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			this.family = family;
			Control = new Pango.FontDescription();
			SetStyle(style);
			Size = size;
			FontDecoration = decoration;

			var familyHandler = (FontFamilyHandler)family.Handler;
			Control.Family = familyHandler.Control.Name;
		}

		public void Create(FontTypeface face, float size, FontDecoration decoration)
		{
			typeface = face;
			Control = ((FontTypefaceHandler)face.Handler).Control.Describe();
			Size = size;
			FontDecoration = decoration;
		}

		void SetStyle(FontStyle style)
		{
			if ((style & FontStyle.Bold) != 0)
				Control.Weight = Pango.Weight.Bold;
			if ((style & FontStyle.Italic) != 0)
				Control.Style = Pango.Style.Italic;
		}

		public Pango.AttrList Attributes
		{
			get
			{
				if (attributes == null)
				{
					attributes = new Pango.AttrList();
					attributes.Insert(new Pango.AttrUnderline(FontDecoration.HasFlag(FontDecoration.Underline) ? Pango.Underline.Single : Pango.Underline.None));
					attributes.Insert(new Pango.AttrStrikethrough(FontDecoration.HasFlag(FontDecoration.Strikethrough)));
				}
				return attributes;
			}
		}

		public float Size
		{
			get { return (float)(Control.Size / Pango.Scale.PangoScale); }
			private set { Control.Size = (int)(value * Pango.Scale.PangoScale); }
		}

		public FontStyle FontStyle
		{
			get
			{
				if (style == null)
				{
					style = FontStyle.None;
					if (Control.Weight == Pango.Weight.Bold || Control.Weight == Pango.Weight.Heavy || Control.Weight == Pango.Weight.Semibold || Control.Weight == Pango.Weight.Ultrabold)
						style |= FontStyle.Bold;
					if (Control.Style == Pango.Style.Italic || Control.Style == Pango.Style.Oblique)
						style |= FontStyle.Italic;
				}
				return style.Value;
			}
		}

		public FontDecoration FontDecoration { get; private set; }

		public string FamilyName
		{
			get { return familyName ?? Family.Name; }
		}

		public FontFamily Family
		{
			get
			{
				if (family == null)
				{
					family = new FontFamily(familyName ?? Control.Family);
				}
				return family;
			}
		}

		public static Pango.FontFace FindFontFace(Pango.FontDescription desc, Pango.FontFamily family = null)
		{
			if (family == null)
			{
				family = FontFamilyHandler.FindCorrectedFamily(desc.Family);
			}
			var weight = desc.Weight;
			var style = desc.Style;
			var stretch = desc.Stretch;

			foreach (var face in family.Faces)
			{
				var faceDesc = face.Describe();
				if (faceDesc.Weight == weight && faceDesc.Style == style && faceDesc.Stretch == stretch)
				{
					return face;
				}
			}
			return null;
		}


		public FontTypeface Typeface
		{
			get
			{
				if (typeface == null)
				{
					var familyHandler = Family.Handler as FontFamilyHandler;
					var face = FindFontFace(Control, familyHandler.Control) ?? familyHandler.Control.Faces.First();
					typeface = new FontTypeface(Family, new FontTypefaceHandler(face));
				}
				return typeface;
			}
		}

		Pango.FontMetrics metrics;

		public Pango.FontMetrics Metrics
		{
			get
			{
				if (metrics == null)
					metrics = FontsHandler.Context.GetMetrics(Control, Pango.Language.Default);
				return metrics;
			}
		}

		public float Ascent
		{
			get { return (float)Metrics.Ascent / (float)Pango.Scale.PangoScale; }
		}

		public float Descent
		{
			get { return (float)Metrics.Descent / (float)Pango.Scale.PangoScale; }
		}

		float? lineHeight;

		public float LineHeight
		{
			get
			{
				if (lineHeight == null)
				{
					using (var layout = new Pango.Layout(FontsHandler.Context))
					{
						layout.FontDescription = Control;
						layout.SetText("X");
						Pango.Rectangle ink, logical;
						layout.GetExtents(out ink, out logical);
						lineHeight = (float)logical.Height / (float)Pango.Scale.PangoScale;
					}
				}
				return lineHeight ?? 0f;
			}
		}

		public float Baseline
		{
			get { return Ascent; }
		}

		float? leading;

		public float Leading
		{
			get
			{
				if (leading == null)
				{
					using (var layout = new Pango.Layout(FontsHandler.Context))
					{
						layout.FontDescription = Control;
						layout.SetText("X");
						Pango.Rectangle ink, logical;
						layout.GetExtents(out ink, out logical);
						leading = (float)(ink.Y - logical.Y) / (float)Pango.Scale.PangoScale;
					}
				}
				return leading ?? 0f;
			}
		}

		float? xheight;

		public float XHeight
		{
			get
			{
				if (xheight == null)
				{
					using (var layout = new Pango.Layout(FontsHandler.Context))
					{
						layout.FontDescription = Control;
						layout.SetText("x");
						layout.Spacing = 0;
						layout.Alignment = Pango.Alignment.Left;
						layout.Width = int.MaxValue;
						Pango.Rectangle ink, logical;
						layout.GetExtents(out ink, out logical);
						xheight = (float)ink.Height / (float)Pango.Scale.PangoScale;
					}
				}
				return xheight ?? 0f;
			}
		}
	}
}
