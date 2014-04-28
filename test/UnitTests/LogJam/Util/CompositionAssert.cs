// -----------------------------------------------------------------------
// <copyright file="CompositionAssert.cs" company="PrecisionDemand">
// Copyright (c) 2012 PrecisionDemand.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace LogJam.Trace.UnitTests.Util
{
	/// <summary>
	/// Helper class for unit testing MEF composition - copied from http://mef.codeplex.com/wikipage?title=Unit%20Testing%20Microsoft.Composition&referringTitle=Documentation
	/// </summary>
	public static class CompositionAssert
	{
		public static void CanExportSingle(CompositionContext context, Type contractType, string contractName = null, IDictionary<string, object> metadataConstraints = null)
		{
			var lazyType = typeof(Lazy<>).MakeGenericType(contractType);
			var lazyContract = new CompositionContract(lazyType, contractName, metadataConstraints);
			context.GetExport(lazyContract);
		}

		public static void CanExportSingle<TContract>(CompositionContext context, string contractName = null, IDictionary<string, object> metadataConstraints = null)
		{
			CanExportSingle(context, typeof(TContract), contractName, metadataConstraints);
		}
	}

}