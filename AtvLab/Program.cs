using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AtvLab
{
    class Program
    {
        static void Main(string[] args)
        {
            string textoCompleto = File.ReadAllText(@"C:\Users\jfpsb\Downloads\testCollection.dat\testCollection.dat", Encoding.UTF8).Replace("\n", "");
            string textoCollection = textoCompleto.Split(new string[] { "<collection>", "</collection>" }, StringSplitOptions.RemoveEmptyEntries)[0];
            string[] pages = textoCollection.Split(new string[] { "<page>", "</page>" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, List<Token>> tokens = new Dictionary<string, List<Token>>();

            foreach(string page in pages)
            {
                string id = page.Split(new string[] { "<id>", "</id>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string title = page.Split(new string[] { "<title>", "</title>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                string text = page.Split(new string[] { "<text>", "</text>" }, StringSplitOptions.RemoveEmptyEntries)[1];
            }

            Console.ReadLine();
        }
    }
}
