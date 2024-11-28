// Copyright (c) 2022-2024, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

// Ignore Spelling: Rebase

using SnowflakeID.Exceptions;
using SnowflakeID.Helpers;
using System;
using System.Globalization;

namespace SnowflakeID
{
    /// <summary>
    /// This class represents the Snowflake object.
    /// <seealso href="https://en.wikipedia.org/wiki/Snowflake_ID">Wikipedia article about SnowflakeId</seealso>
    /// </summary>
    /// <remarks>
    /// <para><see href="https://www.nuget.org/packages/SnowflakeIDGenerator">NuGet</see></para>
    /// <para><seealso href="https://github.com/fenase/SnowflakeIDGenerator">Source</seealso></para>
    /// <para><seealso href="https://fenase.github.io/SnowflakeIDGenerator/api/SnowflakeID.html">API</seealso></para>
    /// <para><seealso href="https://fenase.github.io/projects/SnowflakeIDGenerator">Site</seealso></para>
    /// </remarks>
    public class Snowflake : IEquatable<Snowflake>, IComparable<Snowflake>, IComparable
    {
        #region constants
        /// <summary>
        /// The maximum sequence number that can be generated per millisecond.
        /// When this value is reached, the sequence is reset to 0.
        /// </summary>
        public const long MaxSequence = 4096; // Set to 0 then this value is reached. seq % MaxSequence

        /// <summary>
        /// Max number of machines / servers allowed. Range from 0 to MaxMachineId-1
        /// </summary>
        public const long MaxMachineId = 1024; //amount. Range: [0..1024) = [0..1023]

        /// <summary>
        /// Max number of milliseconds since epoch. Range from 0 to MaxTimestamp-1
        /// </summary>
        public const long MaxTimestamp = 1L << 42; // Range: [0..4398046511103] = [0..2^42-1] = [0..4398046511104)


        private const int BITS_SHIFT_DATETIMEMILLIS = 22;
        private const int BITS_SHIFT_MACHINE = 12;
        private const int BITS_SHIFT_SEQUENCE = 0;


        private const ulong MASK_DATETIMEMILLIS_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0011_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111;
        private const ulong MASK_ESTACION_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0011_1111_1111;
        private const ulong MASK_SECUENCIA_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111;

        /// <summary>
        /// Gets the current epoch being used.
        /// </summary>
        /// <value>
        /// A <see cref="DateTime"/> representing the current epoch.
        /// </value>
        public DateTime Epoch { get; private set; }

        /// <summary>
        /// Total number of digits for the generated code.
        /// </summary>
        /// <value>
        /// An <see cref="int"/> representing the number of digits.
        /// </value>
        public static readonly int NumberOfDigits = ulong.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowflake"/> class using the default epoch (UNIX time 1-1-1970).
        /// </summary>
        public Snowflake() : this(GlobalConstants.DefaultEpoch) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowflake"/> class using a custom date as epoch.
        /// </summary>
        /// <param name="epoch">The date to use as the epoch.</param>
        public Snowflake(DateTime epoch)
        {
            this.Epoch = epoch;
        }

        /// <summary>
        /// Sets the timeStamp portion of the snowflake based on current time and selected epoch.
        /// Gets real time of the snowflake based on selected epoch.
        /// </summary>
        public DateTime UtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the machine/server number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is greater than or equal to <see cref="MaxMachineId"/>.
        /// </exception>
        [CLSCompliant(false)]
        public ulong MachineId
        {
            get => _MachineId;
            set
            {
                if (value >= MaxMachineId)
                {
                    throw new ArgumentOutOfRangeException(nameof(MachineId), $"{nameof(MachineId)} must be less than {MaxMachineId}. Got: {value}.");
                }
                _MachineId = value;
            }
        }

