//-----------------------------------------------------------------------
// <copyright file="IocContainer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Sunlight.Framework.IoC;

    class TypeRegistry
    {
        Delegate factory;
        bool isCreated;
        bool isStatic;
        object value;

        public TypeRegistry(
            Delegate factory)
        {
            this.factory = factory;
        }

        public bool IsStatic
        {
            get { return this.isStatic; }
            set { this.isStatic = value; }
        }

        [Script(@"
            if (!this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::isStatic})
            {
                return this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::factory}(ioc);
            }

            if (!this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::isCreated})
            {
                this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::value} = this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::factory}(ioc);
                this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::isCreated} = true;
            }

            return this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::value};
            ")]
        [IgnoreGenericArguments]
        public extern T GetValue<T>(IocContainer ioc) where T : class;
    }

    public class IocContainer
    {
        StringDictionary<TypeRegistry> factoryMap = new StringDictionary<TypeRegistry>();
        StringDictionary<TypeRegistry> asyncFactoryMap = new StringDictionary<TypeRegistry>();
        int resolutionCount = 0;

        public IocHelper Register<T>(Func<T> factory) where T : class
        {
            TypeRegistry typeRegistry = new TypeRegistry(factory);
            this.factoryMap[typeof(T).TypeId] = typeRegistry;
            return new IocHelper(
                delegate
                {
                    typeRegistry.IsStatic = true;
                },
                delegate(Type type)
                {
                    this.factoryMap[type.TypeId] = typeRegistry;
                });
        }

        public IocHelper Register<T>(Func<IocContainer, T> factory) where T : class
        {
            TypeRegistry typeRegistry = new TypeRegistry(factory);
            this.factoryMap[typeof(T).TypeId] = typeRegistry;
            return new IocHelper(
                delegate
                {
                    typeRegistry.IsStatic = true;
                },
                delegate(Type type)
                {
                    this.factoryMap[type.TypeId] = typeRegistry;
                });
        }

        public IocHelper RegisterAsync<T>(Func<IocContainer, Promise<T>> asyncFactory) where T : class
        {
            Func<IocContainer, LazyAsync<T>> lazyAsyncFactory = (ioc) => new LazyAsync<T>(asyncFactory(ioc));
            TypeRegistry typeRegistry = new TypeRegistry(lazyAsyncFactory);
            this.asyncFactoryMap[typeof(T).TypeId] = typeRegistry;
            return new IocHelper(
                delegate
                {
                    typeRegistry.IsStatic = true;
                },
                delegate(Type type)
                {
                    this.asyncFactoryMap[type.TypeId] = typeRegistry;
                });
        }

        public T Resolve<T>() where T : class
        {
            var rv = this.TryResolve<T>();
            if (rv == null)
            { throw new Exception("type not registreed"); }

            return rv;
        }

        public T TryResolve<T>() where T : class
        {
            if (this.resolutionCount > 100)
            { throw new Exception("Ioc has cycles, use ResolveLazy<T> to break cycle"); }

            this.resolutionCount++;
            try
            {
                TypeRegistry typeRegistry = null;
                if (this.factoryMap.TryGetValue(typeof(T).TypeId, out typeRegistry))
                { return typeRegistry.GetValue<T>(this); }

                return null;
            }
            finally
            {
                this.resolutionCount--;
            }
        }

        public LazyAsync<T> ResolveAsyn<T>() where T : class
        {
            var rv = this.TryResolveAsync<T>();
            if (rv == null)
            { throw new Exception("type not registreed"); }

            return rv;
        }

        public LazyAsync<T> TryResolveAsync<T>()
        {
            if (this.resolutionCount > 100)
            { throw new Exception("Ioc has cycles, use ResolveLazy<T> to break cycle"); }

            this.resolutionCount++;
            try
            {
                TypeRegistry typeRegistry = null;
                if (this.factoryMap.TryGetValue(typeof(T).TypeId, out typeRegistry))
                { return typeRegistry.GetValue<LazyAsync<T>>(this); }

                return null;
            }
            finally
            {
                this.resolutionCount--;
            }
        }

        public Lazy<T> ResolveLazyOrNull<T>() where T : class
        {
            TypeRegistry typeRegistry = null;
            if (this.factoryMap.TryGetValue(typeof(T).TypeId, out typeRegistry))
            { return new Lazy<T>(() => typeRegistry.GetValue<T>(this)); }

            return null;
        }

        public Lazy<T> ResolveLazy<T>() where T : class
        {
            return new Lazy<T>(() => this.factoryMap[typeof(T).TypeId].GetValue<T>(this));
        }

        public Factory<T> ResolveFactory<T>() where T : class
        {
            return new Factory<T>(() => this.factoryMap[typeof(T).TypeId].GetValue<T>(this));
        }
    }
}
