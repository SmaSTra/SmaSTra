using Common;
using Common.ExtensionMethods;
using SmaSTraDesigner.BusinessLogic;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.config;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SmaSTraDesigner.BusinessLogic.classhandler;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses.extras;
using static SmaSTraDesigner.BusinessLogic.classhandler.ClassManager;

namespace SmaSTraDesigner.Controls.Support
{
    /// <summary>
    /// Interaktionslogik für DialogCreateCustomElement.xaml
    /// </summary>
    public partial class DialogCreateCustomElement : Window, INotifyPropertyChanged

    {
        private string elementName = "";
        private DataType outputType;
        private List<DataType> inputTypes = new List<DataType>();
        private string methodCode = "";
        private string packageName = "";
        private string description = "No description";

        private DataType[] allDataTypes = DataType.GetDataTypes();
        private ObservableCollection<InputTypeViewModel> inputTypesViewModels = new ObservableCollection<InputTypeViewModel>();
        private string outputTypeString = "OutputType";
        ClassManager classManager = Singleton<ClassManager>.Instance;

        private bool firstPage;
        public bool FirstPage
        {
            get
            {
                return firstPage;
            }
            set
            {
                firstPage = value;
                this.NotifyPropertyChanged("FirstPage");
            }
        }
        

        public DialogCreateCustomElement()
        {
            InitializeComponent();
            this.DataContext = this;
            allDataTypes = DataType.GetDataTypes();
            cboxOutputTypeString.DataContext = new InputTypeViewModel() { InputTypeString = "OutputType", SelectedDataType = allDataTypes[0] };
            InputTypesViewModels.Add(new InputTypeViewModel() { InputTypeString = "InputType", SelectedDataType = allDataTypes [0]});
            FirstPage = true;
            
        }

        public string ElementName
        {
            get
            {
                return elementName;
            }
            set
            {
                if(elementName != value)
                elementName = value;
            }
        }

        public DataType OutputType
        {
            get
            {
                return outputType;
            }
            set
            {
                if (outputType != value)
                    outputType = value;
            }
        }

        public List<DataType> InputTypes
        {
            get
            {
                return inputTypes;
            }
            set
            {
                if (inputTypes != value)
                    inputTypes = value;
            }
        }

        public string MethodCode
        {
            get
            {
                return methodCode;
            }
            set
            {
                if (methodCode != value)
                    methodCode = value;
            }
        }

        public string PackageName
        {
            get
            {
                return packageName;
            }
            set
            {
                if (packageName != value)
                    packageName = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (description != value)
                    description = value;
            }
        }

        public DataType[] AllDataTypes
        {
            get
            {
                return allDataTypes;
            }
            set
            {
                if(allDataTypes != value)
                {
                    allDataTypes = value;
                }
            }
        }

        public ObservableCollection<InputTypeViewModel> InputTypesViewModels
        {
            get
            {
                return inputTypesViewModels;
            }
            set
            {
                if (inputTypesViewModels != value)
                    inputTypesViewModels = value;
            }
        }

        public string OutputTypeString
        {
            get
            {
                return outputTypeString;
            }
            set
            {
                value = value.RemoveAll(" ", "_");
                if (outputTypeString != value)
                    outputTypeString = value;
            }
        }

        private void cboxTypesString_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DataType selectedType;
            InputTypeViewModel inputTypeViewModel = ((ComboBox)sender).DataContext as InputTypeViewModel;
            if(inputTypeViewModel == null)
            {
                return;
            }
            int typeIndex = ((ComboBox)sender).SelectedIndex - 1;
            if (typeIndex >= 0) {
                selectedType = AllDataTypes[typeIndex];
            } else
            {               
                selectedType = null;
            }
            inputTypeViewModel.SelectedDataType = selectedType;
        }

        private void btnAddInput_Click(object sender, RoutedEventArgs e)
        {
            InputTypesViewModels.Add(new InputTypeViewModel() { InputTypeString = "inputType", SelectedDataType = allDataTypes[0] });
        }

