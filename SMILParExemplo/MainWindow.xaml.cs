using Microsoft.Win32;
using SMILSeqExemplo.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
                Parse(smilNodeList, null);
            }
        }

        private void Parse(XmlNodeList nodeList, Object container)
        {
            foreach (XmlNode node in nodeList)
            {
                switch (node.Name)
                {
                    case "smil":
                        Parse(node.ChildNodes, null);
                        break;
                    case "body":
                        Parse(node.ChildNodes, null);
                        break;
                    case "seq":
                        XmlNodeList tags = node.ChildNodes;
                        List<int> durs = new List<int>();

                        if (tags.Count > 0)
                        {
                            int dur = 0;

                            if (node.Attributes.Count > 0)
                            {
                                dur = int.Parse(node.Attributes["dur"].Value.Replace("s", ""));
                                durs.Add(dur);
                            }

                            WrapPanel gridSeqPanel = new WrapPanel
                            {
                                HorizontalAlignment = HorizontalAlignment.Center
                            };

                            stackPanel.Children.Add(gridSeqPanel);

                            Parse(tags, gridSeqPanel);

                            Thread t = new Thread(() =>
                            {
                                while(true)
                                {
                                    foreach (FrameworkElement f in gridSeqPanel.Children)
                                    {

                                    }
                                }
                            });
                            t.Start();
                        }

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

                        Parse(node.ChildNodes, wrapPanel);
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

                        AdicionarImagem(wrapper, imageControl, imagem);

                        break;
                    default:
                        break;
                }
            }
        }

        public void AdicionarImagem(WrapPanel grid, Image image, Imagem imagem)
        {
            grid.Dispatcher.BeginInvoke(new Action(() =>
            {

            }));

            grid.Children.Add(image);
        }
    }
}
