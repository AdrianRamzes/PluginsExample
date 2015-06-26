using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Media;

namespace PluginInterface
{
    public interface IPlugin : IDisposable
    {
        MenuItem GetMenuItem();

        void Initialize(Canvas canvas, Color color, int thickness);

        void SetColor(Color color);

        void SetThickness(int thickness);
    }
}
