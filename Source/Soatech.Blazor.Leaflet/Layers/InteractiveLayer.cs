namespace Soatech.Blazor.Leaflet.Layers
{
    using Soatech.Blazor.Leaflet.Events;

    public abstract class InteractiveLayer : Layer
    {
        private bool _interactive = true;
        private bool _bubblingMouseEvents = true;

        [Parameter]
        public bool Interactive
        {
            get => _interactive;
            set => this.SetAndRaiseIfChanged(ref _interactive, value);
        }

        [Parameter]
        public bool BubblingMouseEvents
        {
            get => _bubblingMouseEvents;
            set => this.SetAndRaiseIfChanged(ref _bubblingMouseEvents, value);
        }

        #region events

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnClick { get; set; }

        [JSInvokable]
        public ValueTask NotifyClickEvent(MouseEvent e)
        {
            return OnClick?.Invoke(e) ?? ValueTask.CompletedTask;
        }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnDblClick { get; set; }

        [JSInvokable]
        public ValueTask NotifyDblClickEvent(MouseEvent e)
        {
            return OnDblClick?.Invoke(e) ?? ValueTask.CompletedTask;
        }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnMouseDown { get; set; }

        [JSInvokable]
        public ValueTask NotifyMouseDownEvent(MouseEvent e)
        {
            return OnMouseDown?.Invoke(e) ?? ValueTask.CompletedTask;
        }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnMouseUp { get; set; }

        [JSInvokable]
        public ValueTask NotifyMouseUpEvent(MouseEvent e)
        {
            return OnMouseUp?.Invoke(e) ?? ValueTask.CompletedTask;
        }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnMouseOver { get; set; }

        [JSInvokable]
        public ValueTask NotifyMouseOverEvent(MouseEvent e)
        {
            return OnMouseOver?.Invoke(e) ?? ValueTask.CompletedTask;
        }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnMouseOut { get; set; }

        [JSInvokable]
        public ValueTask NotifyMouseOutEvent(MouseEvent e)
        {
            return OnMouseOut?.Invoke(e) ?? ValueTask.CompletedTask;
        }

        [Parameter]
        public Func<MouseEvent, ValueTask>? OnContextMenu { get; set; }

        [JSInvokable]
        public ValueTask NotifyContextMenuEvent(MouseEvent e)
        {
            return OnContextMenu?.Invoke(e) ?? ValueTask.CompletedTask;
        }

        #endregion
    }
}
