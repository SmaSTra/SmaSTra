using Common;
using SmaSTraDesigner.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmaSTraDesigner.Controls.Support
{
    /// <summary>
    /// Interaktionslogik für DialogCreateCustomElement.xaml
    /// </summary>
    public partial class DialogCreateCustomElement : Window

    {
        private string elementName;
        private DataType outputType;
        private ObservableCollection<DataType> inputTypes = new ObservableCollection<DataType>();

        private DataType[] allDataTypes = Singleton<ClassManager>.Instance.getDataTypes();
        private ObservableCollection<InputTypeViewModel> inputTypesViewModels = new ObservableCollection<InputTypeViewModel>();
        private string outputTypeString = "output type String";
        

        public DialogCreateCustomElement()
        {
            InitializeComponent();
            this.DataContext = this;
            allDataTypes = Singleton<ClassManager>.Instance.getDataTypes();
            cboxOutputTypeString.DataContext = new InputTypeViewModel() { InputTypeString = "output type string" };
            InputTypesViewModels.Add(new InputTypeViewModel() {InputTypeString = "input type string"});
            
            
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

        public ObservableCollection<DataType> InputTypes
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

        public DataType[] AllDataTypes
        {
            get
            {
                return allDataTypes;
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
            InputTypeViewModel inputTypeViewModel = (InputTypeViewModel)((ComboBox)sender).DataContext;
            int typeIndex = ((ComboBox)sender).SelectedIndex - 1;
            if (typeIndex >= 0) {
                selectedType = allDataTypes[typeIndex];
            } else
            {
                //TODO: create new DataType instead of úsing the first
                selectedType = allDataTypes[0];
            }
            inputTypeViewModel.SelectedDataType = selectedType;
        }

        private void cboxOutputTypeString_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DataType selectedType;
            int index = ((ComboBox)sender).SelectedIndex - 1;
            if (index >= 0)
            {
                selectedType = allDataTypes[index];
            }
            else
            {
                //TODO: create new DataType instead of úsing the first
                selectedType = allDataTypes[0];
            }
            OutputType = selectedType;
        }

        private void btnAddInput_Click(object sender, RoutedEventArgs e)
        {
            InputTypesViewModels.Add(new InputTypeViewModel() { InputTypeString = "input type string" });
        }

        private void btnIOFinished_Click(object sender, RoutedEventArgs e)
        {
            InputTypes.Clear();
            foreach(InputTypeViewModel inputTypeViewModel in InputTypesViewModels)
            {
                if (inputTypeViewModel.SelectedDataType != null)
                {
                    InputTypes.Add(inputTypeViewModel.SelectedDataType);
                }
            }
            //TODO next Dialog
            DialogResult = true;
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

        private string inputTypeString;
        public string InputTypeString
        {
            get
            {
                return inputTypeString;
            }
            set
            {
                if(inputTypeString != value)
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
}
