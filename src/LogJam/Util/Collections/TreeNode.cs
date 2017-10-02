// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeNode.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Util.Collections
{
	using System;
	using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using LogJam.Shared.Internal;


    /// <summary>
    /// A base class for an ordered tree structure containing homogeneous nodes of type <typeparamref name="T"/>.
    /// The order of the tree structure is determined both by <see cref="WouldBeDescendent"/>, <see cref="Equals"/>, and <see cref="Compare"/>.
    /// Children are maintained in sorted order using <see cref="Compare"/>.  In addition, each tree node is
    /// placed in a depth of the tree such that its parent returns <c>true</c> from <see cref="WouldBeDescendent"/> when passed the node, the node's
    /// <see cref="WouldBeDescendent"/> returns <c>true</c> when passed all it's children.
    /// </summary>
    /// <typeparam name="T">The type of all nodes within this tree.</typeparam>
    /// <remarks>This class is not threadsafe.</remarks>
#if CODECONTRACTS
	[ContractClass(typeof(TreeNodeContract<>))]
#endif
    public abstract class TreeNode<T> : IComparer<T>
		where T : TreeNode<T>
	{
#region Fields

		private readonly List<T> _children = new List<T>(3);

#endregion

#region Public Properties

		/// <summary>
		/// Gets the children.
		/// </summary>
		/// <value>
		/// TODO The children.
		/// </value>
		public IEnumerable<T> Children { get { return _children; } }

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>
		/// TODO The parent.
		/// </value>
		public T Parent { get; set; }

		/// <summary>
		/// Gets the root node of the tree.
		/// </summary>
		/// <value>
		/// The root <see cref="TreeNode{T}"/> connected to this node.
		/// </value>
		public T Root
		{
			get
			{
				// Walk upwards until we can't walk anymore.
				T temp = (T) this;
				while (temp.Parent != null)
				{
					temp = temp.Parent;
				}

				return temp;
			}
		}

#endregion

#region Public Methods and Operators

		/// <summary>
		/// Specifies whether <paramref name="node"/> should be a descendent or ancestor of this when added to the tree.
		/// </summary>
		/// <param name="node">A <see cref="TreeNode"/> to compare to <c>this</c>.</param>
		/// <returns><c>true</c> if <paramref name="node"/> would be a descendent of <c>this</c> when
		/// added to the tree.  Returns <c>false</c> if <paramref name="node"/> would be an ancestor or sibling when added to the tree.
		/// If <c>node</c> equals <c>this</c>, <c>false</c> should be returned.
		/// </returns>
		public abstract bool WouldBeDescendent(T node);

		/// <summary>
		/// Determines the correct position of <paramref name="node"/> within the tree, and inserts it there.
		/// </summary>
		/// <param name="node">The <see cref="TreeNode{T}"/> to insert.</param>
		/// <returns>The root of the tree after inserting <param name="node"/>.</returns>
		public T InsertNode(T node)
		{
            Arg.NotNull(node, nameof(node));

			if (WouldBeDescendent(node))
			{
				// Determine if node should become the parent of any of my children
				for (int i = 0; i < _children.Count; ++i)
				{
					if (node.WouldBeDescendent(_children[i]))
					{ // Child #i should be a descendent of node
						node._children.Add(_children[i]);
						_children.RemoveAt(i);
						--i; // Cancel out increment
					}
				}

				// Find the position of node within descendents
				int position = _children.BinarySearch(node, this);
				if (position >= 0)
				{
					throw new InvalidOperationException("Nodes cannot be equal - it would create a non-deterministic position within the tree.");
				}

				// Convert the bitwise complement to a positive number, to determine the insert position.
				position = ~position;
				if (position > 0)
				{ // Compare to position - 1
					if (_children[position - 1].WouldBeDescendent(node))
					{ // Make node a child of position - 1
						_children[position - 1].InsertNode(node);
						return Root;
					}
				}
				if (position < _children.Count)
				{ // Compare to position
					if (_children[position].WouldBeDescendent(node))
					{ // Make node a child of position
						_children[position].InsertNode(node);
						return Root;
					}
				}

				// Insert node as a child
				_children.Insert(position, node);
				return Root;
			}
			else
			{ // Not a descendent - push node up
				if (Parent != null)
				{ // Tell the parent to insert it
					return Parent.InsertNode(node);
				}
				else if (node.WouldBeDescendent((T) this))
				{ // Make this the child
					return node.InsertNode((T) this);
				}
				else
				{ // Don't know what to do
					throw new InvalidOperationException("Multiple (peer) root nodes not supported.");
				}
			}
		}

		public bool RemoveDescendent(T node)
		{
			if (object.ReferenceEquals(node, this))
			{
				// Can't remove root
				throw new InvalidOperationException("Can't remove root element");
			}

			// Find the child to either remove or delegate to
			foreach (T child in Children)
			{
				if (object.ReferenceEquals(node, child))
				{ // Remove the child
					return _children.Remove(child);
				}
				else if (child.WouldBeDescendent(node))
				{
					return child.RemoveDescendent(node);
				}
			}

			// Node not found
			return false;
		}

		/// <summary>
		/// Compares <paramref name="x"/> and <paramref name="y"/>, return -1 is x is smaller than y, 0 if x and y are equal, and 1 if x is greater than y.
		/// </summary>
		/// <param name="x">A tree node</param>
		/// <param name="y">A tree node</param>
		/// <returns>
		/// -1 is x is smaller than y, 0 if x and y are equal, and 1 if x is greater than y.
		/// </returns>
		public abstract int Compare(T x, T y);

#endregion

	}


#if CODECONTRACTS
	[ContractClassFor(typeof(TreeNode<>))]
	internal abstract class TreeNodeContract<T> : TreeNode<T>
		where T : TreeNodeContract<T>
	{

		public override bool WouldBeDescendent(T node)
		{
			Contract.Requires<ArgumentNullException>(node != null);

			throw new NotImplementedException();
		}

		public override int Compare(T x, T y)
		{
			throw new NotImplementedException();
		}

	}
#endif

}