        /// <summary>
        /// Gets or sets the machine/server number as an integer.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is negative or greater than or equal to <see cref="MaxMachineId"/>.
        /// </exception>
        public int MachineIdInt32
        {
            get => (int)MachineId;
            set
            {
#if NET8_0_OR_GREATER
                ArgumentOutOfRangeException.ThrowIfNegative(value);
#else
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(MachineIdInt32)); }
#endif
                MachineId = (ulong)value;
            }
        }
        private ulong _MachineId;

        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is greater than or equal to <see cref="MaxSequence"/>.
        /// </exception>
        [CLSCompliant(false)]
        public ulong Sequence
        {
            get => _Sequence;
            set
            {
                if (value >= MaxSequence)
                {
                    throw new ArgumentOutOfRangeException(nameof(Sequence), $"{nameof(Sequence)} must be less than {MaxSequence}. Got: {value}.");
                }
                _Sequence = value;
            }
        }

        /// <summary>
        /// Gets or sets the sequence number as an integer.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is negative or greater than or equal to <see cref="MaxSequence"/>.
        /// </exception>
        public int SequenceInt32
        {
            get => (int)Sequence;
            set
            {
#if NET8_0_OR_GREATER
                ArgumentOutOfRangeException.ThrowIfNegative(value);
#else
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(SequenceInt32)); }
#endif
                Sequence = (ulong)value;
            }
        }
        private ulong _Sequence;

        /// <summary>
        /// Gets or sets the timestamp as the number of milliseconds since the selected epoch.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is greater than or equal to <see cref="MaxTimestamp"/>.
        /// </exception>
        [CLSCompliant(false)]
        public ulong Timestamp
        {
            get => ((ulong)UtcDateTime.Subtract(Epoch).Ticks) / TimeSpan.TicksPerMillisecond;
            set
            {
                if (value >= MaxTimestamp)
                {
                    throw new ArgumentOutOfRangeException(nameof(Timestamp), $"{nameof(Timestamp)} must be less than {MaxTimestamp}. Got: {value}.");
                }
                UtcDateTime = DateTime.SpecifyKind(Epoch.AddTicks((long)value * TimeSpan.TicksPerMillisecond), DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets or sets the timestamp as the number of milliseconds since the selected epoch.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the value is less than 0 or greater than or equal to <see cref="MaxTimestamp"/>.
        /// </exception>
        public long TimestampInt64
        {
            get => (long)Timestamp;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(TimestampInt64), $"{nameof(TimestampInt64)} must be greater than 0. Got: {value}.");
                }

                if (value >= MaxTimestamp)
                {
                    throw new ArgumentOutOfRangeException(nameof(TimestampInt64), $"{nameof(TimestampInt64)} must be less than {MaxTimestamp}. Got: {value}.");
                }
                Timestamp = (ulong)value;
            }
        }

        /// <summary>
        /// Gets the Snowflake ID.
        /// </summary>
        /// <value>
        /// A <see cref="ulong"/> representing the Snowflake ID.
        /// </value>
        [CLSCompliant(false)]
        public virtual ulong Id
        {
            get
            {
                ulong timestamp = (ulong)(UtcDateTime - Epoch).TotalMilliseconds;

                return ((timestamp & MASK_DATETIMEMILLIS_RIGHT_ALIGNED) << BITS_SHIFT_DATETIMEMILLIS)
                                        | ((MachineId & MASK_ESTACION_RIGHT_ALIGNED) << BITS_SHIFT_MACHINE)
                                        | ((Sequence & MASK_SECUENCIA_RIGHT_ALIGNED) << BITS_SHIFT_SEQUENCE);
            }
            private set
            {
                Sequence = (value >> BITS_SHIFT_SEQUENCE) & MASK_SECUENCIA_RIGHT_ALIGNED;
                MachineId = (value >> BITS_SHIFT_MACHINE) & MASK_ESTACION_RIGHT_ALIGNED;
                Timestamp = (value >> BITS_SHIFT_DATETIMEMILLIS) & MASK_DATETIMEMILLIS_RIGHT_ALIGNED;
            }
        }

        /// <summary>
        /// Gets the Snowflake ID as a string.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the Snowflake ID.
        /// </value>
        public string Code
        {
            get => Id.ToString(CultureInfo.InvariantCulture).PadLeft(NumberOfDigits, '0');
            private set => Id = ulong.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Rebase the Snowflake to a new epoch CHANGING THE GENERATED CODE but keeping the same date and time.
        /// </summary>
        /// <param name="newEpoch">The new epoch to set.</param>
        public void RebaseEpoch(DateTime newEpoch) => Epoch = newEpoch;

        /// <summary>
        /// Changes the snowflake's epoch keeping the code intact.
        /// This will adjust the represented <see cref="UtcDateTime"/> to match the new epoch.
        /// </summary>
        /// <param name="newEpoch">The new epoch to set.</param>
        public void ChangeEpoch(DateTime newEpoch)
        {
            UtcDateTime += newEpoch - Epoch;
            Epoch = newEpoch;
        }

        /// <summary>
        /// Creates a SnowflakeId object from a SnowflakeId code.
        /// </summary>
        /// <param name="s">The SnowflakeId code as a string.</param>
        /// <returns>A new instance of the <see cref="Snowflake"/> class.</returns>
        public static Snowflake Parse(string s) => new() { Code = s };

        /// <summary>
        /// Creates a SnowflakeId object from a SnowflakeId code using a custom epoch.
        /// </summary>
        /// <param name="s">The SnowflakeId code as a string.</param>
        /// <param name="customEpoch">The custom date to use as the epoch.</param>
        /// <returns>A new instance of the <see cref="Snowflake"/> class.</returns>
        public static Snowflake Parse(string s, DateTime customEpoch) => new(customEpoch) { Code = s };

        /// <summary>
        /// Creates a SnowflakeId object from a SnowflakeId code.
        /// </summary>
        /// <param name="b">The SnowflakeId code as a ulong.</param>
        /// <returns>A new instance of the <see cref="Snowflake"/> class.</returns>
        [CLSCompliant(false)]
        public static Snowflake Parse(ulong b) => new() { Id = b };

        /// <summary>
        /// Creates a SnowflakeId object from a SnowflakeId code using a custom epoch.
        /// </summary>
        /// <param name="b">The SnowflakeId code as a ulong.</param>
        /// <param name="customEpoch">The custom date to use as the epoch.</param>
        /// <returns>A new instance of the <see cref="Snowflake"/> class.</returns>
        [CLSCompliant(false)]
        public static Snowflake Parse(ulong b, DateTime customEpoch) => new(customEpoch) { Id = b };

        /// <summary>
        /// Gets the Snowflake ID as a string.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> representing the Snowflake ID.
        /// </returns>
        public override string ToString() => Code;

        /// <summary>
        /// Checks equality between this Snowflake object and another object.
        /// </summary>
        /// <param name="obj">The object to compare with the current Snowflake object.</param>
        /// <returns>
        /// <c>true</c> if the specified object is equal to the current Snowflake object; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is not Snowflake other) { return false; }
            return Equals(other);
        }

        /// <summary>
        /// Checks equality between this Snowflake object and another Snowflake object.
        /// </summary>
        /// <param name="other">The Snowflake object to compare with the current Snowflake object.</param>
        /// <returns>
        /// <c>true</c> if the specified Snowflake object is equal to the current Snowflake object; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Equals(Snowflake other) =>
            other is not null && Id == other.Id && Epoch == other.Epoch;

        /// <summary>
        /// Serves as the default hash function. Override of <seealso cref="object.GetHashCode()"/>.
        /// </summary>
        /// <returns>
        /// A hash code for the current Snowflake object.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = 3;
                hc += 5 * Id.GetHashCode();
                hc += 7 * Epoch.GetHashCode();
                return hc;
            }
        }

        /// <summary>
        /// Compares the current Snowflake object with another object.
        /// </summary>
        /// <param name="obj">The object to compare with the current Snowflake object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// <list type="bullet">
        /// <item>
        /// <description>Less than zero: This object is less than the <paramref name="obj"/> parameter.</description>
        /// </item>
        /// <item>
        /// <description>Zero: This object is equal to <paramref name="obj"/>.</description>
        /// </item>
        /// <item>
        /// <description>Greater than zero: This object is greater than <paramref name="obj"/>.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <exception cref="SnowflakesUsingDifferentEpochsException">
        /// Thrown when comparing Snowflake objects generated using different epochs.
        /// </exception>
        public int CompareTo(object obj)
        {
            if (obj is not Snowflake other) { return 1; }
            return CompareTo(other);
        }

        /// <summary>
        /// Compares the current Snowflake object with another Snowflake object.
        /// </summary>
        /// <param name="other">The Snowflake object to compare with the current Snowflake object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings:
        /// <list type="bullet">
        /// <item>
        /// <description>Less than zero: This object is less than the <paramref name="other"/> parameter.</description>
        /// </item>
        /// <item>
        /// <description>Zero: This object is equal to <paramref name="other"/>.</description>
        /// </item>
        /// <item>
        /// <description>Greater than zero: This object is greater than <paramref name="other"/>.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <exception cref="SnowflakesUsingDifferentEpochsException">
        /// Thrown when comparing Snowflake objects generated using different epochs.
        /// </exception>
        public int CompareTo(Snowflake other)
        {
            if (other == null) { return 1; }
            if (Epoch != other.Epoch) { throw new SnowflakesUsingDifferentEpochsException(SnowflakesUsingDifferentEpochsException.DefaultMessage, nameof(other)); }
            return Id.CompareTo(other.Id);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Snowflake"/> are equal.
        /// </summary>
        /// <param name="s1">The first <see cref="Snowflake"/> to compare.</param>
        /// <param name="s2">The second <see cref="Snowflake"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="Snowflake"/> instances are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Snowflake s1, Snowflake s2)
        {
            if (s1 is null)
            {
                if (s2 is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return s1.Equals(s2);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Snowflake"/> are not equal.
        /// </summary>
        /// <param name="s1">The first <see cref="Snowflake"/> to compare.</param>
        /// <param name="s2">The second <see cref="Snowflake"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the two <see cref="Snowflake"/> instances are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Snowflake s1, Snowflake s2) => !(s1 == s2);

        /// <summary>
        /// Determines whether the first specified <see cref="Snowflake"/> is greater than the second specified <see cref="Snowflake"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="Snowflake"/> to compare.</param>
        /// <param name="s2">The second <see cref="Snowflake"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the first <see cref="Snowflake"/> is greater than the second <see cref="Snowflake"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="SnowflakesUsingDifferentEpochsException">
        /// Thrown when comparing <see cref="Snowflake"/> objects generated using different epochs.
        /// </exception>
        public static bool operator >(Snowflake s1, Snowflake s2) => (!(s1 is null ^ s2 is null)) && s1 != s2 && s1.CompareTo(s2) > 0;

        /// <summary>
        /// Determines whether the first specified <see cref="Snowflake"/> is less than the second specified <see cref="Snowflake"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="Snowflake"/> to compare.</param>
        /// <param name="s2">The second <see cref="Snowflake"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the first <see cref="Snowflake"/> is less than the second <see cref="Snowflake"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="SnowflakesUsingDifferentEpochsException">
        /// Thrown when comparing <see cref="Snowflake"/> objects generated using different epochs.
        /// </exception>
        public static bool operator <(Snowflake s1, Snowflake s2) => (!(s1 is null ^ s2 is null)) && s1 != s2 && s1.CompareTo(s2) < 0;

        /// <summary>
        /// Determines whether the first specified <see cref="Snowflake"/> is greater than or equal to the second specified <see cref="Snowflake"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="Snowflake"/> to compare.</param>
        /// <param name="s2">The second <see cref="Snowflake"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the first <see cref="Snowflake"/> is greater than or equal to the second <see cref="Snowflake"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="SnowflakesUsingDifferentEpochsException">
        /// Thrown when comparing <see cref="Snowflake"/> objects generated using different epochs.
        /// </exception>
        public static bool operator >=(Snowflake s1, Snowflake s2) => s1 == s2 || s1 > s2;

        /// <summary>
        /// Determines whether the first specified <see cref="Snowflake"/> is less than or equal to the second specified <see cref="Snowflake"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="Snowflake"/> to compare.</param>
        /// <param name="s2">The second <see cref="Snowflake"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the first <see cref="Snowflake"/> is less than or equal to the second <see cref="Snowflake"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="SnowflakesUsingDifferentEpochsException">
        /// Thrown when comparing <see cref="Snowflake"/> objects generated using different epochs.
        /// </exception>
        public static bool operator <=(Snowflake s1, Snowflake s2) => s1 == s2 || s1 < s2;

        #region Implicit and explicit cast operator with alternative functions. This part might be partially redundant
        /// <summary>
        /// Converts the specified string to a <see cref="Snowflake"/> instance.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns>A <see cref="Snowflake"/> instance that is equivalent to the specified string.</returns>
        public static explicit operator Snowflake(string s) => Parse(s);

        /// <summary>
        /// Creates a <see cref="Snowflake"/> instance from the specified string.
        /// </summary>
        /// <param name="s">The string representation of the <see cref="Snowflake"/>.</param>
        /// <returns>A <see cref="Snowflake"/> instance that corresponds to the specified string.</returns>
        public static Snowflake FromString(string s) => (Snowflake)s;

        /// <summary>
        /// Converts the specified unsigned long integer to a <see cref="Snowflake"/> instance.
        /// </summary>
        /// <param name="s">The unsigned long integer to convert.</param>
        /// <returns>A <see cref="Snowflake"/> instance that is equivalent to the specified unsigned long integer.</returns>
        [CLSCompliant(false)]
        public static explicit operator Snowflake(ulong s) => Parse(s);

        /// <summary>
        /// Creates a <see cref="Snowflake"/> instance from the specified unsigned long integer.
        /// </summary>
        /// <param name="s">The unsigned long integer representation of the <see cref="Snowflake"/>.</param>
        /// <returns>A <see cref="Snowflake"/> instance that corresponds to the specified unsigned long integer.</returns>
        [CLSCompliant(false)]
        public static Snowflake FromUInt64(ulong s) => (Snowflake)s;

        /// <summary>
        /// Converts the specified <see cref="Snowflake"/> instance to its string representation.
        /// </summary>
        /// <param name="s">The <see cref="Snowflake"/> instance to convert.</param>
        /// <returns>A string representation of the specified <see cref="Snowflake"/> instance.</returns>
        public static implicit operator string(Snowflake s) => s?.ToString();

        /// <summary>
        /// Converts the specified <see cref="Snowflake"/> instance to its unsigned long integer representation.
        /// </summary>
        /// <param name="s">The <see cref="Snowflake"/> instance to convert.</param>
        /// <returns>An unsigned long integer representation of the specified <see cref="Snowflake"/> instance.</returns>
        [CLSCompliant(false)]
        public static implicit operator ulong(Snowflake s) => s?.Id ?? default;

        /// <summary>
        /// Converts the current <see cref="Snowflake"/> instance to its unsigned long integer representation.
        /// </summary>
        /// <returns>An unsigned long integer representation of the current <see cref="Snowflake"/> instance.</returns>
        [CLSCompliant(false)]
        public ulong ToUInt64() => this;
        #endregion
    }
}
