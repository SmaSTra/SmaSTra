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
                selectedType = allDataTypes[typeIndex];
            } else
            {
                //TODO: create new DataType instead of úsing the first
                selectedType = allDataTypes[0];
            }
            inputTypeViewModel.SelectedDataType = selectedType;
        }

        private void btnAddInput_Click(object sender, RoutedEventArgs e)
        {
            InputTypesViewModels.Add(new InputTypeViewModel() { InputTypeString = "input type string" });
        }

        private void btnIOFinished_Click(object sender, RoutedEventArgs e)
        {
            if(ElementName.Length < 1)
            {
                tbStatus.Text = "Bitte einen Namen für das neue Element angeben.";
                return;
            }

            OutputType = ((InputTypeViewModel)cboxOutputTypeString.DataContext).SelectedDataType;
            if(OutputType == null)
            {
                tbStatus.Text = "Bitte einen Output Typ für das neue Element angeben.";
                return;
            }

            InputTypes.Clear();
            foreach(InputTypeViewModel inputTypeViewModel in InputTypesViewModels)
            {
                if (inputTypeViewModel.SelectedDataType == null)
                {
                    tbStatus.Text = "Ein Input hat keinen Typ ausgewählt. Bitte wählen Sie einen Typ aus";
                    return;
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
