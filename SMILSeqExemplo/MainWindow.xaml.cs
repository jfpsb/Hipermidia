using Microsoft.Win32;
using SMILSeqExemplo.Controller;
using SMILSeqExemplo.Modelo;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;

namespace SMILSeqExemplo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XmlDocument xmlDocument;
        private MainWindowController controller;
        private CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
            controller = new MainWindowController();
        }

        private void CarregarSMIL_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivo XML (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                CarregaXml(openFileDialog.FileName);
            }
        }

        private async void CarregaXml(string filename)
        {
            xmlDocument = new XmlDocument();

            xmlDocument.Load(filename);

            if (xmlDocument.DocumentElement.Name == "smil")
            {
                // Encerra task executando atualmente
                cts?.Cancel();

                XmlNodeList imgList = xmlDocument.GetElementsByTagName("img");

                controller.ResetSmil();

                foreach (XmlNode imgNode in imgList)
                {
                    if (imgNode.Name == "img")
                    {
                        Imagem imagem = new Imagem
                        {
                            BitmapImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(filename), imgNode.Attributes["src"].Value))),
                            Delay = int.Parse(imgNode.Attributes["dur"].Value.Replace("s", ""))
                        };
                        controller.AddImagem(imagem);
                    }
                }

                // Cria novo token para nova task
                cts = new CancellationTokenSource();

                try
                {
                    await PlaySlideShow(cts.Token);
                }
                catch (TaskCanceledException tce)
                {
                    // Trata exceção quando a task que foi cancelada tenta executar
                    Console.WriteLine("Slideshow foi cancelado: " + tce.Message);
                    return;
                }
            }
        }

        private Task PlaySlideShow(CancellationToken cancellationToken)
        {
            Task slideShowTask = Task.Run(() =>
            {
                while (true)
                {
                    foreach (Imagem imagem in controller.Smil.Imagens)
                    {
                        // Encerra task executando atualmente
                        if (cancellationToken.IsCancellationRequested)
                            return null;

                        //Invoke da UI Thread
                        imageHolder.Dispatcher.Invoke(new Action(() =>
                        {
                            imageHolder.Source = imagem.BitmapImage;
                        }));

                        Thread.Sleep(imagem.Delay * 1000);
                    }
                }
            }, cancellationToken);

            return slideShowTask;
        }
    }
}
