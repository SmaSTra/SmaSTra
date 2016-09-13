using SmaSTraDesigner.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<DataType> inputTypes;

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


        public DialogCreateCustomElement()
        {
            InitializeComponent();
        }
    }
}
