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
                return this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::factory}();
            }

            if (!this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::isCreated})
            {
                this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::value} = this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::factory}();
                this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::isCreated} = true;
            }

            return this.@{[Sunlight.Framework]Sunlight.Framework.TypeRegistry::value};
            ")]
        [IgnoreGenericArguments]
        public extern T GetValue<T>() where T : class;
    }

    public class IocContainer
    {
        StringDictionary<TypeRegistry> factoryMap = new StringDictionary<TypeRegistry>();
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

        public T Resolve<T>() where T : class
        {
            if (this.resolutionCount > 100)
            {
                throw new Exception("Ioc has cycles, use ResolveLazy<T> to break cycle");
            }

            this.resolutionCount++;
            try
            {
                return this.factoryMap[typeof(T).TypeId].GetValue<T>();
            }
            finally
            {
                this.resolutionCount--;
            }
        }

        public object ResolveType(Type type)
        {
            if (this.resolutionCount > 100)
            {
                throw new Exception("Ioc has cycles, use ResolveLazy<T> to break cycle");
            }


            this.resolutionCount++;
            try
            {
                return this.factoryMap[type.TypeId].GetValue<object>();
            }
            finally
            {
                this.resolutionCount--;
            }
        }

        public Lazy<T> ResolveLazy<T>() where T : class
        {
            return new Lazy<T>(this.factoryMap[typeof(T).TypeId].GetValue<T>);
        }
    }
}
