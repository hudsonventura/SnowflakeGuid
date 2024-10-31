// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

using System;

namespace SnowflakeID.Exceptions
{
    /// <summary>
    /// When the timestamp is out of range.
    /// </summary>
#if (NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER) && !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class TimestampOutOfRangeException : ArgumentOutOfRangeException
    {
        /// <summary>Initializes a new instance of the <see cref="TimestampOutOfRangeException" /> class.</summary>
        public TimestampOutOfRangeException() : base() { }

        /// <summary>Initializes a new instance of the <see cref="TimestampOutOfRangeException" /> class with the name of the parameter that causes this exception.</summary>
        /// <param name="paramName">The name of the parameter that causes this exception.</param>
        public TimestampOutOfRangeException(string paramName) : base(paramName)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TimestampOutOfRangeException" /> class with a specified error message and the exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public TimestampOutOfRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TimestampOutOfRangeException" /> class with the parameter name, the value of the argument, and a specified error message.</summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="actualValue">The value of the argument that causes this exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public TimestampOutOfRangeException(string paramName, object actualValue, string message) : base(paramName, actualValue, message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TimestampOutOfRangeException" /> class with the name of the parameter that causes this exception and a specified error message.</summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public TimestampOutOfRangeException(string paramName, string message) : base(paramName, message)
        {
        }



#if (NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER) && !NET8_0_OR_GREATER
        /// <summary>Initializes a new instance of the <see cref="TimestampOutOfRangeException" /> class with serialized data.</summary>
        /// <param name="serializationInfo">The object that holds the serialized object data.</param>
        /// <param name="streamingContext">The contextual information about the source or destination.</param>
        protected TimestampOutOfRangeException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
#endif
    }
}
