namespace SmaSTraDesigner.BusinessLogic
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class Transformation : Node
	{
		#region fields

		/// <summary>
		/// TODO: (PS) Comment this.
		/// </summary>
		private ObservableCollection<Node> inputNodes = null;

		#endregion fields

		#region properties

		/// <summary>
		/// Gets or sets the InputNodes property value.
		/// TODO: (PS) Comment this.
		/// </summary>
		public ObservableCollection<Node> InputNodes
		{
			get
			{
				return this.inputNodes;
			}
			set
			{
				if (value != this.inputNodes)
				{
					ObservableCollection<Node> oldValue = this.inputNodes;
					this.inputNodes = value;
					this.OnInputNodesChanged(oldValue, value);
					this.OnPropertyChanged("InputNodes");
				}
			}
		}

		#endregion properties

		#region overrideable methods

		/// <summary>
		/// Called when the InputNodes property changed its value.
		/// </summary>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void OnInputNodesChanged(ObservableCollection<Node> oldValue, ObservableCollection<Node> newValue)
		{
		}

		#endregion overrideable methods
	}
}