// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;

namespace SnowflakeID.Exceptions
{
    /// <summary>
    /// When trying to compare Ids using different epochs
    /// </summary>
#if (NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER) && !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class SnowflakesUsingDifferentEpochsException : ArgumentException
    {
        /// <summary>
        /// Default message
        /// </summary>
        public const string DefaultMessage = "When comparing SnowflakeIds, both should be using the same epoch for the comparison to make sense.";
        /// <summary>Initializes a new instance of the <see cref="SnowflakesUsingDifferentEpochsException" /> class.</summary>
        public SnowflakesUsingDifferentEpochsException() : this(DefaultMessage) { }

        /// <summary>Initializes a new instance of the <see cref="SnowflakesUsingDifferentEpochsException" /> class with a specified error message.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public SnowflakesUsingDifferentEpochsException(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="SnowflakesUsingDifferentEpochsException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
        public SnowflakesUsingDifferentEpochsException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="SnowflakesUsingDifferentEpochsException" /> class with a specified error message and the name of the parameter that causes this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="paramName">The name of the parameter that caused the current exception.</param>
        public SnowflakesUsingDifferentEpochsException(string message, string paramName) : base(message, paramName) { }

        /// <summary>Initializes a new instance of the <see cref="SnowflakesUsingDifferentEpochsException" /> class with a specified error message, the parameter name, and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="paramName">The name of the parameter that caused the current exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
        public SnowflakesUsingDifferentEpochsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException) { }

#if (NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER) && !NET8_0_OR_GREATER
        /// <summary>Initializes a new instance of the <see cref="SnowflakesUsingDifferentEpochsException" /> class with serialized data.</summary>
        /// <param name="serializationInfo">The object that holds the serialized object data.</param>
        /// <param name="streamingContext">The contextual information about the source or destination.</param>
        protected SnowflakesUsingDifferentEpochsException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
#endif
    }
}
