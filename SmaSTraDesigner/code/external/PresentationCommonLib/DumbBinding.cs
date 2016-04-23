namespace Common
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Windows;
	using System.Windows.Data;

	using Common.EventArgs;

	// TODO: (PS) Comment this.
	/// <summary>
	/// Provides the possibility to bind properties of non-DependencyObject classes
	/// using the PropertyChangedHandle class.
	/// </summary>
	public class DumbBinding
	{
		#region static fields

		private static Dictionary<KeyValuePair<int, int>, List<DumbBinding>> instances = new Dictionary<KeyValuePair<int, int>, List<DumbBinding>>();

		#endregion static fields

		#region static methods

		public static void ClearAllBindings(object targetObjectOrType)
		{
			if (targetObjectOrType == null)
			{
				throw new ArgumentNullException("targetObjectOrType");
			}

			lock (instances)
			{
				int hashCode = targetObjectOrType.GetHashCode();
				foreach (var key in instances.Keys.Where(kvp => kvp.Value == hashCode).ToList())
				{
					var list = instances[key];
					for (int i = list.Count - 1; i >= 0; i--)
					{
						DumbBinding binding = list[i];
						if (object.Equals(targetObjectOrType, binding.Target.TargetObject) || binding.Target.TargetObject == null)
						{
							binding.Clear();
							list.RemoveAt(i);
						}
					}

					if (list.Count == 0)
					{
						instances.Remove(key);
					}
				}
			}
		}

		public static void ClearBinding(Type targetType, string targetPropertyName, object[] targetPropertyIndex = null)
		{
			ClearBinding(new StaticDumbBindingTarget(targetType, targetPropertyName, false, false, targetPropertyIndex));
		}

		public static void ClearBinding(object targetObject, string targetPropertyName, object[] targetPropertyIndex = null)
		{
			ClearBinding(new InstanceDumbBindingTarget(targetObject, targetPropertyName, false, false, targetPropertyIndex));
		}

		public static void ClearBinding(DumbBindingTarget target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			object targetObject = target.TargetObject;
			if (targetObject == null)
			{
				throw new ArgumentException("target.TargetObject is no longer alive.", "target");
			}

			lock (instances)
			{
				KeyValuePair<int, int> key = new KeyValuePair<int, int>(target.GetHashCode(), targetObject.GetHashCode());
				if (instances.ContainsKey(key))
				{
					var list = instances[key];
					for (int i = list.Count - 1; i >= 0; i--)
					{
						DumbBinding binding = list[i];
						if (object.Equals(target, binding.Target) || binding.Target.TargetObject == null)
						{
							binding.Clear();
							list.RemoveAt(i);
						}
					}

					if (list.Count == 0)
					{
						instances.Remove(key);
					}
				}
			}
		}

		public static void ClearUnusedBindings()
		{
			lock (instances)
			{
				foreach (var key in instances.Keys.ToArray())
				{
					List<DumbBinding> list = new List<DumbBinding>();
					foreach (var binding in instances[key])
					{
						if (binding.Target.TargetObject == null)
						{
							binding.Clear();
						}
						else
						{
							list.Add(binding);
						}
					}

					if (list.Count == 0)
					{
						instances.Remove(key);
					}
					else
					{
						instances[key] = list;
					}
				}
			}
		}

		public static bool DoesBindingWrite(BindingBase binding)
		{
			Binding bindingAsBinding;
			MultiBinding bindingAsMultiBinding;
			if ((bindingAsBinding = binding as Binding) != null)
			{
				return PropertyChangedHandle.DoesBindingWrite(bindingAsBinding);
			}
			else if ((bindingAsMultiBinding = binding as MultiBinding) != null)
			{
				return bindingAsMultiBinding.Mode == BindingMode.TwoWay || bindingAsMultiBinding.Mode == BindingMode.OneWayToSource;
			}

			throw new NotSupportedException(String.Format("Binding type {0} not supported.", binding.GetType()));
		}

		public static DumbBinding GetBinding(Type targetType, string targetPropertyName, object[] targetPropertyIndex = null)
		{
			return GetBinding(new StaticDumbBindingTarget(targetType, targetPropertyName, false, false, targetPropertyIndex));
		}

		public static DumbBinding GetBinding(object targetObject, string targetPropertyName, object[] targetPropertyIndex = null)
		{
			return GetBinding(new InstanceDumbBindingTarget(targetObject, targetPropertyName, false, false, targetPropertyIndex));
		}

		public static DumbBinding GetBinding(DumbBindingTarget target)
		{
			lock (instances)
			{
				if (target == null)
				{
					throw new ArgumentNullException("target");
				}

				object targetObject = target.TargetObject;
				if (targetObject == null)
				{
					throw new ArgumentException("target.TargetObject is no longer alive.", "target");
				}

				KeyValuePair<int, int> key = new KeyValuePair<int, int>(target.GetHashCode(), targetObject.GetHashCode());
				if (instances.ContainsKey(key))
				{
					return instances[key].FirstOrDefault(binding => object.Equals(binding.Target, target));
				}
				else
				{
					return null;
				}
			}
		}

		public static DumbBinding SetBinding(object targetObject, string targetPropertyName, object[] targetPropertyIndex, BindingBase bindingToSource)
		{
			return SetBinding(new InstanceDumbBindingTarget(targetObject, targetPropertyName, DoesBindingWrite(bindingToSource),
				DoesBindingRead(bindingToSource), targetPropertyIndex), bindingToSource);
		}

		public static DumbBinding SetBinding(object targetObject, string targetPropertyName, BindingBase bindingToSource)
		{
			return SetBinding(targetObject, targetPropertyName, null, bindingToSource);
		}

		public static DumbBinding SetBinding(object targetObject, DependencyProperty targetProperty, BindingBase bindingToSource)
		{
			if (targetProperty == null)
			{
				throw new ArgumentNullException("targetProperty");
			}

			return SetBinding(targetObject, targetProperty.Name, null, bindingToSource);
		}

		public static DumbBinding SetBinding(Type targetType, string targetPropertyName, object[] targetPropertyIndex, BindingBase bindingToSource)
		{
			return SetBinding(new StaticDumbBindingTarget(targetType, targetPropertyName, DoesBindingWrite(bindingToSource),
				DoesBindingRead(bindingToSource), targetPropertyIndex), bindingToSource);
		}

		public static DumbBinding SetBinding(Type targetType, string targetPropertyName, BindingBase bindingToSource)
		{
			return SetBinding(targetType, targetPropertyName, null, bindingToSource);
		}

		public static DumbBinding SetBinding(DumbBindingTarget target, BindingBase bindingToSource)
		{
			lock (instances)
			{
				if (target == null)
				{
					throw new ArgumentNullException("target");
				}

				DumbBinding result = new DumbBinding(bindingToSource, target);

				ClearBinding(target);

				object targetObject = target.TargetObject;
				if (targetObject == null)
				{
					throw new ArgumentException("target.TargetObject is no longer alive.", "target");
				}

				KeyValuePair<int, int> key = new KeyValuePair<int, int>(target.GetHashCode(), targetObject.GetHashCode());
				if (instances.ContainsKey(key))
				{
					instances[key].Add(result);
				}
				else
				{
					instances.Add(key, new List<DumbBinding>() { result });
				}

				return result;
			}
		}

		private static bool DoesBindingRead(BindingBase binding)
		{
			Binding bindingAsBinding;
			MultiBinding bindingAsMultiBinding;
			if ((bindingAsBinding = binding as Binding) != null)
			{
				return PropertyChangedHandle.DoesBindingRead(bindingAsBinding);
			}
			else if ((bindingAsMultiBinding = binding as MultiBinding) != null)
			{
				return bindingAsMultiBinding.Mode == BindingMode.Default || bindingAsMultiBinding.Mode == BindingMode.OneWay || bindingAsMultiBinding.Mode == BindingMode.TwoWay;
			}

			throw new NotSupportedException(String.Format("Binding type {0} not supported.", binding.GetType()));
		}

		#endregion static methods

		#region fields

		private object monitor = new object();
		private bool updatingTarget = false;

		#endregion fields

		#region constructors

		private DumbBinding(BindingBase bindingToSource, DumbBindingTarget target)
		{
			if (bindingToSource == null)
			{
				throw new ArgumentNullException("bindingToSource");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			this.BindingToSource = bindingToSource;
			this.Target = target;
			target.PropertyChangedCallback = this.OnTargetPropertyChanged;

			BindingMode? bindingMode = null;
			Binding bindingToSourceAsBinding;
			MultiBinding bindingToSourceAsMultiBinding;
			if ((bindingToSourceAsBinding = bindingToSource as Binding) != null)
			{
				bindingMode = bindingToSourceAsBinding.Mode;
				this.SourcePropertyChangedHandle = PropertyChangedHandle.GetDistinctInstance(bindingToSourceAsBinding, this.OnSourceValueChanged);
				this.UpdateTarget(this.SourcePropertyChangedHandle.PropertyValue, false);
			}
			else if ((bindingToSourceAsMultiBinding = bindingToSource as MultiBinding) != null)
			{
				bindingMode = bindingToSourceAsMultiBinding.Mode;
				this.SourceMultiPropertyChangedHandle = MultiPropertyChangedHandle.GetDistinctInstance(bindingToSourceAsMultiBinding.Bindings.Cast<Binding>().ToArray(),
					this.OnSourceValuesChanged);
				this.UpdateTarget(this.MultiConvert(this.GetSourceValues()), false);
			}
			else
			{
				throw new NotSupportedException(String.Format("Binding type {0} not supported.", bindingToSource.GetType()));
			}

			if (bindingMode == BindingMode.OneWayToSource)
			{
				this.UpdateSource();
			}
		}

		#endregion constructors

		#region events

		/// <summary>
		/// Occurs when source value was updated.
		/// </summary>
		public event EventHandler<DumbBindingValueUpdatedEventArgs> SourceUpdated;

		/// <summary>
		/// Occurs when target value was updated.
		/// </summary>
		public event EventHandler<DumbBindingValueUpdatedEventArgs> TargetUpdated;

		#endregion events

		#region properties

		/// <summary>
		/// Gets the binding status.
		/// </summary>
		public BindingStatus BindingStatus
		{
			get
			{
				if (this.SourcePropertyChangedHandle != null)
				{
					return this.SourcePropertyChangedHandle.BindingStatus;
				}
				else
				{
					// TODO: (PS) Implement this.
					Debug.WriteLine("Binding status for Multi-Dumb-Binding not implemented.");
					return BindingStatus.Inactive;
				}
			}
		}

		/// <summary>
		/// Gets the binding's source.
		/// </summary>
		public BindingBase BindingToSource
		{
			get;
			private set;
		}

		public MultiPropertyChangedHandle SourceMultiPropertyChangedHandle
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the PropertyChangedHandle used to monitor the source property.
		/// </summary>
		public PropertyChangedHandle SourcePropertyChangedHandle
		{
			get;
			private set;
		}

		public DumbBindingTarget Target
		{
			get;
			private set;
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			DumbBinding other = obj as DumbBinding;

			return other != null &&
				((this.SourcePropertyChangedHandle != null &&
				PropertyChangedHandle.Equals(this.SourcePropertyChangedHandle, other.SourcePropertyChangedHandle, false)) ||
				(this.SourceMultiPropertyChangedHandle != null &&
				MultiPropertyChangedHandle.Equals(this.SourceMultiPropertyChangedHandle, other.SourceMultiPropertyChangedHandle, false))) &&
				object.Equals(this.Target, other.Target);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return HashCodeOperations.Combine(this.BindingToSource, this.Target);
		}

		#endregion overrideable methods

		#region methods

		public void UpdateSource()
		{
			object value = this.Target.PropertyValue;
			if (value != DependencyProperty.UnsetValue)
			{
				this.UpdateSource(value);
			}
		}

		public void UpdateSource(object value)
		{
			if (DoesBindingWrite(this.BindingToSource))
			{
				if (this.SourcePropertyChangedHandle != null)
				{
					object oldValue = this.SourcePropertyChangedHandle.PropertyValue;
					this.SourcePropertyChangedHandle.PropertyValue = value;
					this.OnSourceUpdated(new DumbBindingValueUpdatedEventArgs(oldValue, value));
				}
				else
				{
					var oldValues = this.GetSourceValues();
					var newValues = this.MultiConvertBack(value);
					for (int i = 0; i < newValues.Length; i++)
					{
						this.SourceMultiPropertyChangedHandle.PropertyChangedHandles[i].PropertyValue = newValues[i];
					}

					this.OnSourceUpdated(new DumbBindingValueUpdatedEventArgs(oldValues, newValues));
				}
			}
		}

		/// <summary>
		/// Updates the target property using the current source value.
		/// </summary>
		public void UpdateTarget()
		{
			if (DoesBindingRead(this.BindingToSource))
			{
				if (this.SourcePropertyChangedHandle != null)
				{
					this.UpdateTarget(this.SourcePropertyChangedHandle.PropertyValue);
				}
				else
				{
					this.UpdateTarget(this.MultiConvert(this.GetSourceValues()));
				}
			}
		}

		/// <summary>
		/// Updates the target property using the given value.
		/// </summary>
		/// <param name="value">The new value.</param>
		public void UpdateTarget(object value)
		{
			this.UpdateTarget(value, true);
		}

		private void Clear()
		{
			this.Target.Dispose();
			if (this.SourcePropertyChangedHandle != null)
			{
				this.SourcePropertyChangedHandle.Dispose();
			}
			else
			{
				this.SourceMultiPropertyChangedHandle.Dispose();
			}
		}

		private object[] GetSourceValues()
		{
			return this.SourceMultiPropertyChangedHandle.PropertyChangedHandles.Select(handle => handle.PropertyValue).ToArray();
		}

		private object MultiConvert(object[] values)
		{
			MultiBinding multiBinding = (MultiBinding)this.BindingToSource;

			return multiBinding.Converter.Convert(values, this.Target.TargetProperty.PropertyType, multiBinding.ConverterParameter,
				multiBinding.ConverterCulture ?? CultureInfo.CurrentCulture);
		}

		private object[] MultiConvertBack(object value)
		{
			MultiBinding multiBinding = (MultiBinding)this.BindingToSource;

			return multiBinding.Converter.ConvertBack(value,
				Enumerable.Repeat(typeof(object), this.SourceMultiPropertyChangedHandle.PropertyChangedHandles.Length).ToArray(),
				multiBinding.ConverterParameter, multiBinding.ConverterCulture);
		}

		/// <summary>
		/// Raises the <see cref="E:SourceUpdated"/> event.
		/// </summary>
		/// <param name="e">The PropertyChangedEventArgs instance containing the event data.</param>
		/// <returns>The PropertyChangedEventArgs instance containing the event data.</returns>
		private DumbBindingValueUpdatedEventArgs OnSourceUpdated(DumbBindingValueUpdatedEventArgs e)
		{
			if (this.SourceUpdated != null)
			{
				this.SourceUpdated(this, e);
			}

			return e;
		}

		/// <summary>
		/// Called when the source property's value changed.
		/// </summary>
		/// <param name="args">The PropertyChangedCallbackArgs.</param>
		private void OnSourceValueChanged(PropertyChangedCallbackArgs args)
		{
			this.UpdateTarget();
		}

		private void OnSourceValuesChanged(MultiPropertyChangedCallbackArgs args)
		{
			this.UpdateTarget();
		}

		private void OnTargetPropertyChanged(object newValue)
		{
			lock (monitor)
			{
				if (!this.updatingTarget)
				{
					this.UpdateSource(newValue);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="E:TargetUpdated"/> event.
		/// </summary>
		/// <param name="e">The PropertyChangedEventArgs instance containing the event data.</param>
		/// <returns>The PropertyChangedEventArgs instance containing the event data.</returns>
		private DumbBindingValueUpdatedEventArgs OnTargetUpdated(DumbBindingValueUpdatedEventArgs e)
		{
			if (this.TargetUpdated != null)
			{
				this.TargetUpdated(this, e);
			}

			return e;
		}

		private void UpdateTarget(object value, bool raiseEvent)
		{
			if (DoesBindingRead(this.BindingToSource))
			{
				lock (monitor)
				{
					try
					{
						object oldValue = this.Target.PropertyValue;
						this.updatingTarget = true;
						this.Target.PropertyValue = value;
						this.updatingTarget = false;
						if (raiseEvent)
						{
							this.OnTargetUpdated(new DumbBindingValueUpdatedEventArgs(oldValue, value));
						}
					}
					catch (Exception ex)
					{
						throw new Exception("Updating target property did not work.", ex);
					}
				}
			}
		}

		#endregion methods
	}
}