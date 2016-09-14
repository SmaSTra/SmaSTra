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

        private ObservableCollection<string> inputTypesString = new ObservableCollection<string>();
        private string outputTypeString = "output type String";
        private DataType[] allDataTypes;

        public DialogCreateCustomElement()
        {
            InitializeComponent();
            this.DataContext = this;
            allDataTypes = Singleton<ClassManager>.Instance.getDataTypes();
            InputTypesString.Add("input type string");
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

        public ObservableCollection<string> InputTypesString
        {
            get
            {
                return inputTypesString;
            }
            set
            {
                if (inputTypesString != value)
                    inputTypesString = value;
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

        public DataType[] AllDataTypes
        {
            get
            {
                return allDataTypes;
            }
        }
    }
}
