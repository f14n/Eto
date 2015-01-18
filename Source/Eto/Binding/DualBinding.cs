using System;

namespace Eto
{
	/// <summary>
	/// Mode of the <see cref="DualBinding{T}"/>
	/// </summary>
	/// <remarks>
	/// This specifies what direction the updates of each of the properties are automatically handled.
	/// Only properties that have a Changed event, or objects that implement <see cref="System.ComponentModel.INotifyPropertyChanged"/>
	/// will handle automatically updating the binding.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public enum DualBindingMode
	{
		/// <summary>
		/// Binding will update the destination if the source property is changed
		/// </summary>
		OneWay,

		/// <summary>
		/// Binding will update both the destination or source if updated on either the source or destination, respectively
		/// </summary>
		TwoWay,

		/// <summary>
		/// Binding will update the source if the destination property is changed
		/// </summary>
		OneWayToSource,

		/// <summary>
		/// Binding will only set the destination from the source when initially bound
		/// </summary>
		/// <remarks>
		/// This is ideal when you want to set the values of the destination, then only update the source
		/// at certain times using the <see cref="DualBinding{T}.Update"/> method.
		/// </remarks>
		OneTime
	}

	/// <summary>
	/// Binding for joining two object bindings together
	/// </summary>
	/// <remarks>
	/// The DualBinding is the most useful binding, as it allows you to bind two objects together.
	/// This differs from the <see cref="IndirectBinding{T}"/> where it only specifies how to get/set the value from a single object.
	/// 
	/// </remarks>
	public class DualBinding<T> : Binding
	{
		bool channeling;

		/// <summary>
		/// Gets the source binding
		/// </summary>
		public DirectBinding<T> Source { get; private set; }

		/// <summary>
		/// Gets the destination binding
		/// </summary>
		public DirectBinding<T> Destination { get; private set; }

		/// <summary>
		/// Gets the mode of the binding
		/// </summary>
		public DualBindingMode Mode { get; private set; }

		/// <summary>
		/// Initializes a new instance of the DualBinding class with two object property bindings
		/// </summary>
		/// <param name="source">Object to retrieve the source value from</param>
		/// <param name="sourceProperty">Property to retrieve from the source</param>
		/// <param name="destination">Object to set the destination value to</param>
		/// <param name="destinationProperty">Property to set on the destination</param>
		/// <param name="mode">Mode of the binding</param>
		public DualBinding(object source, string sourceProperty, object destination, string destinationProperty, DualBindingMode mode = DualBindingMode.TwoWay)
			: this(
				new ObjectBinding<object, T>(source, sourceProperty),
				new ObjectBinding<object, T>(destination, destinationProperty),
				mode
				)
		{
		}

		/// <summary>
		/// Initializes a new instance of the DualBinding class with two specified bindings
		/// </summary>
		/// <param name="source">Binding for retrieving the source value from</param>
		/// <param name="destination">Binding for setting the destination value to</param>
		/// <param name="mode">Mode of the binding</param>
		public DualBinding(DirectBinding<T> source, DirectBinding<T> destination, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			this.Source = source;
			this.Destination = destination;
			this.Mode = mode;

			if (mode == DualBindingMode.OneWay || mode == DualBindingMode.TwoWay)
				source.DataValueChanged += HandleSourceChanged;
			if (mode == DualBindingMode.OneWayToSource || mode == DualBindingMode.TwoWay)
				destination.DataValueChanged += HandleDestinationChanged;

			// set initial value
			this.SetDestination();
		}

		void HandleSourceChanged(object sender, EventArgs e)
		{
			SetDestination();
		}

		void HandleDestinationChanged(object sender, EventArgs e)
		{
			SetSource();
		}

		/// <summary>
		/// Sets the source object's property with the value of the destination
		/// </summary>
		public void SetSource()
		{
			if (!channeling)
			{
				channeling = true;
				Source.DataValue = Destination.DataValue;
				channeling = false;
			}
		}

		/// <summary>
		/// Sets the destination object's property with the value of the source
		/// </summary>
		public void SetDestination()
		{
			if (!channeling)
			{
				channeling = true;
				Destination.DataValue = Source.DataValue;
				channeling = false;
			}
		}

		/// <summary>
		/// Updates the binding value (sets the source with the value of the destination)
		/// </summary>
		public override void Update(BindingUpdateMode mode = BindingUpdateMode.Destination)
		{
			base.Update(mode);

			if (mode == BindingUpdateMode.Source)
				SetSource();
			else
				SetDestination();
		}

		/// <summary>
		/// Unbinds both the source and destination bindings
		/// </summary>
		public override void Unbind()
		{
			base.Unbind();

			Source.Unbind();
			Destination.Unbind();
		}
	}
}
