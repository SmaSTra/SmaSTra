using SmaSTraDesigner.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic.uitransactions
{
    public interface UITransaction
    {

        /// <summary>
        /// Applies the Transaction to the Designer.
        /// This can also be used for Redo.
        /// </summary>
        /// <param name="designer">To apply to.</param>
        void Redo(UcTreeDesigner designer);


        /// <summary>
        /// Undos the Action to the Designer.
        /// </summary>
        /// <param name="designer">To undo from.</param>
        void Undo(UcTreeDesigner designer);


    }
}
