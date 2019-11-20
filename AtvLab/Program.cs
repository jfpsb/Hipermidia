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

            Dictionary<string, List<string>> index = new Dictionary<string, List<string>>();

            foreach (string page in pages)
            {
                string id = page.Split(new string[] { "<id>", "</id>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string title = page.Split(new string[] { "<title>", "</title>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                string text = page.Split(new string[] { "<text>", "</text>" }, StringSplitOptions.RemoveEmptyEntries)[1];

                string[] palavrasTitulo = title.Split(new char[] { ' ', ',', '\'', '"', '[', ']', '{', '}', '|' }, StringSplitOptions.RemoveEmptyEntries);
                string[] palavrasTexto = text.Split(new char[] { ' ', ',', '\'', '"', '[', ']', '{', '}', '|' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string pt in palavrasTitulo)
                {
                    if (pt.Length < 4) continue;

                    string palavra = pt.ToLower();

                    if (index.ContainsKey(palavra))
                    {
                        if (!index[palavra].Contains(id))
                            index[palavra].Add(id);
                    }
                    else
                    {
                        List<string> lista = new List<string>();
                        lista.Add(id);
                        index.Add(palavra, lista);
                    }
                }

                foreach (string ptxt in palavrasTexto)
                {
                    if (ptxt.Length < 4) continue;

                    string palavra = ptxt.ToLower();

                    if (index.ContainsKey(palavra))
                    {
                        if (!index[palavra].Contains(id))
                            index[palavra].Add(id);
                    }
                    else
                    {
                        List<string> lista = new List<string>();
                        lista.Add(id);
                        index.Add(palavra, lista);
                    }
                }
            }

            Console.WriteLine("DONE");

            while (true)
            {
                Console.WriteLine("Pesquise uma Palavra: ");
                string palavra = Console.ReadLine();
                palavra = palavra.ToLower();

                if (index.ContainsKey(palavra))
                {
                    index[palavra].ForEach((s) => { Console.Write("[" + s + "] "); });
                }
                else
                {
                    Console.WriteLine("Essa Palavra Não Está Indexada");
                }

                Console.WriteLine();
            }
        }
    }
}
