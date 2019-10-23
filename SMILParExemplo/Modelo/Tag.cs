using System.Threading;
using System.Windows.Controls;

namespace SMILParExemplo.Modelo
{
    public abstract class Tag
    {
        public Grid Grid { get; set; }
        public Thread Thread { get; set; }
        public int Duration { get; set; }
    }
}
