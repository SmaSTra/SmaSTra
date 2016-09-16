using Common;
using Common.ExtensionMethods;
using SmaSTraDesigner.BusinessLogic;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static SmaSTraDesigner.BusinessLogic.ClassManager;

namespace SmaSTraDesigner.Controls.Support
{
    /// <summary>
    /// Interaktionslogik für DialogCreateCustomElement.xaml
    /// </summary>
    public partial class DialogCreateCustomElement : Window

    {
        private string elementName = "";
        private DataType outputType;
        private List<DataType> inputTypes = new List<DataType>();
        private string methodCode = "";

        private DataType[] allDataTypes = Singleton<ClassManager>.Instance.getDataTypes();
        private ObservableCollection<InputTypeViewModel> inputTypesViewModels = new ObservableCollection<InputTypeViewModel>();
        private string outputTypeString = "output type String";
        

        public DialogCreateCustomElement()
        {
            InitializeComponent();
            this.DataContext = this;
            allDataTypes = Singleton<ClassManager>.Instance.getDataTypes();
            cboxOutputTypeString.DataContext = new InputTypeViewModel() { InputTypeString = "outputType", SelectedDataType = allDataTypes[0] };
            InputTypesViewModels.Add(new InputTypeViewModel() { InputTypeString = "inputType", SelectedDataType = allDataTypes [0]});
            
            
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

        private void btnIOFinished_Click(object sender, RoutedEventArgs e)
        {
            if(ElementName.Length < 1)
            {
                tbStatus.Text = "Bitte einen Namen für das neue Element angeben.";
                return;
            }
            ClassManager classManager = Singleton<ClassManager>.Instance;
            OutputType = ((InputTypeViewModel)cboxOutputTypeString.DataContext).SelectedDataType;
            if(OutputType == null)
            {   //Check new DataType name
                if (OutputTypeString.Length < 1)
                {
                    tbStatus.Text = "Bitte einen Namen für den neuen Output Typ angeben.";
                    return;
                } else
                {   //create new DataType and update AllDataTypes[]
                    DataType newDataType = new DataType(OutputTypeString);
                    OutputType = newDataType;
                    if (!AllDataTypes.Contains(newDataType))
                    {
                        classManager.AddDataType(newDataType.Name);
                        AllDataTypes = classManager.getDataTypes();
                    }
                }
            }

            InputTypes.Clear();
            foreach(InputTypeViewModel inputTypeViewModel in InputTypesViewModels)
            {
                if (inputTypeViewModel.SelectedDataType == null)
                {   //Check new DataType name
                    if (inputTypeViewModel.InputTypeString.Length < 1)
                    {
                        tbStatus.Text = "Bitte einen Namen für den neuen Input Typ angeben.";
                        return;
                    } else
                    {   //create new DataType and update AllDataTypes[]
                        DataType newDataType = new DataType(inputTypeViewModel.InputTypeString);
                        inputTypeViewModel.SelectedDataType = newDataType;
                        if (!AllDataTypes.Contains(newDataType))
                        {
                            classManager.AddDataType(newDataType.Name);
                            AllDataTypes = classManager.getDataTypes();
                        }
                    }
                }
                InputTypes.Add(inputTypeViewModel.SelectedDataType);
            }

            if(MethodCode.Length < 1)
            {
                tbStatus.Text = "Die Methode ist leer. Bitte geben sie den Methodentext ein";
                return;
            }

            //TODO next Dialog
            DialogResult = true;
        }

        private void btnDeleteInputType_Click(object sender, RoutedEventArgs e)
        {
            InputTypeViewModel inputToDelete = (InputTypeViewModel)((Button)sender).DataContext;
            InputTypesViewModels.Remove(inputToDelete);
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

            string description = "No description";
            string mainClass = "created." + javaFreandlyName;
            string[] neededOtherClasses = new string[0];
            string[] neededPermissions = new string[0];
            ConfigElement[] config = new ConfigElement[0];
            string methodName = javaFreandlyName;
            bool isStatic = true;

            return new TransformationNodeClass(javaFreandlyName, ElementName, description, OutputType, InputTypes.ToArray(), 
                mainClass, neededOtherClasses, neededPermissions, config, 
                methodName, isStatic);
        }


        private DataSourceNodeClass GenerateClassAsDataSource()
        {
            string javaFreandlyName = ElementName.RemoveAll(" ", "_");

            string description = "No description";
            string mainClass = "created." + javaFreandlyName;
            string[] neededOtherClasses = new string[0];
            string[] neededPermissions = new string[0];
            ConfigElement[] config = new ConfigElement[0];
            string dataMethod = "getData";
            string startMethod = "start";
            string stopMethod = "stop";

            return new DataSourceNodeClass(javaFreandlyName, ElementName, description, OutputType, 
                mainClass, neededOtherClasses, neededPermissions, config,
                dataMethod, startMethod, stopMethod);
        }

    }

    public class InputTypeViewModel
    {

        public InputTypeViewModel()
        {
            DataType[] allDataTypes = Singleton<ClassManager>.Instance.getDataTypes();
            allTypesString = new string[allDataTypes.Length + 1];
            for (int i = 1; i <= allDataTypes.Length; i++)
            {
                allTypesString[i] = allDataTypes[i - 1].Name;
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
                if (inputTypeString != value)
                {
                    inputTypeString = value;
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
