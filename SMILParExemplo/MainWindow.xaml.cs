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
                Render(smilNodeList, null, 0);
            }
        }

        private async void Render(XmlNodeList nodeList, Object wrapper, int hierarquia)
        {
            foreach (XmlNode node in nodeList)
            {
                switch (node.Name)
                {
                    case "smil":
                        Render(node.ChildNodes, null, 0);
                        break;
                    case "body":
                        Render(node.ChildNodes, null, 0);
                        break;
                    case "par":
                        Grid grid = new Grid();
                        grid.HorizontalAlignment = HorizontalAlignment.Center;
                        grid.Name = "Grid" + hierarquia;

                        ColumnDefinition gridCol1 = new ColumnDefinition();
                        ColumnDefinition gridCol2 = new ColumnDefinition();
                        RowDefinition row1 = new RowDefinition();
                        RowDefinition row2 = new RowDefinition();

                        gridCol1.MaxWidth = gridCol2.MaxWidth = 500;
                        gridCol1.MinWidth = gridCol2.MinWidth = 400;
                        row1.MaxHeight = row2.MaxHeight = 400;
                        row1.MinHeight = row2.MinHeight = 200;

                        grid.ColumnDefinitions.Add(gridCol1);
                        grid.ColumnDefinitions.Add(gridCol2);
                        grid.RowDefinitions.Add(row1);
                        grid.RowDefinitions.Add(row2);

                        dockPanel.Children.Add(grid);
                        Render(node.ChildNodes, grid, hierarquia + 1);
                        break;
                    case "img":
                        Grid wrapperGrid = (Grid)wrapper;
                        Image imageControl = new Image();
                        imageControl.Stretch = System.Windows.Media.Stretch.Fill;
                        Imagem imagem = new Imagem
                        {
                            BitmapImage = new BitmapImage(new Uri(Path.Combine(dir, node.Attributes["src"].Value))),
                            Delay = int.Parse(node.Attributes["dur"].Value.Replace("s", ""))
                        };
                        imageControl.Source = imagem.BitmapImage;

                        int count = wrapperGrid.Children.Count;

                        if (count == 0)
                        {
                            Grid.SetColumn(imageControl, 0);
                            Grid.SetRow(imageControl, 0);
                        }
                        else if (count == 1)
                        {
                            Grid.SetColumn(imageControl, 1);
                            Grid.SetRow(imageControl, 0);
                        }
                        else if (count == 2)
                        {
                            Grid.SetColumn(imageControl, 0);
                            Grid.SetRow(imageControl, 1);
                        }
                        else
                        {
                            Grid.SetColumn(imageControl, 1);
                            Grid.SetRow(imageControl, 1);
                        }

                        //wrapperGrid.Children.Add(imageControl);
                        await AdicionarImagem(wrapperGrid, imageControl, imagem.Delay);

                        break;
                    default:
                        break;
                }
            }
        }

        public Task AdicionarImagem(Grid grid, Image image, int delay)
        {
            Task task = Task.Run(() =>
            {
                grid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    grid.Children.Add(image);
                }));

                Thread.Sleep(1000 * delay);

                //grid.Dispatcher.BeginInvoke(new Action(() =>
                //{
                //    grid.Children.Remove(image);
                //}));
            });

            return task;
        }
    }
}
