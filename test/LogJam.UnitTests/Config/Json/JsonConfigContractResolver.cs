// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfigContractResolver.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Json
{
    using System;
    using System.Diagnostics.Contracts;

    using Newtonsoft.Json.Serialization;


    /// <summary>
    /// Supports customizing JSON serialization using the <see cref="IContractResolver" /> extension point.
    /// </summary>
    internal sealed class JsonConfigContractResolver : IContractResolver
    {

        private readonly IContractResolver _innerContractResolver;

        /// <summary>
        /// We have to use an inner serializer to avoid using the outer one to avoid recursion,
        /// <see cref="ConfigTypeJsonConverter.WriteJson" />.
        /// </summary>
        //private readonly JsonSerializer _innerSerializer;
        public JsonConfigContractResolver(IContractResolver defaultContractResolver)
        {
            Contract.Requires<ArgumentException>(! (defaultContractResolver is JsonConfigContractResolver), "JsonConfigContractResolver should not be double-nested.");

            if (defaultContractResolver == null)
            {
                defaultContractResolver = new DefaultContractResolver(false);
            }
            _innerContractResolver = defaultContractResolver;

            //_innerSerializer = JsonSerializer.Create(new JsonSerializerSettings()
            //										 {
            //											 ContractResolver = _innerContractResolver
            //										 });
        }

        public JsonContract ResolveContract(Type type)
        {
            JsonContract contract = _innerContractResolver.ResolveContract(type);

            if ((contract is JsonObjectContract)
                && LogJamConfigTypes.IsRegisteredAssignableType(type))
            {
                contract.Converter = new ConfigTypeJsonConverter(type); //, _innerSerializer);
            }

            return contract;
        }

    }

}
