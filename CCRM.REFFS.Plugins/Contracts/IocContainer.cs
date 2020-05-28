// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IocContainer.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//   IocContainer Helper class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class IocContainer
    {
        private readonly Dictionary<Type, Func<object>> registrations =
            new Dictionary<Type, Func<object>>();

        ////public void Register<TService, TImplementation>()
        ////    where TImplementation : TService
        ////{
        ////    this.registrations.Add(
        ////                            typeof(TService),
        ////                            () => this.GetInstance<TImplementation>());
        ////}

        public void Register<TService>(Func<TService> instanceCreator)
        {
            if (this.registrations.Where(w => w.Key.Equals(typeof(TService))).FirstOrDefault().Key == null)
            {
                this.registrations.Add(typeof(TService), () => instanceCreator());
            }
        }

        ////public void RegisterSingle<TService>(TService instance)
        ////{
        ////    if (this.registrations.Where(w => w.Key.Equals(typeof(TService))).FirstOrDefault().Key == null)
        ////    {
        ////        this.registrations.Add(typeof(TService), () => instance);
        ////    }
        ////}

        public void RegisterSingle<TService>(Func<TService> instanceCreator)
        {
            Lazy<TService> lazy = new Lazy<TService>(instanceCreator);
            this.Register<TService>(() => lazy.Value);
        }

        ////public TService GetInstance<TService>()
        ////{
        ////    return (TService)this.GetInstance(typeof(TService));
        ////}

        ////public object GetInstance(Type serviceType)
        ////{
        ////    Func<object> creator;
        ////    if (this.registrations.TryGetValue(serviceType, out creator) == false)
        ////    {
        ////        if (serviceType.IsAbstract == true)
        ////        {
        ////            throw new InvalidOperationException(
        ////            "No registration for " + serviceType);
        ////        }
        ////    }

        ////    return creator.Invoke();
        ////}
    }
}
