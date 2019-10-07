using Microsoft.Win32;
using SMILParExemplo.Modelo;
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
                XmlNodeList bodyNodeList = xmlDocument.SelectNodes("smil/body");
                //Parse(smilNodeList, stackPanel);

                List<Container> lista = P(bodyNodeList[0].ChildNodes, null);

                foreach (Container container in lista)
                {
                    container.Thread?.Start();
                    stackPanel.Children.Add(container.UIElement);
                }
            }
        }

        private List<Container> P(XmlNodeList nodeList, Panel panel)
        {
            List<Container> uiElementList = new List<Container>();

            foreach (XmlNode node in nodeList)
            {
                switch (node.Name)
                {
                    case "seq":
                        Grid grid = new Grid();

                        Container seqContainer = new Container();
                        XmlNodeList innerSeqTags = node.ChildNodes;
                        List<Container> innerSeqContainers = P(innerSeqTags, grid);

                        seqContainer.UIElement = grid;

                        if (node.Attributes["dur"] != null)
                        {
                            seqContainer.Dur = int.Parse(node.Attributes["dur"].Value.Replace("s", ""));
                        }
                        else
                        {
                            int dur = 0;

                            foreach (Container c in innerSeqContainers)
                            {
                                dur += c.Dur;
                            }

                            seqContainer.Dur = dur;
                        }

                        if (innerSeqTags.Count > 0)
                        {
                            Thread thread = new Thread(() =>
                            {
                                foreach (Container container in innerSeqContainers)
                                {
                                    container.Thread?.Start();

                                    grid.Dispatcher.Invoke(new Action(() =>
                                    {
                                        if (container != null)
                                        {
                                            grid.Children.Clear();
                                            grid.Children.Add(container.UIElement);
                                        }
                                    }));

                                    if (container != null)
                                    {
                                        Thread.Sleep(container.Dur * 1000);
                                    }
                                }

                                grid.Dispatcher.Invoke(new Action(() =>
                                {
                                    grid.Children.Clear();
                                }));
                            });

                            thread.IsBackground = true;
                            seqContainer.Thread = thread;
                        }

                        uiElementList.Add(seqContainer);

                        break;
                    case "par":
                        WrapPanel wrapPanel = new WrapPanel()
                        {
                            ItemHeight = 500,
                            ItemWidth = 700,
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        Container parContainer = new Container();
                        XmlNodeList innerParTags = node.ChildNodes;
                        List<Container> innerParContainers = P(innerParTags, wrapPanel);

                        parContainer.UIElement = wrapPanel;

                        if (node.Attributes["dur"] != null)
                        {
                            parContainer.Dur = int.Parse(node.Attributes["dur"].Value.Replace("s", ""));
                        }
                        else
                        {
                            int dur = innerParContainers[0].Dur;

                            foreach (Container c in innerParContainers)
                            {
                                if (c.Dur < dur)
                                    dur = c.Dur;
                            }

                            parContainer.Dur = dur;
                        }

                        if (innerParTags.Count > 0)
                        {
                            Thread thread = new Thread(() =>
                            {
                                foreach (Container container in innerParContainers)
                                {
                                    container.Thread?.Start();

                                    wrapPanel.Dispatcher.Invoke(new Action(() =>
                                    {
                                        if (container != null)
                                        {
                                            wrapPanel.Children.Add(container.UIElement);
                                        }
                                    }));
                                }
                            });

                            thread.IsBackground = true;
                            parContainer.Thread = thread;
                        }

                        uiElementList.Add(parContainer);

                        break;
                    case "img":
                        Container imgContainer = new Container();

                        Image image = new Image
                        {
                            Stretch = System.Windows.Media.Stretch.Fill,
                            Source = new BitmapImage(new Uri(Path.Combine(dir, node.Attributes["src"].Value)))
                        };

                        imgContainer.UIElement = image;
                        imgContainer.Dur = int.Parse(node.Attributes["dur"].Value.Replace("s", ""));

                        uiElementList.Add(imgContainer);

                        break;
                }
            }

            return uiElementList;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Parse(XmlNodeList nodeList, Panel container)
        {
            foreach (XmlNode node in nodeList)
            {
                switch (node.Name)
                {
                    case "smil":
                        Parse(node.ChildNodes, container);
                        break;
                    case "body":
                        Parse(node.ChildNodes, stackPanel);
                        break;
                    case "seq":
                        Grid grid = new Grid()
                        {
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        XmlNodeList innerTags = node.ChildNodes;
                        List<UIElement> containers = new List<UIElement>();

                        foreach (XmlNode innerNode in innerTags)
                        {
                            if (innerNode.ChildNodes.Count > 0)
                            {
                                int count = grid.Children.Count;
                                Parse(innerNode.ChildNodes, grid);
                                containers.Add(grid.Children[count]);
                            }
                            else
                            {
                                containers.Add(Parse2(innerNode));
                            }
                        }

                        Thread thread = new Thread(() =>
                        {
                            while (true)
                            {
                                foreach (UIElement uie in containers)
                                {
                                    grid.Dispatcher.Invoke(new Action(() =>
                                    {
                                        if (uie != null)
                                        {
                                            grid.Children.Add(uie);
                                        }
                                    }));

                                    if (uie != null)
                                        Thread.Sleep(2000);
                                }

                                grid.Dispatcher.Invoke(new Action(() =>
                                {
                                    grid.Children.Clear();
                                }));
                            }
                        });

                        container.Children.Add(grid);

                        thread.IsBackground = true;
                        thread.Start();

                        break;
                    default:
                        Parse(node.ChildNodes, null);
                        break;
                }
            }
        }

        private FrameworkElement Parse2(XmlNode node)
        {
            switch (node.Name)
            {
                case "img":
                    Image imageContainer = new Image();

                    imageContainer.Stretch = System.Windows.Media.Stretch.Fill;
                    imageContainer.Source = new BitmapImage(new Uri(Path.Combine(dir, node.Attributes["src"].Value)));

                    return imageContainer;
                default:
                    return null;
            }
        }
    }
}
