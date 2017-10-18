// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamePrefixTreeNode.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Util.Collections
{
	using System;
#if CODECONTRACTS
    using System.Diagnostics.Contracts;
#endif

    using LogJam.Shared.Internal;


    /// <summary>
	/// A node in a tree where each node contains the NamePrefix of all its descendents.  In other words,
	/// a namespace tree.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal abstract class NamePrefixTreeNode<T> : TreeNode<T>
		where T : NamePrefixTreeNode<T>
	{

		private readonly string _namePrefix;

		protected NamePrefixTreeNode(string namePrefix)
		{
            Arg.NotNull(namePrefix, nameof(namePrefix));

			_namePrefix = namePrefix;
		}

		/// <summary>
		/// Gets the name prefix for this <see cref="NamePrefixTreeNode"/> object.  This node's <see cref="TreeNode{T}.Children"/> begin with this node's
		/// <c>NamePrefix</c>.  This node's <c>NamePrefix</c> starts with its <see cref="TreeNode{T}.Parent"/> Tra
		/// </summary>
		/// <value>
		/// The name prefix.
		/// </value>
		public string NamePrefix
		{
			get { return _namePrefix; }
		}

		#region Overrides of TreeNode<TracerConfig>

		/// <summary>
		/// TODO The would be descendent.
		/// </summary>
		/// <param name="node">
		/// TODO The node.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public override bool WouldBeDescendent(T node)
		{
			if (NamePrefix.Length == 0)
			{
				return node.NamePrefix.Length > 0;
			}

			return (node.NamePrefix.Length > NamePrefix.Length) && node.NamePrefix.StartsWith(NamePrefix) && (node.NamePrefix[NamePrefix.Length] == '.');
		}

		public override int Compare(T x, T y)
		{
			return string.Compare(x.NamePrefix, y.NamePrefix, StringComparison.Ordinal);
		}

		#endregion

		public T Find(string namePrefix)
		{
            Arg.NotNull(namePrefix, nameof(namePrefix));

			if (namePrefix.StartsWith(NamePrefix))
			{
				if ((namePrefix.Length == NamePrefix.Length) && namePrefix.Equals(NamePrefix))
				{
					return (T) this;
				}

				// Else look for matches in children
				foreach (T child in Children)
				{
					if (namePrefix.StartsWith(child.NamePrefix))
					{ // Delegate to this child
						return child.Find(namePrefix);
					}
				}

				return null;
			}
			else
			{
				// Not a child of this - look up the tree
				if (Parent != null)
				{
					return Parent.Find(namePrefix);
				}
				else
				{
					throw new InvalidOperationException("Hit a null Parent.");
				}
			}
		}

		/// <summary>
		/// TODO The find nearest parent.
		/// </summary>
		/// <param name="name">
		/// TODO The name.
		/// </param>
		/// <returns>
		/// The <see cref="T"/>.
		/// </returns>
		public T FindNearestParentOf(string name)
		{
            Arg.NotNull(name, nameof(name));
#if CODECONTRACTS
			Contract.Ensures(Contract.Result<T>() != null);
#endif

            if (name.StartsWith(NamePrefix))
			{
				// this is nearest parent if none of Children are parents
				foreach (T child in Children)
				{
					if (name.StartsWith(child.NamePrefix))
					{ // Delegate to this child
						return child.FindNearestParentOf(name);
					}
				}
				return (T) this;
			}

			// Not a child of this - look up the tree
			if (Parent != null)
			{
				return Parent.FindNearestParentOf(name);
			}
			else
			{
				throw new InvalidOperationException("No parent found.");
			}
		}

	}
}
