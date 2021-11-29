using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soatech.Blazor.Leaflet
{
    public static class Extensions
    {
        public static IObservable<PropertyChangedEventArgs> WhenChanged(this INotifyPropertyChanged me, string propertyName)
        {
            return Observable.Create<PropertyChangedEventArgs>(o =>
            {
                void x(object? s, PropertyChangedEventArgs e) => o.OnNext(e);
                me.PropertyChanged += x;
                var result = Disposable.Create(dispose: () => me.PropertyChanged -= x);
                return result;
            }).Where(ev => ev.PropertyName == propertyName);
        }

        public static IServiceCollection AddSoatechBlazorComponents(this IServiceCollection services)
        {
            return services;
        }
    }
}
