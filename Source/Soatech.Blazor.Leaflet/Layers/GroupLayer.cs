namespace Soatech.Blazor.Leaflet.Layers
{
    using System.Collections.ObjectModel;

    public partial class GroupLayer
    {
        private IJSObjectReference? _nativeLayer;
        private readonly ObservableCollection<Layer> _layers = new();

        public ObservableCollection<Layer> MapLayers => _layers;

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public GroupLayer()
        {
        }

        protected override ValueTask<IJSObjectReference?> CreateNative()
        {
            return ParentMap.CreateGroupLayer(this);
        }
    }
}
