//-----------------------------------------------------------------------
// <copyright file="LazyAsync.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for LazyAsync
    /// </summary>
    public class LazyAsync<T>
    {
        private List<Action<T>> _resolveCbList = new List<Action<T>>();
        private List<Action<object>> _errorCbList = new List<Action<object>>();
        private readonly Promise<T> _lazyPromise;
        private T _value;
        private object _error;
        private bool _initialized = false;

        public LazyAsync(Promise<T> promise)
        {
            _lazyPromise = promise
                .Then((v) =>
                {
                    _value = v;
                    _initialized = true;

                    var resolveListBak = _resolveCbList;
                    _resolveCbList = null;
                    _errorCbList = null;

                    for (int iResolve = 0; iResolve < resolveListBak.Count; iResolve++)
                    { resolveListBak[iResolve](v); }

                    return v;
                })
                .Catch<object>((e) =>
                {
                    _error = e;
                    _initialized = true;

                    var rejectCbList = _errorCbList;
                    _resolveCbList = null;
                    _errorCbList = null;
                    for (int iReject = 0; iReject < rejectCbList.Count; iReject++)
                    { rejectCbList[iReject](e); }
                });
        }

        public Promise<T> Value
        {
            get
            {
                return new Promise<T>((resolve, reject) =>
                    {
                        if (_initialized)
                        {
                            if (_error != null)
                            { reject(_error); }
                            else
                            { resolve(_value); }
                        }
                        else
                        {
                            _resolveCbList.Add(resolve);
                            _errorCbList.Add(reject);
                        }
                    });
            }
        }
    }
}
