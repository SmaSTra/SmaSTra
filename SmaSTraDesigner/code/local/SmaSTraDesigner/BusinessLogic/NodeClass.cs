﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic
{
	public class NodeClass
	{
		public NodeClass(string name, Node baseNode, DataType outputType, DataType[] inputTypes = null)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("String argument 'name' must not be null or empty (incl. whitespace).", "name");
			}
			if (baseNode == null)
			{
				throw new ArgumentNullException("baseNode");
			}
			if (baseNode is Transformation && (inputTypes == null || inputTypes.Length == 0))
			{
				throw new ArgumentException("There must be input types given for a transformation node class", "inputTypes");
			}

			this.Name = name;
			this.BaseNode = baseNode;
			baseNode.Class = this;
			this.OutputType = outputType;
			this.InputTypes = inputTypes;
		}

		// TODO: (PS) Comment this.
		public string Name
		{
			get;
			private set;
		}

		// TODO: (PS) Comment this.
		public string DisplayName
		{
			get;
			set;
		}
		
		// TODO: (PS) Comment this.
		public string Description
		{
			get;
			set;
		}

		// TODO: (PS) Comment this.
		public DataType[] InputTypes
		{
			get;
			private set;
		}

		// TODO: (PS) Comment this.
		public DataType OutputType
		{
			get;
			private set;
		}

		// TODO: (PS) Comment this.
		public Node BaseNode
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return String.Format("{{{0} {1}}}", this.GetType().Name, this.Name);
		}
	}
}