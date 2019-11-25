using System.Collections.Generic;

namespace AtvLab
{
    public class Pagina : IComparer<Pagina>
    {
        private int TxtPonto = 1;
        public string Id { get; set; }
        public int Pontos { get; set; } = 0;
        public void AddPontoTexto()
        {
            Pontos += TxtPonto++;
        }
        public int Compare(Pagina x, Pagina y)
        {
            if(x.Pontos == y.Pontos)
                return 0;

            if (x.Pontos < y.Pontos)
                return 1;

            return -1;
        }
    }
}
