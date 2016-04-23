using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Common
{
	// TODO: (PS) Comment this.
	// TODO: (PS) Finish this.
	public class UniversalEventChain
	{
		public UniversalEventHandle[] Handles
		{
			get;
			private set;
		}

		public UniversalEventChain(params UniversalEventHandle[] handles)
		{
			if (handles == null)
			{
				throw new ArgumentNullException("handles");
			}

			this.Handles = handles;
		}

		public UniversalEventChain(IEnumerable<UniversalEventHandle> handles)
		{
			if (handles == null)
			{
				throw new ArgumentNullException("handles");
			}

			this.Handles = handles.ToArray();
		}
	}
}