        private void btnDeleteInputType_Click(object sender, RoutedEventArgs e)
        {
            InputTypeViewModel inputToDelete = (InputTypeViewModel)((Button)sender).DataContext;
            InputTypesViewModels.Remove(inputToDelete);
        }

        private void btnIONext_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ElementName))
            {
                tbStatus.Text = "Please enter a name for the new element.";
                return;
            }
            
            if(classManager.GetNodeClassForType(ElementName) != null)
            {
                tbStatus.Text = "A Element with that name already exists.";
                return;
            }

            OutputType = ((InputTypeViewModel)cboxOutputTypeString.DataContext).SelectedDataType;
            if (OutputType == null)
            {   //Check new DataType name
                if (OutputTypeString.Length < 1 || string.IsNullOrWhiteSpace(OutputTypeString))
                {
                    tbStatus.Text = "Please enter a name for the new output type.";
                    return;
                }
                else
                {   //create new DataType and update AllDataTypes[]
                    DataType newDataType = DataType.GetCachedType(OutputTypeString);
                    OutputType = newDataType;
                    AllDataTypes = DataType.GetDataTypes();
                }
            }

            InputTypes.Clear();
            foreach (InputTypeViewModel inputTypeViewModel in InputTypesViewModels)
            {
                if (inputTypeViewModel.SelectedDataType == null)
                {   //Check new DataType name
                    if (inputTypeViewModel.InputTypeString.Length < 1 || string.IsNullOrWhiteSpace(inputTypeViewModel.InputTypeString))
                    {
                        tbStatus.Text = "Please enter a name for the new input type.";
                        return;
                    }
                    else
                    {   //create new DataType and update AllDataTypes[]
                        DataType newDataType = DataType.GetCachedType(inputTypeViewModel.InputTypeString);
                        inputTypeViewModel.SelectedDataType = newDataType;
                        AllDataTypes = DataType.GetDataTypes();
                    }
                }
                InputTypes.Add(inputTypeViewModel.SelectedDataType);
            }
            updateMethodHeader();
            FirstPage = false;
        }

        private void updateMethodHeader()
        {
            //public double getData(){
            //public static boolean CustomConversionUpdate2(DataType long arg0) {
            string methodHeader = "public";
            string staticText = "";
            string outputTypeText = "";
            string methodNameText = "";
            string argumentsText = "(";
            staticText = InputTypes.Empty() ? "" : " static";
            outputTypeText = " " + OutputType.MinimizedName;
            methodNameText = InputTypes.Empty() ? " getData" : " " + ElementName.RemoveAll(" ", "_");
            for(int i = 0; i < InputTypes.Count; i++)
            {
                if (i != 0) argumentsText += ", ";
                argumentsText += InputTypes[i].MinimizedName + " arg" + i;
            }
            argumentsText = argumentsText + ")";

            methodHeader = methodHeader + staticText + outputTypeText + methodNameText + argumentsText + " {";
            tempTbHeader.Text = methodHeader;

        }

        private void btnIOBack_Click(object sender, RoutedEventArgs e)
        {
            FirstPage = true;
        }

        private void btnIOFinished_Click(object sender, RoutedEventArgs e)
        {
            if (MethodCode.Length < 1)
            {
                tbStatus.Text = "Missing method body. Please enter method code";
                return;
            }

            if (PackageName.Length < 1)
            {
                tbStatus.Text = "Missing package declaration. Please enter a package name";
                return;
            }

            DialogResult = true;
        }
        

        public AbstractNodeClass GenerateClassFromInputs()
        {
            NodeType type = InputTypes.Empty() ?  NodeType.Sensor : NodeType.Transformation;
            switch (type)
            {
                case NodeType.Transformation: return GenerateClassAsTransformation();
                case NodeType.Sensor: return GenerateClassAsDataSource();
                case NodeType.Combined:
                default:
                    throw new ArgumentException("Can not create " + type.ToString() + " Element in the Creation GUI!");
            }
        }

        private TransformationNodeClass GenerateClassAsTransformation()
        {
            string javaFreandlyName = ElementName.RemoveAll(" ","_");

            string description = Description;
            string mainClass = PackageName + "." + javaFreandlyName;
            string creator = Environment.UserName;
            string[] neededOtherClasses = new string[0];
            INeedsExtra[] neededExtras = new INeedsExtra[0];
            ConfigElement[] config = new ConfigElement[0];
            ProxyProperty[] proxyProperties = new ProxyProperty[0];
            string methodName = javaFreandlyName;
            bool isStatic = true;

            return new TransformationNodeClass(javaFreandlyName, ElementName, description, creator, OutputType, InputTypes.ToArray(),
                mainClass, neededOtherClasses, neededExtras, config, proxyProperties, true, Path.Combine(WorkSpace.DIR, WorkSpace.CREATED_DIR, javaFreandlyName),
                methodName, isStatic);
        }


        private DataSourceNodeClass GenerateClassAsDataSource()
        {
            string javaFreandlyName = ElementName.RemoveAll(" ", "_");

            string description = Description;
            string mainClass = PackageName + "." + javaFreandlyName;
            string creator = Environment.UserName;
            string[] neededOtherClasses = new string[0];
            INeedsExtra[] neededExtras = new INeedsExtra[0];
            ConfigElement[] config = new ConfigElement[0];
            ProxyProperty[] proxyProperties = new ProxyProperty[0];
            string dataMethod = "getData";
            string startMethod = "start";
            string stopMethod = "stop";

            return new DataSourceNodeClass(javaFreandlyName, ElementName, description, creator, OutputType, 
                mainClass, neededOtherClasses, neededExtras, config, proxyProperties, true, Path.Combine(WorkSpace.DIR, WorkSpace.CREATED_DIR, javaFreandlyName),
                dataMethod, startMethod, stopMethod);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tboxNewElementName.Focus();
        }

        private void tboxNewElementName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string enteredName = ((TextBox)sender).Text;
            if (classManager.GetNodeClassForType(enteredName) != null)
            {
                tboxNewElementName.Foreground = Brushes.Red;
                tbStatus.Foreground = Brushes.Red;
                tbStatus.Text = "A Element with that name already exists.";
            }
            else
            {
                tboxNewElementName.Foreground = Brushes.Black;
                tbStatus.Foreground = Brushes.Black;
                tbStatus.Text = "";
            }
        }
    }

    public class InputTypeViewModel
    {

        public InputTypeViewModel()
        {
            DataType[] allDataTypes = DataType.GetDataTypes();
            allTypesString = new string[allDataTypes.Length + 1];
            for (int i = 1; i <= allDataTypes.Length; i++)
            {
                allTypesString[i] = allDataTypes[i - 1].MinimizedName;
            }
            allTypesString[0] = "[Add new DataType]";
        }

        private string[] allTypesString;
        public string[] AllTypesString
        {
            get
            {
                return allTypesString;
            }
        }

        //Used to create new DataType if selectedDataType == 0
        private string inputTypeString;
        public string InputTypeString
        {
            get
            {
                return inputTypeString;
            }
            set
            {
                value = value.RemoveAll(" ", "_");
                if (inputTypeString != value)
                {
                    inputTypeString = value;
                    SelectedDataType = null;
                }
            }
        }

        private DataType selectedDataType;
        public DataType SelectedDataType
        {
            get
            {
                return selectedDataType;
            }
            set
            {
                if (selectedDataType != value)
                {
                    selectedDataType = value;
                }
            }
        }
    }


    public class ConverterCountToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                if ((int)value != 0)
                {
                    Console.WriteLine("Visibility.Collapsed");
                    return Visibility.Collapsed;
                }
                else
                {
                    Console.WriteLine("Visibility.Visible");
                    return Visibility.Visible;
                }
            }
            Console.WriteLine("Visibility.Collapsed");
            return Visibility.Collapsed;        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }
    }

}
