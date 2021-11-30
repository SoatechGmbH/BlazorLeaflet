namespace Soatech.Blazor.Leaflet.Layers
{
    using Microsoft.JSInterop;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class TileLayer
    {
        public ValueTask SetUrl(string url, bool? noRedraw = null)
        {
            return _nativeLayer?.InvokeVoidAsync("setUrl", url, noRedraw) ?? ValueTask.CompletedTask;
        }

    }
}
