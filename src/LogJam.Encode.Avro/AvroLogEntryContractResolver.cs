
namespace LogJam.Encode.Avro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Hadoop.Avro;


    /// <summary>
    /// Resolves type information for log entry types for the purposes of Avro serialization.
    /// </summary>
    internal class AvroLogEntryContractResolver : AvroContractResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvroLogEntryContractResolver"/> class.
        /// </summary>
        public AvroLogEntryContractResolver() 
        {}

        /// <summary>
        /// Gets the serialization information about the type.
        /// This information is used for creation of the corresponding schema node.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>
        /// Serialization information about the type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown, if the type argument is null.</exception>
        public override TypeSerializationInfo ResolveType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            string name;
            string @namespace;
            type.GetAvroTypeName(out name, out @namespace);

            return new TypeSerializationInfo
            {
                Name = StripAvroNonCompatibleCharacters(name),
                Namespace = StripAvroNonCompatibleCharacters(@namespace),
                // ILogEntry implementations cannot be null in the log.  
                Nullable = false
            };
        }

        /// <summary>
        /// Gets the serialization information about the type members.
        /// This information is used for creation of the corresponding schema nodes.
        /// </summary>
        /// <param name="type">Type containing members which should be serialized.</param>
        /// <returns>
        /// Serialization information about the fields/properties.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown, if the type argument is null.</exception>
        public override MemberSerializationInfo[] ResolveMembers(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var fields = type
                .GetAllFields()
                .Where(f => ((f.Attributes & FieldAttributes.Public) != 0) &&
                            ((f.Attributes & FieldAttributes.Static) == 0) &&
                            !f.IgnoreDataMember());

            var properties =
                type.GetAllProperties()
                    .Where(p =>
                           p.DeclaringType.IsAnonymous() ||
                           p.DeclaringType.IsKeyValuePair() ||
                           // Note: Property setters are not required to serialize a log entry.
                           // This is because deserialization doesn't have to be to the C# type
                           (p.CanRead && p.GetGetMethod() != null && !p.IgnoreDataMember()));

            var serializedProperties = TypeExtensions.RemoveDuplicates(properties);
            return fields
                .Concat<MemberInfo>(serializedProperties)
                .Select(m => new MemberSerializationInfo { Name = m.Name, MemberInfo = m, Nullable = m.IsNullable() })
                .ToArray();
        }

        public override IEnumerable<Type> GetKnownTypes(Type type)
        {
            return base.GetKnownTypes(type);
        }

    }
}
