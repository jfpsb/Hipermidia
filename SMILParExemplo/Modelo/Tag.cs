using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;

namespace SMILParExemplo.Modelo
{
    public abstract class Tag
    {
        public Grid Owner { get; set; }
        public List<Tag> Filhos { get; set; } = new List<Tag>();
        public Thread Thread { get; set; } = null;
        public int Duration { get; set; } = 0;
    }
}
