using System;
using Eto.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Collections;

namespace Eto.Forms
{
	/// <summary>
	/// Represents a cell in a <see cref="TableRow"/>
	/// </summary>
	[ContentProperty("Control")]
	[TypeConverter(typeof(TableCellConverter))]
	public class TableCell
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TableCell"/> will scale its width
		/// </summary>
		/// <remarks>
		/// All controls in the same column of this cell will get the same scaling value.
		/// Scaling will make the column expand to fit the rest of the width of the container, minus the preferred
		/// width of any non-scaled columns.
		/// 
		/// If there are no columns with width scaling, the last column will automatically get scaled.
		/// 
		/// With scaling turned off, cells in the column will fit the preferred size of the widest control.
		/// </remarks>
		/// <value><c>true</c> if scale width; otherwise, <c>false</c>.</value>
		public bool ScaleWidth { get; set; }

		/// <summary>
		/// Gets or sets the control in this cell, or null for an empty space
		/// </summary>
		/// <value>The control.</value>
		public Control Control { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableCell"/> class.
		/// </summary>
		public TableCell()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TableCell"/> class.
		/// </summary>
		/// <param name="control">Control for this cell</param>
		/// <param name="scaleWidth">Scale the width of the control if <c>true</c>, otherwise scale to the preferred size of the control.</param>
		public TableCell(Control control, bool scaleWidth = false)
		{
			Control = control;
			ScaleWidth = scaleWidth;
		}

		/// <summary>
		/// Converts a control to a table cell
		/// </summary>
		/// <param name="control">Control to convert to a cell.</param>
		public static implicit operator TableCell(Control control)
		{
			return new TableCell(control);
		}

		/// <summary>
		/// Converts an array of cells to a new cell with a table of vertical cells in a new child TableLayout
		/// </summary>
		/// <param name="items">Items to convert.</param>
		public static implicit operator TableCell(TableCell[] items)
		{
			return new TableCell(new TableLayout(items));
		}

		/// <summary>
		/// Converts an array of rows to a new cell with vertical rows in a new child TableLayout
		/// </summary>
		/// <param name="rows">Rows to convert.</param>
		public static implicit operator TableCell(TableRow[] rows)
		{
			return new TableCell(new TableLayout(rows));
		}

		/// <summary>
		/// Converts a string to a TableCell with a label control implicitly.
		/// </summary>
		/// <remarks>
		/// This provides an easy way to add labels to your layout through code, without having to create <see cref="Label"/> instances.
		/// </remarks>
		/// <param name="labelText">Text to convert to a Label control.</param>
		public static implicit operator TableCell(string labelText)
		{
			return new TableCell(new Label { Text = labelText });
		}
	}

	class TableCellCollection : Collection<TableCell>, IList
	{
		public TableCellCollection()
		{
		}

		public TableCellCollection(IEnumerable<TableCell> list)
			: base(list.Select(r => r ?? new TableCell { ScaleWidth = true }).ToList())
		{
		}

		protected override void InsertItem(int index, TableCell item)
		{
			if (item == null)
				item = new TableCell { ScaleWidth = true };
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, TableCell item)
		{
			if (item == null)
				item = new TableCell { ScaleWidth = true };
			base.SetItem(index, item);
		}

		int IList.Add(object value)
		{
			// allow adding a control directly from xaml
			var control = value as Control;
			if (control != null)
				Add((TableCell)control);
			else
				Add((TableCell)value);
			return Count - 1;
		}
	}
}
