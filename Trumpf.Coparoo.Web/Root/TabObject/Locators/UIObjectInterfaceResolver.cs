﻿// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Exceptions;

    /// <summary>
    /// The UI object interface resolver.
    /// </summary>
    internal class UIObjectInterfaceResolver : IUIObjectInterfaceResolver
    {
        private ITabObject rootObject;
        private static Dictionary<Type, Type> resolveCache = new Dictionary<Type, Type>();
        private static Type[] controlTypesCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIObjectInterfaceResolver"/> class.
        /// </summary>
        /// <param name="rootObject">The process object.</param>
        public UIObjectInterfaceResolver(ITabObject rootObject) => this.rootObject = rootObject;

        /// <summary>
        /// Resolve the control type.
        /// </summary>
        /// <typeparam name="TControl">The control type to resolve.</typeparam>
        /// <returns>The control type with parameter-less default constructor.</returns>
        public Type Resolve<TControl>() where TControl : IControlObject => ResolveControlType<TControl>(true);

        /// <summary>
        /// Resolve the control type.
        /// </summary>
        /// <typeparam name="TControl">The control type to resolve.</typeparam>
        /// <param name="retryOnce">Whether to retry once.</param>
        /// <returns>The control type with parameter-less default constructor.</returns>
        private Type ResolveControlType<TControl>(bool retryOnce) where TControl : IControlObject
        {
            Type toResolve = typeof(TControl);
            Type result;
            if (!toResolve.IsInterface)
            {
                result = toResolve;
            }
            else if (resolveCache.TryGetValue(toResolve, out result))
            {
                // do nothing
            }
            else
            {
                controlTypesCache = controlTypesCache ?? PageTests.Locate.ControlObjectTypes.Where(e => e.IsAssignableFrom(e)).ToArray();
                Type[] matches = controlTypesCache.Where(e => toResolve.IsAssignableFrom(e)).ToArray();
                if (!matches.Any() && !retryOnce)
                {
                    throw new ControlObjectNotFoundException<TControl>();
                }
                else if (!matches.Any() && retryOnce)
                {
                    controlTypesCache = null;
                    result = ResolveControlType<TControl>(false);
                }
                else
                {
                    if (matches.Count() >= 2)
                    {
                        Trace.WriteLine($"Warning: Multiple matches found for control object interface <{toResolve.FullName}>: {string.Join(", ", matches.Select(e => e.FullName))}; continue with first match...");
                    }

                    result = matches.First();
                    resolveCache.Add(toResolve, result);
                }
            }

            return result;
        }
    }
}