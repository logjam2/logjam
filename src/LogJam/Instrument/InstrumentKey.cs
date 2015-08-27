// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstrumentKey.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument
{
	using System;
	using System.Diagnostics.Contracts;


	/// <summary>
	/// Immutable key that uniquely identifies <see cref="IInstrument"/> objects.
	/// </summary>
	public sealed class InstrumentKey : IEquatable<InstrumentKey>, IComparable<InstrumentKey>
	{

		private readonly string _className, _instanceId;

		public InstrumentKey(string className, string instanceId = null)
		{
			Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(className));
			if ((instanceId != null) && (instanceId.Length == 0))
			{
				instanceId = null;
			}

			_className = className;
			_instanceId = instanceId;
		}

		/// <summary>
		/// A class name for the <see cref="IInstrument"/>.  This is usually the full name of the class that is being tracked by the instrument.
		/// </summary>
		//[NotNull]
		public string ClassName
		{
			get
			{
				Contract.Ensures(Contract.Result<string>() != null);

				return _className;
			}
		}

		/// <summary>
		/// An optional instance ID or name - use to support multiple instruments of the same type within the same <see cref="ClassName"/>. 
		/// </summary>
		public string InstanceId
		{ get { return _instanceId; } }

		#region Equality

		public bool Equals(InstrumentKey other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return string.Equals(_className, other._className) && string.Equals(_instanceId, other._instanceId);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as InstrumentKey);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_className.GetHashCode() * 397) ^ (_instanceId != null ? _instanceId.GetHashCode() : 0);
			}
		}

		#endregion

		/// <summary>
		/// Order by <see cref="ClassName"/>, then <see cref="InstanceId"/>.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(InstrumentKey other)
		{
			if (other == null)
			{
				return int.MinValue;
			}

			int classNameCompareResult = string.Compare(_className, other._className, StringComparison.InvariantCultureIgnoreCase);
			if (classNameCompareResult == 0)
			{
				return string.Compare(_instanceId, other._instanceId, StringComparison.InvariantCultureIgnoreCase);
			}
			else
			{
				return classNameCompareResult;
			}
		}

		public override string ToString()
		{
			if (_instanceId == null)
			{
				return _className;
			}
			else
			{
				return string.Format("{0}:{1}", _className, _instanceId);
			}
		}

	}

}