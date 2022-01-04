using System;
using System.Collections.Generic;

namespace Profighter.Client.Serialization
{
    /// <summary>
    /// Defines an interface to configure a type with a discriminator field.
    /// </summary>
    public interface IDiscriminatorOptions
    {
        /// <summary>
        /// Gets the name of the discriminator field.
        /// </summary>
        string DiscriminatorFieldName { get; }

        /// <summary>
        /// Gets the base type for discriminator.
        /// </summary>
        Type BaseType { get; }

        /// <summary>
        /// Gets a value indicating whether the discriminator should be serialized to type.
        /// </summary>
        bool SerializeDiscriminator { get; }

        /// <summary>
        /// Gets the mappings from discriminator values to types.
        /// </summary>
        IEnumerable<(string TypeName, Type Type)> GetDiscriminatedTypes();
    }
}