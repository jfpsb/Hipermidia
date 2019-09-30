using Microsoft.Win32;
using SMILSeqExemplo.Controller;
using SMILSeqExemplo.Modelo;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;

namespace SMILParExemplo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dir;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CarregarSMIL_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivo XML (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                dir = Path.GetDirectoryName(openFileDialog.FileName);
                CarregaXml(openFileDialog.FileName);
            }
        }

        private void CarregaXml(string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(filename);

            if (xmlDocument.DocumentElement.Name == "smil")
            {
                XmlNodeList smilNodeList = xmlDocument.SelectNodes("smil");
                Render(smilNodeList, null);
            }
        }

        private void Render(XmlNodeList nodeList, Object container)
        {
            foreach (XmlNode node in nodeList)
            {
                switch (node.Name)
                {
                    case "smil":
                        Render(node.ChildNodes, null);
                        break;
                    case "body":
                        Render(node.ChildNodes, null);
                        break;
                    case "par":
                        int quantFilhos = node.ChildNodes.Count;
                        int numLin = quantFilhos / 2;

                        WrapPanel wrapPanel = new WrapPanel
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            ItemWidth = 600,
                            ItemHeight = 400
                        };

                        dockPanel.Children.Add(wrapPanel);

                        Render(node.ChildNodes, wrapPanel);
                        break;
                    case "img":
                        WrapPanel wrapper = (WrapPanel)container;
                        Image imageControl = new Image();
                        imageControl.Stretch = System.Windows.Media.Stretch.Fill;
                        Imagem imagem = new Imagem
                        {
                            BitmapImage = new BitmapImage(new Uri(Path.Combine(dir, node.Attributes["src"].Value))),
                            Delay = int.Parse(node.Attributes["dur"].Value.Replace("s", ""))
                        };
                        imageControl.Source = imagem.BitmapImage;

                        Thread thread = new Thread(() => AdicionarImagem(wrapper, imageControl, imagem));
                        thread.Start();

                        break;
                    default:
                        break;
                }
            }
        }

        public void AdicionarImagem(WrapPanel wrapPanel, Image image, Imagem imagem)
        {
            wrapPanel.Dispatcher.BeginInvoke(new Action(() =>
            {
                wrapPanel.Children.Add(image);
            }));

            if (imagem.Delay != 0)
            {
                Thread.Sleep(imagem.Delay * 1000);

                wrapPanel.Dispatcher.BeginInvoke(new Action(() =>
                {
                    wrapPanel.Children.Remove(image);
                }));
            }
        }
    }
}
