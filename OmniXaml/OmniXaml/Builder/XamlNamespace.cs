namespace OmniXaml.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Typing;

    public class XamlNamespace
    {
        public string Name { get; private set; }

        public XamlNamespace(AssemblyConfiguration assemblyConfiguration, string name)
        {
            if (assemblyConfiguration == null)
            {
                throw new ArgumentNullException(nameof(assemblyConfiguration));
            }

            var clrNamespaceAddresses = assemblyConfiguration.ClrNamespaceConfiguration.Namespaces
                .Select(ns => new ClrNamespaceAddress(assemblyConfiguration.Assembly, ns));

            Addresses = new ClrNamespaceAddressCollection(clrNamespaceAddresses);
            Name = name;            
        }

        public XamlNamespace(string name, ClrNamespaceAddressCollection addresses)
        {
            Addresses = addresses;
            Name = name;
        }

        public ClrNamespaceAddressCollection Addresses { get; private set; }

        public static ClrNamespaceConfiguration CreateMapFor(string ns)
        {
            return new ClrNamespaceConfiguration(new List<string>(), ns);
        }
    }

    public class ClrNamespaceAddressCollection : Collection<ClrNamespaceAddress>
    {
        public ClrNamespaceAddressCollection(IEnumerable<ClrNamespaceAddress> enumerable) : base(enumerable.ToList())
        {
        }

        public Type Get(string typeName)
        {
            var types = from mapping in this.Items
                        let t = mapping.Assembly.GetType(mapping.Namespace + "." + typeName)
                        where t != null
                        select t;

            return types.FirstOrDefault();
        }
    }
}