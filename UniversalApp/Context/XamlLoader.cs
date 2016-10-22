﻿namespace Yuniversal.Context
{
    using System.Reflection;
    using Windows.UI.Xaml.Controls;
    using Adapters;
    using OmniXaml;
    using OmniXaml.TypeLocation;

    public class XamlLoader
    {
        private readonly MetadataProvider metadataProvider;
        private readonly TypeDirectory directory;

        public XamlLoader()
        {
            metadataProvider = new MetadataProvider();
            directory = RegisterTypeLocation();
        }

        private TypeDirectory RegisterTypeLocation()
        {
            var typeDirectory = new TypeDirectory();

            var type = typeof(Page);
            var ass = type.GetTypeInfo().Assembly;
            typeDirectory.AddNamespace(
                XamlNamespace
                    .Map("root")
                    .With(
                        Route
                            .Assembly(ass)
                            .WithNamespaces(type.Namespace),
                        Route
                            .Assembly(typeof(OmniDataTemplate).GetTypeInfo().Assembly)
                            .WithNamespaces(typeof(OmniDataTemplate).Namespace))
            );


            typeDirectory.RegisterPrefix(new PrefixRegistration(string.Empty, "root"));

            return typeDirectory;
        }

        public object Load(string xaml)
        {

            var objectBuilder = new ExtendedObjectBuilder(new InstanceCreator(), Registrator.GetSourceValueConverter(), metadataProvider, new InstanceLifecycleSignaler());
            var cons = GetConstructionNode(xaml);
            return objectBuilder.Create(cons);
        }

        private ConstructionNode GetConstructionNode(string xaml)
        {
            var sut = new XamlToTreeParser(directory, metadataProvider, new []{new InlineParser(directory), });
            var tree = sut.Parse(xaml);
            return tree;
        }
    }
}