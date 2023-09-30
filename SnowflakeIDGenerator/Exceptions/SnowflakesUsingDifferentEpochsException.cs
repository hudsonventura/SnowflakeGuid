using System;

namespace SnowflakeID.Exceptions
{
    /// <summary>
    /// When trying to compare Ids using different epochs
    /// </summary>
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
    }
}
