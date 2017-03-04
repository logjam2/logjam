// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyDictionary.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Initializer
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;


    /// <summary>
    /// Holds strongly typed dependencies by type.  Provides a lightweight form of a service locator for components,
    /// usually scoped to a single LogWriter pipeline.
    /// </summary>
    public sealed class DependencyDictionary
	{
		/// <summary>
		/// Dictionary of dependency types to objects.
		/// </summary>
		private readonly IDictionary<Type, object> _dictionary;
		/// <summary>
		/// Set to <c>true</c> to make the object immutable when <see cref="EndInitialization()"/> is called.
		/// </summary>
		private bool _immutable;

		public DependencyDictionary()
		{
			_dictionary = new Dictionary<Type, object>();
		}

		public bool Contains<T>()
			where T : class
		{
			lock (this)
			{
				return _dictionary.ContainsKey(typeof(T));
			}
		}

		public void Add<T>(T dependency)
			where T : class
		{
			Contract.Requires<ArgumentNullException>(dependency != null);

			lock (this)
			{
				if (_immutable)
				{
					throw new LogJamSetupException("DependencyDictionary is immutable after logjam initialization completes.", this);
				}
				_dictionary.Add(typeof(T), dependency);
			}
		}

		public void Add(Type type, object dependency)
		{
			Contract.Requires<ArgumentNullException>(type != null);
			Contract.Requires<ArgumentNullException>(dependency != null);

			if (! type.IsInstanceOfType(dependency))
			{
				string errorMessage = string.Format("Dependency {0} cannot be cast to type {1}.", dependency, type.FullName);
				throw new LogJamSetupException(errorMessage, this);
			}

			lock (this)
			{
				if (_immutable)
				{
					throw new LogJamSetupException("DependencyDictionary is immutable after logwriter initialization completes.", this);
				}
				_dictionary.Add(type, dependency);
			}
		}

		public void AddIfNotDefined(Type type, object dependency)
		{
			Contract.Requires<ArgumentNullException>(type != null);
			Contract.Requires<ArgumentNullException>(dependency != null);

			lock (this)
			{
				if (! _dictionary.ContainsKey(type))
				{
					Add(type, dependency);
				}
			}
		}

		internal void EndInitialization()
		{
			_immutable = true;
		}

		public bool TryGet<T>(out T dependency)
			where T : class
		{
			lock (this)
			{
				object o;
				if (_dictionary.TryGetValue(typeof(T), out o))
				{
					dependency = o as T;
					return dependency != null;
				}
				else
				{
					dependency = null;
					return false;
				}
			}
		}

		public T Get<T>()
			where T : class
		{
			lock (this)
			{
				object o;
				if (_dictionary.TryGetValue(typeof(T), out o))
				{
					T dependency = o as T;
					if (dependency != null)
					{
						return dependency;
					}
				}

				throw new LogJamSetupException("Cannot resolve dependency type " + typeof(T).FullName, this);
			}
		}

	}

}