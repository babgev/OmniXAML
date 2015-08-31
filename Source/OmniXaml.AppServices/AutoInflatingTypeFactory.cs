﻿namespace OmniXaml.AppServices
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    public class AutoInflatingTypeFactory : ITypeFactory
    {
        private readonly ITypeFactory coreTypeFactory;
        private readonly IInflatableTranslator inflatableTranslator;
        private readonly Func<ITypeFactory, IXamlLoader> loaderFactory;

        public virtual IEnumerable<Type> Inflatables { get; } = new Collection<Type>();

        public AutoInflatingTypeFactory(ITypeFactory coreTypeFactory,
            IInflatableTranslator inflatableTranslator,
            Func<ITypeFactory, IXamlLoader> loaderFactory)
        {
            this.coreTypeFactory = coreTypeFactory;
            this.inflatableTranslator = inflatableTranslator;
            this.loaderFactory = loaderFactory;
        }

        public object Create(Type type)
        {
            return Create(type, null);
        }

        public object Create(Type type, object[] args)
        {
            if (IsInflatable(type))
            {                
                using (var stream = inflatableTranslator.GetStream(type))
                {
                    var instance = coreTypeFactory.Create(type, args);
                    var loader = loaderFactory(this);
                    var inflated = loader.Load(stream, instance);
                    return inflated;
                }
            }

            return coreTypeFactory.Create(type, args);
        }

        private bool IsInflatable(Type type) => HasSomeInflatableThatIsCompatible(type);

        private bool HasSomeInflatableThatIsCompatible(Type type)
        {
            return Inflatables.Any(t => t.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()));
        }

        public bool CanLocate(Type type)
        {
            return coreTypeFactory.CanLocate(type);
        }

        public object Create(Uri uri)
        {
            var type = inflatableTranslator.GetTypeFor(uri);
            return Create(type);
        }
    }
}