using SMILParExemplo.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SMILParExemplo.Controller
{
    public class Controller
    {
        private SMIL Smil;
        private XmlDocument xmlDocument;
        public Controller()
        {
            Smil = new SMIL();
        }
        public void CarregaXml(string caminho)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.Load(caminho);
        }
    }
}
