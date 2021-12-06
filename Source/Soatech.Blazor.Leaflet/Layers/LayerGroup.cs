namespace Soatech.Blazor.Leaflet.Layers
{
    using System.Collections.ObjectModel;

    public partial class LayerGroup
    {
        private readonly ObservableCollection<Layer> _layers = new();
        private string _name = "";

        public ObservableCollection<Layer> MapLayers => _layers;

        [Parameter]
        public string Name 
        {
            get => _name;
            set => SetAndRaiseIfChanged(ref _name, value);
        }

        [Parameter]
        public Action<string>? NameChanged { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override Task OnInitializedAsync()
        {
            AsyncDisposables.Add(Disposable.Create(() => MapLayers.Clear()));

            AsyncDisposables.Add(this.WhenChanged(nameof(Name))
                .Select(_ => Name)
                .Subscribe(ac => NameChanged?.Invoke(ac)));

            AsyncDisposables.Add(this.WhenChanged(nameof(Name))
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Select(_ => Name)
                .SelectMany(n => SetName(n).AsTask().ToObservable())
                .Subscribe());

            AsyncDisposables.Add(
                ParentMap.OnInitialized.Subscribe(_ =>
                {
                    AsyncDisposables.Add(
                        MapLayers.WhenCollectionChanged()
                        .Where(e => e.Action != NotifyCollectionChangedAction.Move)
                        .StartWith(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems: MapLayers, oldItems: new List<Layer>()))
                        .Select(e =>
                        (
                            OldItems: e.OldItems?.Cast<Layer>() ?? Enumerable.Empty<Layer>(),
                            NewItems: e.NewItems?.Cast<Layer>() ?? Enumerable.Empty<Layer>()
                        ))
                        .SelectMany(e =>
                            Task.WhenAll(
                                e.OldItems.Select(itm => itm.DisposeAsync().AsTask()))
                            .ToObservable()
                            .Select(_ => e))
                        .SelectMany(e =>
                            Task.WhenAll(
                                e.NewItems.Select(itm => itm.Create().AsTask()))
                            .ToObservable())
                        .Subscribe());
                }));

            return base.OnInitializedAsync();
        }

        protected override ValueTask<IJSObjectReference?> CreateNative()
        {
            return ParentMap.CreateGroupLayer(Name, this);
        }
    }
}
