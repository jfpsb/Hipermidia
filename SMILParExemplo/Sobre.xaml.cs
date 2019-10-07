using System;
using System.Collections.Generic;
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
using System.Xml;

namespace SMILParExemplo
{
    /// <summary>
    /// Interaction logic for Sobre.xaml
    /// </summary>
    public partial class Sobre : Window
    {
        public Sobre()
        {
            InitializeComponent();
        }
        public Sobre(XmlNodeList headNodeList)
        {
            InitializeComponent();

            foreach (XmlNode node in headNodeList)
            {
                if (node.Name == "meta")
                {
                    string linha = node.Attributes["name"].Value + ": " + node.Attributes["content"].Value;

                    Label label = new Label();

                    label.FontSize = 16;
                    label.FontFamily = new FontFamily("Century Gothic");
                    label.Content = linha;

                    stackPanel.Children.Add(label);
                }
            }
        }
    }
}
