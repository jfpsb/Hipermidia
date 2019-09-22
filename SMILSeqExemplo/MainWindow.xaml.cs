using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace SMILSeqExemplo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XmlDocument xmlDocument;

        public MainWindow()
        {
            InitializeComponent();

            xmlDocument = new XmlDocument();

            xmlDocument.Load(@"C:\Users\jfpsb\Downloads\exemplo01\slideshow.xml");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(bah, DispatcherPriority.ContextIdle, null);
        }

        private void bah()
        {
            xmlDocument = new XmlDocument();

            xmlDocument.Load(@"C:\Users\jfpsb\Downloads\exemplo01\slideshow.xml");

            if (xmlDocument.DocumentElement.Name == "smil")
            {
                XmlNodeList imgList = xmlDocument.GetElementsByTagName("img");

                foreach (XmlNode imgNode in imgList)
                {
                    if (imgNode.Name == "img")
                    {
                        imageHolder.Source = new BitmapImage(new Uri(@"C:\Users\jfpsb\Downloads\exemplo01\" + imgNode.Attributes["src"].Value));
                        Thread.Sleep(500);
                    }
                }
            }
        }
    }
}
