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

                string[] palavrasTitulo = title.Split(new char[] { '!', ' ', ',', '\'', '"', '[', ']', '{', '}', '(', ')', '|', '@', ':', '.' }, StringSplitOptions.RemoveEmptyEntries);
                string[] palavrasTexto = text.Split(new char[] { '!', ' ', ',', '\'', '"', '[', ']', '{', '}', '(', ')', '|', '@', ':', '.' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string pt in palavrasTitulo)
                {
                    if (pt.Length < 4) continue;

                    string palavra = pt.ToLower();

                    if (index.ContainsKey(palavra))
                    {
                        Dictionary<string, Pagina> paginas = index[palavra];

                        if (paginas.ContainsKey(id))
                        {
                            paginas[id].AddPontoTitulo();
                        }
                        else
                        {
                            Pagina p = new Pagina() { Id = id };
                            p.AddPontoTitulo();
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
                            Pagina p = paginas[id];
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

            Console.WriteLine("INDEXAÇÃO COMPLETA");

            while (true)
            {
                Console.Write("Pesquise Duas Palavras: ");
                string[] palavras = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (palavras.Length != 2)
                {
                    Console.WriteLine("Você Não Digitou Duas Palavras");
                    continue;
                }

                string palavra1 = palavras[0].ToLower();
                string palavra2 = palavras[1].ToLower();

                Dictionary<string, Pagina> paginasPalavra1 = null;
                Dictionary<string, Pagina> paginasPalavra2 = null;
                Dictionary<string, Pagina> resultado = new Dictionary<string, Pagina>();

                if (index.ContainsKey(palavra1) && index.ContainsKey(palavra2))
                {
                    paginasPalavra1 = index[palavra1];
                    paginasPalavra2 = index[palavra2];

                    foreach (var p1 in paginasPalavra1)
                    {
                        if (paginasPalavra2.ContainsKey(p1.Key))
                        {
                            Pagina p2 = paginasPalavra2[p1.Key];
                            Pagina newp = new Pagina() { Id = p1.Key, Pontos = p1.Value.Pontos + p2.Pontos };
                            resultado.Add(p1.Key, newp);
                            continue;
                        }
                    }
                }
                else if (index.ContainsKey(palavra1))
                {
                    resultado = index[palavra1];
                }
                else
                {
                    resultado = index[palavra2];
                }

                Imprimir(resultado, paginasPalavra1, paginasPalavra2);
            }
        }

        private static void Imprimir(Dictionary<string, Pagina> resultado, Dictionary<string, Pagina> paginasPalavra1, Dictionary<string, Pagina> paginasPalavra2)
        {
            List<Pagina> resultadoLista = resultado.Values.ToList();
            resultadoLista.Sort(new Pagina());

            if (paginasPalavra1 != null)
            {
                List<Pagina> paginasPalavra1Lista = paginasPalavra1.Values.ToList();
                paginasPalavra1Lista.Sort(new Pagina());
                resultadoLista.AddRange(paginasPalavra1Lista);
            }

            if (paginasPalavra2 != null)
            {
                List<Pagina> paginasPalavra2Lista = paginasPalavra2.Values.ToList();
                paginasPalavra2Lista.Sort(new Pagina());
                resultadoLista.AddRange(paginasPalavra2Lista);
            }

            foreach (Pagina pagina in resultadoLista)
            {
                string output = "[Página: {0}, Pontos: {1}]";
                Console.WriteLine(string.Format(output, pagina.Id, pagina.Pontos));
            }
        }
    }
}
