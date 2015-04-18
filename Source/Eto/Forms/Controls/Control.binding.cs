using System;

namespace Eto.Forms
{
	public partial class Control
	{
		/// <summary>
		/// Adds a new dual binding between the control and the specified object
		/// </summary>
		/// <param name="propertyName">Property on the control to update</param>
		/// <param name="source">Object to bind to</param>
		/// <param name="sourcePropertyName">Property on the source object to retrieve/set the value of</param>
		/// <param name="mode">Mode of the binding</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public DualBinding<T> Bind<T>(string propertyName, object source, string sourcePropertyName, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding<T>(
				source,
				sourcePropertyName,
				this,
				propertyName,
				mode
			);
			Bindings.Add(binding);
			return binding;
		}

		/// <summary>
		/// Adds a new dual binding between the control and the specified source binding
		/// </summary>
		/// <param name="widgetPropertyName">Property on the control to update</param>
		/// <param name="sourceBinding">Binding to get/set the value to from the control</param>
		/// <param name="mode">Mode of the binding</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public DualBinding<T> Bind<T>(string widgetPropertyName, DirectBinding<T> sourceBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new DualBinding<T>(
				sourceBinding,
				new ControlBinding<Control,T>(this, new PropertyBinding<T>(widgetPropertyName)),
				mode
			);
			Bindings.Add(binding);
			return binding;
		}

		/// <summary>
		/// Adds a new binding with the control and the the control's current data context 
		/// </summary>
		/// <remarks>
		/// This binds to a property of the <see cref="Control.DataContext"/>, which will return the topmost value
		/// up the control hierarchy.  For example, you can set the DataContext of your form or panel, and then bind to properties
		/// of that context on any of the child controls such as a text box, etc.
		/// </remarks>
		/// <param name="controlPropertyName">Property on the control to update</param>
		/// <param name="dataContextPropertyName">Property on the control's <see cref="Control.DataContext"/> to bind to the control</param>
		/// <param name="mode">Mode of the binding</param>
		/// <param name="defaultControlValue">Default value to set to the control when the value from the DataContext is null</param>
		/// <param name="defaultContextValue">Default value to set to the DataContext property when the control value is null</param>
		/// <returns>A new instance of the DualBinding class that is used to control the binding</returns>
		public DualBinding<T> BindDataContext<T>(string controlPropertyName, string dataContextPropertyName, DualBindingMode mode = DualBindingMode.TwoWay, T defaultControlValue = default(T), T defaultContextValue = default(T))
		{
			var dataContextBinding = new PropertyBinding<T>(dataContextPropertyName);
			var controlBinding = new PropertyBinding<T>(controlPropertyName);
			return BindDataContext(controlBinding, dataContextBinding, mode, defaultControlValue, defaultContextValue);
		}

		/// <summary>
		/// Adds a new binding to the control with a direct value binding
		/// </summary>
		/// <param name="controlBinding">Binding to get/set the value from the control.</param>
		/// <param name="valueBinding">Value binding to get/set the value from another source.</param>
		/// <param name="mode">Mode of the binding</param>
		public DualBinding<T> Bind<T>(IndirectBinding<T> controlBinding, DirectBinding<T> valueBinding, DualBindingMode mode = DualBindingMode.TwoWay)
		{
			var binding = new ControlBinding<Control,T>(this, controlBinding);
			return binding.Bind(sourceBinding: valueBinding, mode: mode);
		}

		/// <summary>
		/// Adds a new binding to the control with an indirect binding to the provided <paramref name="objectValue"/>
		/// </summary>
		/// <param name="controlBinding">Binding to get/set the value from the control.</param>
		/// <param name="objectValue">Object value to bind to.</param>
		/// <param name="objectBinding">Binding to get/set the value from the <paramref name="objectValue"/>.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <param name="defaultControlValue">Default control value to set to the objectValue, if the value of the control property is null.</param>
		/// <param name="defaultContextValue">Default context value to set to the control, if the objectValue or value of the objectBinding is null.</param>
		public DualBinding<T> Bind<T>(IndirectBinding<T> controlBinding, object objectValue, IndirectBinding<T> objectBinding, DualBindingMode mode = DualBindingMode.TwoWay, T defaultControlValue = default(T), T defaultContextValue = default(T))
		{
			var valueBinding = new ObjectBinding<object,T>(objectValue, objectBinding) {
				SettingNullValue = defaultContextValue,
				GettingNullValue = defaultControlValue
			};
			return Bind(controlBinding, valueBinding, mode);
		}

		/// <summary>
		/// Adds a new binding from the control to its data context
		/// </summary>
		/// <param name="controlBinding">Binding to get/set the value from the control.</param>
		/// <param name="dataContextBinding">Binding to get/set the value from the <see cref="Control.DataContext"/>.</param>
		/// <param name="mode">Mode of the binding.</param>
		/// <param name="defaultControlValue">Default control value to set to the objectValue, if the value of the control property is null.</param>
		/// <param name="defaultContextValue">Default context value to set to the control, if the objectValue or value of the objectBinding is null.</param>
		public DualBinding<T> BindDataContext<T>(IndirectBinding<T> controlBinding, IndirectBinding<T> dataContextBinding, DualBindingMode mode = DualBindingMode.TwoWay, T defaultControlValue = default(T), T defaultContextValue = default(T))
		{
			var binding = new ControlBinding<Control, T>(this, controlBinding);
			return binding.BindDataContext(dataContextBinding, mode, defaultControlValue, defaultContextValue);
		}
	}
}

