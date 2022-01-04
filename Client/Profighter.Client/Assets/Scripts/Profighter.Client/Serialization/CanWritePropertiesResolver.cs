using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Profighter.Client.Serialization
{
    /// <summary>
    /// Used by <see cref="Newtonsoft.Json.JsonSerializer"/> to resolves a <see cref="JsonContract"/> for a given <see cref="Type"/>.
    /// Creates a <see cref="JsonProperty"/> for type property that can be written to (have a set accessor).
    /// </summary>
    internal sealed class CanWritePropertiesResolver : DefaultContractResolver
    {
        // As of 7.0.1, Json.NET suggests using a static instance for "stateless" contract resolvers, for performance reasons.
        // http://www.newtonsoft.com/json/help/html/ContractResolver.htm
        // http://www.newtonsoft.com/json/help/html/M_Newtonsoft_Json_Serialization_DefaultContractResolver__ctor.htm
        // "Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance."
        private static readonly CanWritePropertiesResolver instance;

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "Using an explicit static constructor enables lazy initialization.")]
        static CanWritePropertiesResolver()
        {
            instance = new CanWritePropertiesResolver();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="CanWritePropertiesResolver"/> class from being created.
        /// </summary>
        private CanWritePropertiesResolver()
        {
        }

        /// <summary>
        /// Gets an instance of the <see cref="CanWritePropertiesResolver"/> class.
        /// </summary>
        public static CanWritePropertiesResolver Instance => instance;

        #region [Base class Overrides]

        /// <summary>
        /// Creates a <see cref="JsonProperty"/> for the given <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="member">The member to create a <see cref="JsonProperty"/> for.</param>
        /// <param name="memberSerialization">The member's parent <see cref="MemberSerialization"/>.</param>
        /// <returns>A created <see cref="JsonProperty"/> for the given <see cref="MemberInfo"/>.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                PropertyInfo propInfo = member as PropertyInfo;
                prop.Writable = propInfo != null && propInfo.CanWrite;
            }

            return prop;
        }

        /// <summary>
        /// Creates properties for the given <see cref="JsonContract"/>.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>
        /// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>Properties for the given <see cref="JsonContract"/>.</returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            var writable = props.Where(p => p.Writable || p.ShouldSerialize != null).ToList();

            return writable;
        }

        #endregion [Base class Overrides]
    }
}
