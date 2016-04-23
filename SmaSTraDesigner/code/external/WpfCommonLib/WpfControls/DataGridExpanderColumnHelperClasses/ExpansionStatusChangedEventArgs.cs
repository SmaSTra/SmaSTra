namespace Common.WpfControls.DataGridExpanderColumnHelperClasses
{
    using System;
    using System.Windows.Controls;

    #region Enumerations

    /// <summary>
    /// Used to specify the type of change in the expansion status of a datagrid row.
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// Row is/will be expanded.
        /// </summary>
        Expanded = 0,
        /// <summary>
        /// Row is/will be collapsed.
        /// </summary>
        Collapsed
    }

    #endregion Enumerations

    /// <summary>
    /// Event arguments for the <see cref="UcDataGridExpanderColumn"/> control's ItemCollapsed and ItemExpanded events.
    /// </summary>
    public class ExpansionStatusChangedEventArgs : EventArgs
    {
        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpansionStatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="item">The item who's row's expansion status changed.</param>
        /// <param name="dataGrid">The owner DataGrid.</param>
        public ExpansionStatusChangedEventArgs(ChangeType changeType, object item, DataGrid dataGrid)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (dataGrid == null)
            {
                throw new ArgumentNullException("dataGrid");
            }

            this.ChangeType = changeType;
            this.Item = item;
            this.DataGrid = dataGrid;
        }

        #endregion constructors

        #region properties

        /// <summary>
        /// Gets the type of the change.
        /// </summary>
        public ChangeType ChangeType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the owner DataGrid.
        /// </summary>
        public DataGrid DataGrid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the item who's row's expansion status changed.
        /// </summary>
        public object Item
        {
            get;
            private set;
        }

        #endregion properties
    }
}