using System;

namespace SnowflakeID.Exceptions
{
    /// <summary>
    /// When trying to compare Ids using different epochs
    /// </summary>
#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
    [Serializable]
#endif
    public class SnowflakesUsingDifferentEpochsException : ArgumentException
    {
        /// <summary>
        /// Constructor for SnowflakesUsingDifferentEpochsException
        /// </summary>
        public SnowflakesUsingDifferentEpochsException() : this("When comparing SnowflakeIds, both should be using the same epoch for the comparison to make sense.")
        {
        }

        /// <summary>
        /// Constructor for SnowflakesUsingDifferentEpochsException
        /// </summary>
        /// <param name="message"></param>
        public SnowflakesUsingDifferentEpochsException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor for SnowflakesUsingDifferentEpochsException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SnowflakesUsingDifferentEpochsException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
        /// <summary>
        /// Constructor for SnowflakesUsingDifferentEpochsException
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        protected SnowflakesUsingDifferentEpochsException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
#endif
    }
}
