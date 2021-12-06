global using System;
global using System.Collections.Generic;
global using System.Collections.Specialized;
global using System.ComponentModel;
global using System.Linq;
global using System.Reactive;
global using System.Reactive.Disposables;
global using System.Reactive.Linq;
global using System.Reactive.Threading.Tasks;
global using System.Threading.Tasks;

global using Microsoft.AspNetCore.Components;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.JSInterop;

namespace Soatech.Blazor.Leaflet
{
    public static class Extensions
    {
        public static IObservable<PropertyChangedEventArgs> WhenChanged(this INotifyPropertyChanged me)
        {
            return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                e => (o, x) => e(x), 
                e => me.PropertyChanged += e, 
                e => me.PropertyChanged -= e);
        }

        public static IObservable<NotifyCollectionChangedEventArgs> WhenCollectionChanged(this INotifyCollectionChanged me)
        {
            return Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                e => (o, x) => e(x), 
                e => me.CollectionChanged += e, 
                e => me.CollectionChanged -= e);
        }

        public static IObservable<PropertyChangedEventArgs> WhenChanged(this INotifyPropertyChanged me, string propertyName)
        {
            return me.WhenChanged().Where(ev => ev.PropertyName == propertyName);
        }

        public static IServiceCollection AddSoatechBlazorComponents(this IServiceCollection services)
        {
            return services;
        }


    }
}
