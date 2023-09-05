using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Sample;

public class DataTemplateInclude : AvaloniaObject, IDataTemplate
{
    public static readonly StyledProperty<Uri?> SourceProperty = AvaloniaProperty.Register<DataTemplateInclude, Uri?>(
        nameof(Source));

    public DataTemplateInclude(IServiceProvider serviceProvider)
    {
        this
            .WhenAnyValue(x => x.Source)
            .Select(Maybe.From)
            .Select(maybeUri =>
            {
                return maybeUri.Map(uri =>
                {
                    var baseUri = serviceProvider.GetContextBaseUri();
                    return (DataTemplates)AvaloniaXamlLoader.Load(serviceProvider, uri!, baseUri);
                });
            })
            .BindTo(this, x => x.DataTemplates);
    }

    public Maybe<DataTemplates> DataTemplates { get; private set; }

    public Uri? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public Control? Build(object? param)
    {
        return DataTemplates
            .Bind(templates =>
            {
                return templates
                    .TryFirst(template => template.Match(param))
                    .Bind(template => Maybe.From(template.Build(param)));
            })
            .GetValueOrDefault();
    }

    public bool Match(object? data)
    {
        return DataTemplates.Match(x => x.Any(t => t.Match(data)), () => false);
    }
}