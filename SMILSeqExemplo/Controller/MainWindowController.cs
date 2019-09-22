using SMILSeqExemplo.modelo;
using SMILSeqExemplo.Modelo;
using System.Windows.Media.Imaging;

namespace SMILSeqExemplo.Controller
{
    class MainWindowController
    {
        public SMIL Smil { get; set; }

        public MainWindowController()
        {
            Smil = new SMIL();
        }

        public void AddImagem(Imagem imagem)
        {
            Smil.Imagens.Add(imagem);
        }

        public void ResetSmil()
        {
            Smil = new SMIL();
        }
    }
}
