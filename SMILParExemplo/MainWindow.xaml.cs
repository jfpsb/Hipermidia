using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using SMILParExemplo.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
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
        private XmlDocument xmlDocument;
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
            xmlDocument = new XmlDocument();

            xmlDocument.Load(filename);

            if (xmlDocument.DocumentElement.Name == "smil")
            {
                XmlNodeList bodyNodeList = xmlDocument.SelectNodes("smil/body");

                //Retorna uma lista das tags dentro de body
                List<Container> lista = Parse(bodyNodeList[0].ChildNodes, null);

                // Thread do body
                Thread thread = new Thread(() =>
                {
                    //Para cada tag encontrada em body
                    foreach (Container container in lista)
                    {
                        // Inicia thread do container se possuir
                        container.Thread?.Start();

                        stackPanel.Dispatcher.Invoke(new Action(() =>
                        {
                            if (container != null)
                            {
                                stackPanel.Children.Clear();
                                stackPanel.Children.Add(container.UIElement);
                            }
                        }));

                        // Tempo de atraso usando dur
                        if (container != null)
                        {
                            Thread.Sleep(container.Dur * 1000);
                        }
                    }

                    //Limpa panel ao final
                    stackPanel.Dispatcher.Invoke(new Action(() =>
                    {
                        stackPanel.Children.Clear();
                    }));
                });

                thread.IsBackground = true;
                thread.Start();
            }
        }

        private List<Container> Parse(XmlNodeList nodeList, Panel panel)
        {
            //Lista de containers. Um Container contém um item gráfico, uma thread e a dur da tag
            List<Container> uiElementList = new List<Container>();

            //Para cada tag xml na lista
            foreach (XmlNode node in nodeList)
            {
                switch (node.Name)
                {
                    case "seq":
                        Grid grid = new Grid();

                        //Container com panel do seq atual
                        Container seqContainer = new Container();
                        //Tags contidas neste seq
                        XmlNodeList innerSeqTags = node.ChildNodes;
                        //Containers criados a partir dos nós internos do seq
                        List<Container> innerSeqContainers = Parse(innerSeqTags, grid);

                        //Guardo o panel no container
                        seqContainer.UIElement = grid;

                        //Se houver atributo dur eu leio, se não uso os durs dos nós interiores
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
                                    //Para seq dentro de seq, inicia a thread na sequência do itens
                                    container.Thread?.Start();

                                    //Coloca imagem no grid
                                    grid.Dispatcher.Invoke(new Action(() =>
                                    {
                                        if (container != null)
                                        {
                                            grid.Children.Clear();
                                            grid.Children.Add(container.UIElement);
                                        }
                                    }));

                                    //Tempo de atraso usando dur
                                    if (container != null)
                                    {
                                        Thread.Sleep(container.Dur * 1000);
                                    }
                                }

                                //Limpa panel ao final
                                grid.Dispatcher.Invoke(new Action(() =>
                                {
                                    grid.Children.Clear();
                                }));
                            });

                            //Thread encerra ao fechar telar
                            thread.IsBackground = true;
                            //Guardo thread no container
                            seqContainer.Thread = thread;
                        }

                        //Adiciono container na lista de containers
                        uiElementList.Add(seqContainer);

                        break;
                    case "par":
                        WrapPanel wrapPanel = new WrapPanel()
                        {
                            ItemHeight = 360,
                            ItemWidth = 640,
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        //Container para tag par
                        Container parContainer = new Container();
                        //Tags internas deste par
                        XmlNodeList innerParTags = node.ChildNodes;
                        //Containers criados usando tags internas como base
                        List<Container> innerParContainers = Parse(innerParTags, wrapPanel);

                        //Atribui panel no container
                        parContainer.UIElement = wrapPanel;

                        //Se a tag par possuir dur eu leio, se não pego o menor dur das tags internas
                        if (node.Attributes["dur"] != null)
                        {
                            parContainer.Dur = int.Parse(node.Attributes["dur"].Value.Replace("s", ""));
                        }
                        else
                        {
                            if (innerParContainers.Count > 0)
                            {
                                int dur = innerParContainers[0].Dur;

                                foreach (Container c in innerParContainers)
                                {
                                    if (c.Dur < dur)
                                        dur = c.Dur;
                                }

                                parContainer.Dur = dur;
                            }
                        }

                        if (innerParTags.Count > 0)
                        {
                            // Thread do par
                            Thread thread = new Thread(() =>
                            {
                                foreach (Container container in innerParContainers)
                                {
                                    // Thread de cada elemento dentro do par
                                    Thread innerThread = new Thread(() =>
                                    {
                                        //Inicia thread se container possuir
                                        container.Thread?.Start();

                                        wrapPanel.Dispatcher.Invoke(new Action(() =>
                                        {
                                            if (container != null)
                                            {
                                                wrapPanel.Children.Add(container.UIElement);
                                            }
                                        }));

                                        Thread.Sleep(container.Dur * 1000);

                                        wrapPanel.Dispatcher.Invoke(new Action(() =>
                                        {
                                            if (container != null)
                                            {
                                                wrapPanel.Children.Remove(container.UIElement);
                                            }
                                        }));
                                    });

                                    //Encerra thread se fechar janela
                                    innerThread.IsBackground = true;
                                    // Inicia thread de elemento dentro do par
                                    innerThread.Start();
                                }
                            });

                            //Encerra thread se fechar janela
                            thread.IsBackground = true;
                            //Atribui thread no container
                            parContainer.Thread = thread;
                        }

                        //Adiciona container na lista
                        uiElementList.Add(parContainer);

                        break;
                    case "img":
                        Container imgContainer = new Container();

                        //Cria elemento gráfico de imagem
                        Image image = new Image
                        {
                            Stretch = System.Windows.Media.Stretch.Uniform,
                            Source = new BitmapImage(new Uri(Path.Combine(dir, node.Attributes["src"].Value))),
                            MaxWidth = 500
                        };

                        //Atribui no container
                        imgContainer.UIElement = image;
                        //Atribui dur
                        imgContainer.Dur = int.Parse(node.Attributes["dur"].Value.Replace("s", ""));

                        //Adicionar na lista
                        uiElementList.Add(imgContainer);

                        break;
                    case "video":
                        Container videoContainer = new Container() { Dur = int.MaxValue / 1000 };

                        string videoPath = Path.Combine(dir, node.Attributes["src"].Value);

                        // Cria elemento gráfico de vídeo
                        MediaElement mediaElement = new MediaElement
                        {
                            Stretch = System.Windows.Media.Stretch.Uniform,
                            Source = new Uri(videoPath)
                        };

                        //Atribui dur se possuir
                        if (node.Attributes["dur"] != null)
                        {
                            videoContainer.Dur = int.Parse(node.Attributes["dur"].Value.Replace("s", ""));
                        }
                        else
                        {
                            // Tempo total do vídeo
                            videoContainer.Dur = GetVideoDuration(videoPath).Seconds;
                        }

                        //Atribui no container
                        videoContainer.UIElement = mediaElement;


                        uiElementList.Add(videoContainer);

                        break;
                }
            }

            return uiElementList;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (xmlDocument != null)
            {
                XmlNodeList headNodeList = xmlDocument.SelectNodes("smil/head");
                Sobre sobre = new Sobre(headNodeList[0].ChildNodes);
                sobre.ShowDialog();
            }
        }
        /// <summary>
        /// Retorna duração do vídeo
        /// </summary>
        /// <param name="filePath">Caminho do vídeo</param>
        /// <returns>TimeSpan com duração do vídeo</returns>
        private static TimeSpan GetVideoDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }
    }
}