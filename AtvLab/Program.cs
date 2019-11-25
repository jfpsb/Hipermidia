using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AtvLab
{
    class Program
    {
        static void Main(string[] args)
        {
            string textoCompleto = File.ReadAllText(@"C:\Users\jfpsb\Downloads\testCollection.dat\testCollection.dat", Encoding.UTF8).Replace("\n", "");
            string textoCollection = textoCompleto.Split(new string[] { "<collection>", "</collection>" }, StringSplitOptions.RemoveEmptyEntries)[0];
            string[] pages = textoCollection.Split(new string[] { "<page>", "</page>" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, Dictionary<string, Pagina>> index = new Dictionary<string, Dictionary<string, Pagina>>();

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
                        Dictionary<string, Pagina> paginas = index[palavra];

                        if (paginas.ContainsKey(id))
                        {
                            paginas[id].Pontos += 10;
                        }
                        else
                        {
                            Pagina p = new Pagina() { Id = id, Pontos = 10 };
                            paginas.Add(id, p);
                        }
                    }
                    else
                    {
                        Dictionary<string, Pagina> paginas = new Dictionary<string, Pagina>();
                        Pagina p = new Pagina() { Id = id, Pontos = 10 };
                        paginas.Add(id, p);
                        index.Add(palavra, paginas);
                    }
                }

                foreach (string ptxt in palavrasTexto)
                {
                    if (ptxt.Length < 4) continue;

                    string palavra = ptxt.ToLower();

                    if (index.ContainsKey(palavra))
                    {
                        Dictionary<string, Pagina> paginas = index[palavra];

                        if (paginas.ContainsKey(id))
                        {
                            paginas[id].AddPontoTexto();
                        }
                        else
                        {
                            Pagina p = new Pagina() { Id = id };
                            p.AddPontoTexto();
                            paginas.Add(id, p);
                        }
                    }
                    else
                    {
                        Dictionary<string, Pagina> paginas = new Dictionary<string, Pagina>();
                        Pagina p = new Pagina() { Id = id };
                        p.AddPontoTexto();
                        paginas.Add(id, p);
                        index.Add(palavra, paginas);
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
                    List<Pagina> lista = index[palavra].Values.ToList();
                    lista.Sort(new Pagina());
                    foreach (var pagina in lista)
                    {
                        string output = "[Página: {0}, Pontos: {1}]";
                        Console.WriteLine(string.Format(output, pagina.Id, pagina.Pontos));
                    }
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
