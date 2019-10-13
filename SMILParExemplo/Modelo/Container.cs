using System.Threading;
using System.Windows;

namespace SMILParExemplo.Modelo
{
    class Container
    {
        public UIElement UIElement { get; set; }
        public int Dur { get; set; } = 0;
        public Thread Thread { get; set; }
    }
}
