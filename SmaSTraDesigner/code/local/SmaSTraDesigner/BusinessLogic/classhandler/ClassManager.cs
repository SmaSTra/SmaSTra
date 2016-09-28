﻿namespace SmaSTraDesigner.BusinessLogic
{
    using classhandler;
    using classhandler.nodeclasses;
    using codegeneration.loader;
    using Common;
    using config;
    using nodes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using utils;

    /// <summary>
    /// Loads and manages node classes from given metadata generated by SmaSTra base library.
    /// </summary>
    public class ClassManager : INotifyPropertyChanged
	{

        #region fields

        /// <summary>
        /// The Toggle for the Basic Elements
        /// </summary>
        private bool toggleBasic = false;

        /// <summary>
        /// The Toggle for the Custom Elements
        /// </summary>
        private bool toggleCustom = false;

        /// <summary>
        /// The Toggle for the Combined Elements
        /// </summary>
        private bool toggleCombined = false;

        /// <summary>
        /// The Type to filter for.
        /// </summary>
        private string selectedCategory = "none";
        /// <summary>
        /// The Name to filter for.
        /// </summary>
        private string filterString = "";


        /// <summary>
        /// The cached filtered Nodes.
        /// </summary>
        private Node[] filteredNodes = null;


        //////DELETE BELOW IF WORKING!////

        /// <summary>
        /// List of all loaded transformations that represent a simple conversion (one input one output).
        /// </summary>
        private Transformation[] baseConversions = null;

        /// <summary>
		/// List of all custom transformations that represent a simple conversion (one input one output).
		/// </summary>
		private Transformation[] customConversions = null;

        /// <summary>
        /// List of all loaded Buffers that.
        /// </summary>
        private BufferNode[] baseBuffers = null;

        /// <summary>
		/// List of all loaded Buffers that
		/// </summary>
		private BufferNode[] customBuffers = null;

        /// <summary>
        /// List of all loaded data sources.
        /// </summary>
        private DataSource[] baseDataSources = null;

        /// <summary>
		/// List of all custom data sources.
		/// </summary>
		private DataSource[] customDataSources = null;

        /// <summary>
        /// List of all transformations (that do not fall in the conversion category).
        /// </summary>
        private Transformation[] baseTransformations = null;

        /// <summary>
        /// List of all custom transformations (that do not fall in the conversion category).
        /// </summary>
        private Transformation[] customTransformations = null;


        /// <summary>
        /// List of all Combined Nodes.
        /// </summary>
        private CombinedNode[] baseCombinedNodes = null;

        /// <summary>
        /// List of all Combined Conversions.
        /// </summary>
        private CombinedNode[] combinedConversions = null;

        /// <summary>
        /// List of all Combined  DataSources.
        /// </summary>
        private CombinedNode[] combinedDataSources = null;

        /// <summary>
        /// List of all Combined Transformations.
        /// </summary>
        private CombinedNode[] combinedTransformations = null;

        /// <summary>
        /// Dictionary that keeps track of loaded node classes to ensure no ambiguity.
        /// </summary>
        private Dictionary<string, AbstractNodeClass> classes = new Dictionary<string, AbstractNodeClass>();

		/// <summary>
		/// Dictionary that keeps track of loaded data types to ensure no ambiguity.
		/// </summary>
		private Dictionary<string, DataType> dataTypes = new Dictionary<string, DataType>();

		#endregion fields

		#region events

		/// <summary>
		/// Is raised whenever a compatible property changes its value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

        #endregion events

        #region properties


        /// <summary>
        /// The Toggle for the Basic Elements to Bind to the GUI.
        /// </summary>
        public bool ToggleBasic {
            get
            { return toggleBasic; }
            set
            {
                this.toggleBasic = value;
                this.filteredNodes = null;
                this.OnPropertyChanged("FilteredNodes");
            }
        }


        /// <summary>
        /// The Toggle for the Custom Elements to Bind to the GUI.
        /// </summary>
        public bool ToggleCustom
        {
            get
            { return toggleCustom; }
            set
            {
                this.toggleCustom = value;
                this.filteredNodes = null;
                this.OnPropertyChanged("FilteredNodes");
            }
        }


        /// <summary>
        /// The Toggle for the Combined Elements to Bind to the GUI.
        /// </summary>
        public bool ToggleCombined
        {
            get
            { return toggleCombined; }
            set
            {
                this.toggleCombined = value;
                this.filteredNodes = null;
                this.OnPropertyChanged("FilteredNodes");
            }
        }


        /// <summary>
        /// The Selected category to filter to Bind to the GUI.
        /// </summary>
        public string SelectedCategory
        {
            get
            { return selectedCategory; }
            set
            {
                this.selectedCategory = value;
                this.filteredNodes = null;
                this.OnPropertyChanged("FilteredNodes");
            }
        }

        /// <summary>
        /// The String to filter the names for to Bind to the GUI.
        /// </summary>
        public string FilterString
        {
            get
            { return filterString; }
            set
            {
                this.filterString = value.ToLower();
                this.filteredNodes = null;
                this.OnPropertyChanged("FilteredNodes");
            }
        }

        /// <summary>
        /// Gets the Filtered Nodes. To bind to the GUI.
        /// </summary>
        public Node[] FilteredNodes
        {
            get
            {
                if (this.filteredNodes == null)
                {
                    Func<AbstractNodeClass, bool> typeFilter = (n =>
                    {
                        if (this.selectedCategory == "datasource") return n.InputTypes.Count() == 0;
                        if (this.selectedCategory == "conversion") return n.InputTypes.Count() == 1;
                        if (this.selectedCategory == "transformation") return n.InputTypes.Count() > 1;
                        if (this.selectedCategory == "buffer") return n is BufferNodeClass;
                        return false;
                    });

                    Func<AbstractNodeClass, bool> baseFilter = (n => { return toggleBasic && !n.UserCreated; });
                    Func<AbstractNodeClass, bool> customFilter = (n => { return toggleCustom && n.UserCreated; });
                    Func<AbstractNodeClass, bool> combinedFilter = (n => { return toggleCombined && n is CombinedNodeClass; });
                    Func<AbstractNodeClass, bool> nameFilter = (n => { return string.IsNullOrWhiteSpace(this.filterString) || n.Name.ToLower().Contains(filterString); });

                    //Filter + Generate:
                    return classes.Values
                        .Where(typeFilter)

                        .Where(baseFilter)
                        .Where(customFilter)
                        .Where(combinedFilter)

                        .Where(nameFilter)

                        .Distinct()
                        .Select(n => n.generateNode())
                        .NonNull()
                        .OrderBy(s => s.Class.Name)
                        .ToArray();
                }

                return this.filteredNodes;
            }
        }


        /////DELETE BELOW WHEN WORKING////

        /// <summary>
        /// Gets the BaseConversions instance (creates one if none exists).
        /// List of all base transformations that represent a simple conversion (one input one output).
        /// </summary>
        public Transformation[] BaseConversions
		{
			get
			{
				if (this.baseConversions == null)
				{
					this.baseConversions = this.classes.Values
                        .Where(cls => cls is TransformationNodeClass && cls.InputTypes.Length == 1 && !cls.UserCreated)
						.Select(cls => cls.generateNode() as Transformation)
                        .NonNull()
                        .ToArray();
				}

				return this.baseConversions;
			}
		}

        /// <summary>
		/// Gets the BaseConversions instance (creates one if none exists).
		/// List of all custom transformations that represent a simple conversion (one input one output).
		/// </summary>
		public Transformation[] CustomConversions
        {
            get
            {
                if (this.customConversions == null)
                {
                    this.customConversions = this.classes.Values
                        .Where(cls => cls is TransformationNodeClass && cls.InputTypes.Length == 1 && cls.UserCreated)
                        .NonNull()
                        .Select(cls => cls.generateNode() as Transformation)
                        .ToArray();
                }

                return this.customConversions;
            }
        }

        /// <summary>
        /// Gets the Base Buffers instance (creates one if none exists).
        /// List of all base Buffers Collecting stuff.
        /// </summary>
        public BufferNode[] BaseBuffers
        {
            get
            {
                if (this.baseBuffers == null)
                {
                    this.baseBuffers = this.classes.Values
                        .Where(cls => cls is BufferNodeClass && !cls.UserCreated)
                        .Select(cls => cls.generateNode() as BufferNode)
                        .NonNull()
                        .ToArray();
                }

                return this.baseBuffers;
            }
        }

        /// <summary>
        /// Gets the Created Buffers instance (creates one if none exists).
        /// List of all base Buffers Collecting stuff.
		/// </summary>
		public BufferNode[] CustomBuffers
        {
            get
            {
                if (this.baseBuffers == null)
                {
                    this.baseBuffers = this.classes.Values
                        .Where(cls => cls is BufferNodeClass && cls.UserCreated)
                        .Select(cls => cls.generateNode() as BufferNode)
                        .NonNull()
                        .ToArray();
                }

                return this.baseBuffers;
            }
        }

        /// <summary>
        /// Gets the BaseDataSources instance (creates one if none exists).
        /// List of all base data sources.
        /// </summary>
        public DataSource[] BaseDataSources
		{
			get
			{
				if (this.baseDataSources == null)
				{
					this.baseDataSources = this.classes.Values
                        .Where(cls => cls is DataSourceNodeClass && !cls.UserCreated)
						.Select(cls => (DataSource)cls.generateNode())
                        .NonNull()
                        .ToArray();
				}

				return this.baseDataSources;
			}
		}

        /// <summary>
		/// Gets the CustomDataSources instance (creates one if none exists).
		/// List of all custom data sources.
		/// </summary>
		public DataSource[] CustomDataSources
        {
            get
            {
                if (this.customDataSources == null)
                {
                    this.customDataSources = this.classes.Values
                        .Where(cls => cls is DataSourceNodeClass && cls.UserCreated)
                        .Select(cls => (DataSource)cls.generateNode())
                        .NonNull()
                        .ToArray();
                }

                return this.customDataSources;
            }
        }

        /// <summary>
        /// Gets the BaseTransformations instance (creates one if none exists).
        /// List of all base transformations (that do not fall in the conversion category).
        /// </summary>
        public Transformation[] BaseTransformations
		{
			get
			{
				if (this.baseTransformations == null)
				{
					this.baseTransformations = this.classes.Values
                        .Where(cls => cls is TransformationNodeClass && cls.InputTypes.Length > 1 && !cls.UserCreated)
						.Select(cls => (Transformation)cls.generateNode())
                        .NonNull()
                        .ToArray();
				}

				return this.baseTransformations;
			}
		}

        /// <summary>
        /// Gets the BaseTransformations instance (creates one if none exists).
        /// List of all custom transformations (that do not fall in the conversion category).
        /// </summary>
        public Transformation[] CustomTransformations
        {
            get
            {
                if (this.customTransformations == null)
                {
                    this.customTransformations = this.classes.Values
                        .Where(cls => cls is TransformationNodeClass && cls.InputTypes.Length > 1 && cls.UserCreated)
                        .Select(cls => (Transformation)cls.generateNode())
                        .NonNull()
                        .ToArray();
                }

                return this.customTransformations;
            }
        }


        /// <summary>
        /// Gets the Base Combined Nodes instance (creates one if none exists).
        /// List of all Combined Nodes.
        /// </summary>
        public CombinedNode[] BaseCombinedNodes
        {
            get
            {
                if (this.baseCombinedNodes == null)
                {
                    this.baseCombinedNodes = this.classes.Values
                        .Where(cls => cls is CombinedNodeClass)
                        .Select(cls => (CombinedNode)cls.generateNode())
                        .NonNull()
                        .ToArray();
                }

                return this.baseCombinedNodes;
            }
        }

        /// <summary>
        /// Gets the Combined Conversions instance (creates one if none exists).
        /// List of all Combined Conversions.
        /// </summary>
        public CombinedNode[] CombinedConversions
        {
            get
            {
                if (this.combinedConversions == null)
                {
                    this.combinedConversions = this.classes.Values
                        .Where(cls => cls is CombinedNodeClass && cls.InputTypes.Length == 1)
                        .Select(cls => (CombinedNode)cls.generateNode())
                        .NonNull()
                        .ToArray();
                }

                return this.combinedConversions;
            }
        }

        // <summary>
        /// Gets the Combined DataSources instance (creates one if none exists).
        /// List of all Combined DataSources.
        /// </summary>
        public CombinedNode[] CombinedDataSources
        {
            get
            {
                if (this.combinedDataSources == null)
                {
                    this.combinedDataSources = this.classes.Values
                        .Where(cls => cls is CombinedNodeClass && cls.InputTypes.Length == 0)
                        .Select(cls => (CombinedNode)cls.generateNode())
                        .NonNull()
                        .ToArray();
                }

                return this.combinedDataSources;
            }
        }

        // <summary>
        /// Gets the Combined Transformations instance (creates one if none exists).
        /// List of all Combined Conversions.
        /// </summary>
        public CombinedNode[] CombinedTransformations
        {
            get
            {
                if (this.combinedTransformations == null)
                {
                    this.combinedTransformations = this.classes.Values
                        .Where(cls => cls is CombinedNodeClass && cls.InputTypes.Length > 1)
                        .Select(cls => (CombinedNode)cls.generateNode())
                        .NonNull()
                        .ToArray();
                }

                return this.combinedTransformations;
            }
        }

        #endregion properties

        #region methods


        /// <summary>
        /// Adds the Node-Class passed.
        /// </summary>
        /// <param name="nodeClass"> to add. </param>
        /// <returns></returns>
        public AbstractNodeClass AddClass(AbstractNodeClass nodeClass)
        {
            if (nodeClass == null) return null;

            //If already present -> Do not add!
            AbstractNodeClass result;
            if (this.classes.TryGetValue(nodeClass.Name, out result)) return result;

            this.classes.Add(nodeClass.Name, nodeClass);

            // Send property change notifications for appropreate list property, so the GUI can react.
            switch (nodeClass.NodeType)
            {
                case NodeType.Transformation:
                    if (nodeClass.InputTypes.Length == 1)
                    {
                        this.baseConversions = null;
                        this.customConversions = null;
                        this.OnPropertyChanged("BaseConversions");
                        this.OnPropertyChanged("CustomConversions");
                    }
                    else
                    {
                        this.baseTransformations = null;
                        this.customTransformations = null;
                        this.OnPropertyChanged("BaseTransformations");
                        this.OnPropertyChanged("CustomTransformations");
                    }
                    break;

                case NodeType.Sensor:
                    this.baseDataSources = null;
                    this.customDataSources = null;
                    this.OnPropertyChanged("BaseDataSources");
                    this.OnPropertyChanged("CustomDataSources");
                    break;

                case NodeType.Combined:
                    this.baseCombinedNodes = null;
                    this.OnPropertyChanged("BaseCombined");
                    if (nodeClass.InputTypes.Length == 1)
                    {
                        this.combinedConversions = null;
                        this.OnPropertyChanged("CombinedConversions");
                    }
                    else if (nodeClass.InputTypes.Length == 0)
                    {
                        this.combinedDataSources = null;
                        this.OnPropertyChanged("CombinedDataSources");
                    }
                    else if (nodeClass.InputTypes.Length > 1)
                    {
                        this.combinedTransformations = null;
                        this.OnPropertyChanged("CombinedTransformations");
                    }
                    break;

                case NodeType.Buffer:
                    this.baseBuffers = null;
                    this.customBuffers = null;
                    this.OnPropertyChanged("BaseBuffers");
                    this.OnPropertyChanged("CustomBuffers");
                    break;
            }

            //Always update the filtered nodes.
            if(nodeClass != null)
            {
                filteredNodes = null;
                this.OnPropertyChanged("FilteredNodes");
            }

            return nodeClass;
        }

		/// <summary>
		/// Interpret and load a DataType from given name.
		/// </summary>
		/// <param name="dataTypeName"></param>
		/// <returns>Interpreted DataType instance.</returns>
		public DataType AddDataType(string dataTypeName)
		{
			if (String.IsNullOrWhiteSpace(dataTypeName))
			{
				throw new ArgumentException("String argument 'dataTypeName' must not be null or empty (incl. whitespace).", "dataTypeName");
			}

			// Create DataType instance from name if it does not already exist.
			if (!this.dataTypes.ContainsKey(dataTypeName))
			{
				return this.dataTypes[dataTypeName] = new DataType(dataTypeName);
			}

			return this.dataTypes[dataTypeName];
		}

        public DataType[] getDataTypes()
        {
            return dataTypes.Values.ToArray();
        }

        /// <summary>
        /// This reloads everything in the ClassManager.
        /// </summary>
        public void Reload()
        {
            //Clear all the classes already present:
            this.classes.Clear();

            //Clear all the GUI element-references:
            this.baseDataSources = null;
            this.baseConversions = null;
            this.baseTransformations = null;
            this.baseCombinedNodes = null;
            this.baseBuffers = null;

            this.customDataSources = null;
            this.customConversions = null;
            this.customTransformations = null;
            this.customBuffers = null;

            this.combinedDataSources = null;
            this.combinedConversions = null;
            this.combinedTransformations = null;


            //Now reload ower folders:
            LoadClasses(Path.Combine(WorkSpace.DIR, WorkSpace.BASE_DIR));
            LoadClasses(Path.Combine(WorkSpace.DIR, WorkSpace.CREATED_DIR));

            //Call prop-Changed for everything:
            this.OnPropertyChanged("BaseDataSources");
            this.OnPropertyChanged("BaseConversions");
            this.OnPropertyChanged("BaseTransformations");
            this.OnPropertyChanged("BaseCombined");
            this.OnPropertyChanged("BaseBuffers");

            this.OnPropertyChanged("CustomDataSources");
            this.OnPropertyChanged("CustomConversions");
            this.OnPropertyChanged("CustomTransformations");
            this.OnPropertyChanged("CustomBuffers");

            this.OnPropertyChanged("CombinedDataSources");
            this.OnPropertyChanged("CombinedConversions");
            this.OnPropertyChanged("CombinedTransformations");
        }


		/// <summary>
		/// Load all node classes from a directory path.
		/// </summary>
		/// <param name="path">Directory path containing metadata for node classes to load.</param>
		public void LoadClasses(string path)
		{
			if (!Directory.Exists(path)) return;

            //TEST: New Method:
            NodeLoader loader = Singleton<NodeLoader>.Instance;

            // Subdirectories are presumed to contain a node class each.
            string[] dirs = Directory.GetDirectories(path);
			foreach (string dir in dirs)
			{
                string dirName = Path.GetFileName(dir);
                try
                {
                    AbstractNodeClass loadedClass = loader.loadFromFolder(dir);
                    if (loadedClass == null) throw new Exception("Could not Load Class.... *Mumble... Mumble*");
                    AddClass(loadedClass);
                }
                catch (FileNotFoundException e)
                {
                    //This is okay, it means there is no Metadata file in the folder.
                    Debug.WriteLine(e.Message);
                }
                catch(Exception exp)
                {
                    MessageBox.Show("Could not load " + dirName + " Error: \n" + exp.Message + "\nSkipping this element.", "Error in loading " + dirName);
                }
			}
		}

        /// <summary>
        /// Gets the First node found with that type name.
        /// Node types should be unique.
        /// Returns null if none found.
        /// </summary>
        /// <param name="typeName">To search.</param>
        /// <returns>The first found node with that name, null if none found.</returns>
        public Node GetNewNodeForType(String typeName)
        {
            AbstractNodeClass node = GetNodeClassForType(typeName);
            return node == null ? null : node.generateNode();
        }

        /// <summary>
        /// Gets the First NodeClass found with that type name.
        /// Returns null if none found.
        /// </summary>
        /// <param name="typeName">To search.</param>
        /// <returns>The first found NodeClass with that name, null if none found.</returns>
        public AbstractNodeClass GetNodeClassForType(String typeName)
        {
            typeName = typeName.Replace(" ", "").Replace("_", "").ToLower();
            return this.classes.Values
                .FirstOrDefault(x => x.Name.Replace(" ", "").Replace("_", "").ToLower() == typeName);
        }

        
        /// <summary>
        /// Gets the First NodeClass found with that type name.
        /// Returns null if none found.
        /// </summary>
        /// <param name="typeName">To search.</param>
        /// <returns>The first found NodeClass with that name, null if none found.</returns>
        public AbstractNodeClass[] GetFilteredNodeClasses(String filter)
        {
            filter = filter.ToLower();
            return this.classes.Values
                .Where(x => x.Name.Replace(" ","").Replace("_","").ToLower().Contains(filter) )
                .ToArray();
        }


        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed values.</param>
        private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion methods

		#region enumerations

		/// <summary>
		/// Possible node types.
		/// </summary>
		public enum NodeType
		{
			Transformation,
			Sensor,
            Combined,
            Buffer
		}

		#endregion enumerations
	}
}