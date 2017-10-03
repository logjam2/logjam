// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTypeJsonConverter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using LogJam.Shared.Internal;


    /// <summary>
    /// Use KnownType Attribute to match a derived class based on the class given to the serializer.
    /// Selected class will be the first class to match all properties in the json object.
    /// </summary>
    internal sealed class ConfigTypeJsonConverter : JsonConverter
    {

        /// <summary>
        /// We have to use an inner serializer to avoid using the outer one to avoid recursion,
        /// <see cref="ConfigTypeJsonConverter.WriteJson" />.
        /// </summary>
        //private readonly JsonSerializer _innerSerializer;
        private readonly Type _baseType;

        // The types that could be instantiated when an element of Type _baseType is encountered
        private readonly Type[] _possibleTypes;
        // Lookup: For any property name, returns the types that have that property
        private readonly ILookup<string, Type> _typesWithProperty;
        // Lookup: For any JSON type hint property name + value, returns the types that have that property name + value
        private readonly ILookup<JsonTypeHintAttribute, Type> _typesWithTypeHint;
        private readonly ISet<string> _typeHintProperties;

        /// <summary>
        /// </summary>
        /// <param name="baseType">The property type that is get or set (aka the assignable type).</param>
        internal ConfigTypeJsonConverter(Type baseType) //, JsonSerializer innerSerializer)
        {
            Arg.NotNull(baseType, nameof(baseType));
            //Contract.Requires<ArgumentNullException>(innerSerializer != null);

            //_innerSerializer = innerSerializer;

            _baseType = baseType;
            // Determine possible types
            _possibleTypes = LogJamConfigTypes.GetConcreteTypesFor(baseType).ToArray();
            // Create property name => types Lookup
            _typesWithProperty = _possibleTypes.SelectMany(type => type.GetProperties().Select(property => new
                                                                                                           {
                                                                                                               type,
                                                                                                               property
                                                                                                           }))
                                               .ToLookup(a => a.property.Name, a => a.type);
            // Create TypeHint -> type lookup
            _typesWithTypeHint = _possibleTypes.SelectMany(type => JsonTypeHintAttribute.For(type).Select(jsonHint => new
                                                                                                                      {
                                                                                                                          jsonHint,
                                                                                                                          type
                                                                                                                      }))
                                               .ToLookup(a => a.jsonHint, a => a.type);
            // Create list of TypeHint properties
            _typeHintProperties = new HashSet<string>(_typesWithTypeHint.Select(grouping => grouping.Key.Property), StringComparer.OrdinalIgnoreCase);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == _baseType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (! CanConvert(objectType))
            {
                throw new InvalidOperationException("Can't convert type " + objectType + "; expected " + _baseType);
            }

            // Load JObject from stream. Turns out we're also called for null arrays of our objects,
            // so handle a null by returning one.
            var jToken = JToken.Load(reader);
            if (jToken.Type == JTokenType.Null)
            {
                return null;
            }
            if (jToken.Type != JTokenType.Object)
            {
                throw new InvalidOperationException("Json: expected " + _baseType + "; got " + jToken.Type);
            }

            JObject jObject = (JObject) jToken;

            // Narrow down the object type to 1, starting from all possible
            HashSet<Type> remainingTypes = new HashSet<Type>(_possibleTypes);
            foreach (var property in jObject)
            {
                if (remainingTypes.Count <= 1)
                {
                    break;
                }

                // Check if it's a JsonTypeHint property
                if (_typeHintProperties.Contains(property.Key)
                    && (property.Value.Type == JTokenType.String))
                {
                    var lookupTypeHint = new JsonTypeHintAttribute(property.Key, (string) property.Value);
                    var typesWithMatchingHint = _typesWithTypeHint[lookupTypeHint];
                    if (typesWithMatchingHint.Any())
                    { // Narrow down based on the type hint
                        remainingTypes.IntersectWith(typesWithMatchingHint);
                        continue;
                    }
                }

                // Narrow down types to only those with this property
                var typesWithProperty = _typesWithProperty[property.Key];
                remainingTypes.IntersectWith(typesWithProperty);
            }

            if (remainingTypes.Count == 1)
            {
                // Found the only possible type
                var target = Activator.CreateInstance(remainingTypes.First());
                serializer.Populate(jObject.CreateReader(), target);
                return target;
            }
            else if (remainingTypes.Count == 0)
            {
                throw new JsonSerializationException(string.Format("No registered subclass of {0} contains all properties in JSON {1}", _baseType, jObject));
            }
            else
            {
                // Multiple types match: if a single type is inherited by all other candidate types, then that's the one we want (it's
                // marked as serialisable and there's nothing to prompt us to deserialise as one of its subtypes instead). Otherwise,
                // we have an ambiguity: there are two or more sibling types matching the same fields, and no clear (winning) supertype.
                var commonBaseType = remainingTypes.SingleOrDefault(baseType => remainingTypes.All(childType => baseType.IsAssignableFrom(childType)));
                if (commonBaseType != null)
                {
                    var target = Activator.CreateInstance(commonBaseType);
                    serializer.Populate(jObject.CreateReader(), target);
                    return target;
                }

                throw new JsonSerializationException(string.Format("Multiple subclasses of {0} contain all properties in JSON {1} : {2}",
                                                                   _baseType,
                                                                   jObject,
                                                                   string.Join(", ", remainingTypes)));
            }
        }

        public override bool CanWrite { get { return true; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jsonTypeHint = JsonTypeHintAttribute.For(value.GetType()).FirstOrDefault();

            //JToken token;
            //using (JTokenWriter jTokenWriter = new JTokenWriter())
            //{
            //	serializer.Serialize(jTokenWriter, value);
            //	token = jTokenWriter.Token;
            //}

            //using (JTokenReader jTokenReader = new JTokenReader(token))
            //{
            //	if (jsonTypeHint != null)
            //	{
            //		JObject jo = JObject.FromObject(value);
            //		jo.AddFirst(new JProperty(jsonTypeHint.Property, jsonTypeHint.Value));
            //		writer.WriteToken(jo.CreateReader());
            //	}
            //	else
            //	{
            //		writer.WriteToken(jTokenReader);				
            //	}
            //}

            JObject jo = JObject.FromObject(value);
            if (jsonTypeHint != null)
            {
                jo.AddFirst(new JProperty(jsonTypeHint.Property, jsonTypeHint.Value));
            }
            writer.WriteToken(jo.CreateReader());
        }

    }

}
