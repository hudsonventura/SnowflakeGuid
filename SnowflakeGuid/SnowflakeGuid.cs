// Copyright (c) 2022-2025, Federico Seckel.
// Licensed under the BSD 3-Clause License. See LICENSE file in the project root for full license information.

// Ignore Spelling: Rebase

using Exceptions;
using Helpers;
using System;
using System.Globalization;
using System.Text;
using System.Threading;



/// <summary>
/// This class represents the SnowflakeGuid object.
/// <seealso href="https://en.wikipedia.org/wiki/Snowflake_ID">Wikipedia article about SnowflakeId</seealso>
/// </summary>
/// <remarks>
/// <para><see href="https://www.nuget.org/packages/SnowflakeIDGenerator">NuGet</see></para>
/// <para><seealso href="https://github.com/fenase/SnowflakeIDGenerator">Source</seealso></para>
/// <para><seealso href="https://fenase.github.io/SnowflakeIDGenerator/api/SnowflakeID.html">API</seealso></para>
/// <para><seealso href="https://fenase.github.io/projects/SnowflakeIDGenerator">Site</seealso></para>
/// </remarks>
public class SnowflakeGuid : IEquatable<SnowflakeGuid>, IComparable<SnowflakeGuid>, IComparable
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
    public SnowflakeGuid() : this(GlobalConstants.DefaultEpoch) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Snowflake"/> class using a custom date as epoch.
    /// </summary>
    /// <param name="epoch">The date to use as the epoch.</param>
    public SnowflakeGuid(DateTime epoch)
    {
        this.Epoch = epoch;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Snowflake"/> class from a Guid
    /// </summary>
    /// <param name="guid"></param>
    public SnowflakeGuid(Guid guid)
    {
        Guid = guid;

        var uint64 = FromGuid(guid);

        var final = FromUInt64(uint64);

        this.Epoch = final.Epoch;
        this.UtcDateTime = final.UtcDateTime;
        this.Timestamp = final.Timestamp;
        this.MachineId = final.MachineId;
        this.Sequence = final.Sequence;
    }

    /// <summary>
    /// Sets the timeStamp portion of the SnowflakeGuid based on current time and selected epoch.
    /// Gets real time of the SnowflakeGuid based on selected epoch.
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
    /// Gets the SnowflakeGuid ID.
    /// </summary>
    /// <value>
    /// A <see cref="ulong"/> representing the SnowflakeGuid ID.
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
    /// Gets the SnowflakeGuid ID as a string.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the SnowflakeGuid ID.
    /// </value>
    public string Code
    {
        get => Id.ToString(CultureInfo.InvariantCulture).PadLeft(NumberOfDigits, '0');
        private set => Id = ulong.Parse(value, CultureInfo.InvariantCulture);
    }
    

    /// <summary>
    /// Rebase the SnowflakeGuid to a new epoch CHANGING THE GENERATED CODE but keeping the same date and time.
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
    public static SnowflakeGuid Parse(string s) => new() { Code = s };

    /// <summary>
    /// Creates a SnowflakeId object from a SnowflakeId code using a custom epoch.
    /// </summary>
    /// <param name="s">The SnowflakeId code as a string.</param>
    /// <param name="customEpoch">The custom date to use as the epoch.</param>
    /// <returns>A new instance of the <see cref="Snowflake"/> class.</returns>
    public static SnowflakeGuid Parse(string s, DateTime customEpoch) => new(customEpoch) { Code = s };

    /// <summary>
    /// Creates a SnowflakeId object from a SnowflakeId code.
    /// </summary>
    /// <param name="b">The SnowflakeId code as a ulong.</param>
    /// <returns>A new instance of the <see cref="Snowflake"/> class.</returns>
    [CLSCompliant(false)]
    public static SnowflakeGuid Parse(ulong b) => new() { Id = b };

    /// <summary>
    /// Creates a SnowflakeId object from a SnowflakeId code using a custom epoch.
    /// </summary>
    /// <param name="b">The SnowflakeId code as a ulong.</param>
    /// <param name="customEpoch">The custom date to use as the epoch.</param>
    /// <returns>A new instance of the <see cref="Snowflake"/> class.</returns>
    [CLSCompliant(false)]
    public static SnowflakeGuid Parse(ulong b, DateTime customEpoch) => new(customEpoch) { Id = b };

    /// <summary>
    /// Gets the SnowflakeGuid ID as a string.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> representing the SnowflakeGuid ID.
    /// </returns>
    public override string ToString() => Guid.ToString();

    /// <summary>
    /// Checks equality between this SnowflakeGuid object and another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current SnowflakeGuid object.</param>
    /// <returns>
    /// <c>true</c> if the specified object is equal to the current SnowflakeGuid object; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj is not SnowflakeGuid other) { return false; }
        return Equals(other);
    }

    /// <summary>
    /// Checks equality between this SnowflakeGuid object and another SnowflakeGuid object.
    /// </summary>
    /// <param name="other">The SnowflakeGuid object to compare with the current SnowflakeGuid object.</param>
    /// <returns>
    /// <c>true</c> if the specified SnowflakeGuid object is equal to the current SnowflakeGuid object; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool Equals(SnowflakeGuid other) =>
        other is not null && Id == other.Id && Epoch == other.Epoch;

    /// <summary>
    /// Serves as the default hash function. Override of <seealso cref="object.GetHashCode()"/>.
    /// </summary>
    /// <returns>
    /// A hash code for the current SnowflakeGuid object.
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
    /// Compares the current SnowflakeGuid object with another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current SnowflakeGuid object.</param>
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
    /// Thrown when comparing SnowflakeGuid objects generated using different epochs.
    /// </exception>
    public int CompareTo(object obj)
    {
        if (obj is not SnowflakeGuid other) { return 1; }
        return CompareTo(other);
    }

    /// <summary>
    /// Compares the current SnowflakeGuid object with another SnowflakeGuid object.
    /// </summary>
    /// <param name="other">The SnowflakeGuid object to compare with the current SnowflakeGuid object.</param>
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
    /// Thrown when comparing SnowflakeGuid objects generated using different epochs.
    /// </exception>
    public int CompareTo(SnowflakeGuid other)
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
    public static bool operator ==(SnowflakeGuid s1, SnowflakeGuid s2)
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
    public static bool operator !=(SnowflakeGuid s1, SnowflakeGuid s2) => !(s1 == s2);

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
    public static bool operator >(SnowflakeGuid s1, SnowflakeGuid s2) => (!(s1 is null ^ s2 is null)) && s1 != s2 && s1.CompareTo(s2) > 0;

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
    public static bool operator <(SnowflakeGuid s1, SnowflakeGuid s2) => (!(s1 is null ^ s2 is null)) && s1 != s2 && s1.CompareTo(s2) < 0;

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
    public static bool operator >=(SnowflakeGuid s1, SnowflakeGuid s2) => s1 == s2 || s1 > s2;

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
    public static bool operator <=(SnowflakeGuid s1, SnowflakeGuid s2) => s1 == s2 || s1 < s2;

    #region Implicit and explicit cast operator with alternative functions. This part might be partially redundant
    /// <summary>
    /// Converts the specified string to a <see cref="Snowflake"/> instance.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>A <see cref="Snowflake"/> instance that is equivalent to the specified string.</returns>
    public static explicit operator SnowflakeGuid(string s) => Parse(s);

    /// <summary>
    /// Creates a <see cref="Snowflake"/> instance from the specified string.
    /// </summary>
    /// <param name="s">The string representation of the <see cref="Snowflake"/>.</param>
    /// <returns>A <see cref="Snowflake"/> instance that corresponds to the specified string.</returns>
    public static SnowflakeGuid FromString(string s) => (SnowflakeGuid)s;

    /// <summary>
    /// Converts the specified unsigned long integer to a <see cref="Snowflake"/> instance.
    /// </summary>
    /// <param name="s">The unsigned long integer to convert.</param>
    /// <returns>A <see cref="Snowflake"/> instance that is equivalent to the specified unsigned long integer.</returns>
    [CLSCompliant(false)]
    public static explicit operator SnowflakeGuid(ulong s) => Parse(s);

    /// <summary>
    /// Creates a <see cref="Snowflake"/> instance from the specified unsigned long integer.
    /// </summary>
    /// <param name="s">The unsigned long integer representation of the <see cref="Snowflake"/>.</param>
    /// <returns>A <see cref="Snowflake"/> instance that corresponds to the specified unsigned long integer.</returns>
    [CLSCompliant(false)]
    public static SnowflakeGuid FromUInt64(ulong s) => (SnowflakeGuid)s;

    /// <summary>
    /// Converts the specified <see cref="Snowflake"/> instance to its string representation.
    /// </summary>
    /// <param name="s">The <see cref="Snowflake"/> instance to convert.</param>
    /// <returns>A string representation of the specified <see cref="Snowflake"/> instance.</returns>
    public static implicit operator string(SnowflakeGuid s) => s?.ToString();

    /// <summary>
    /// Converts the specified <see cref="Snowflake"/> instance to its unsigned long integer representation.
    /// </summary>
    /// <param name="s">The <see cref="Snowflake"/> instance to convert.</param>
    /// <returns>An unsigned long integer representation of the specified <see cref="Snowflake"/> instance.</returns>
    /// <remarks>
    /// If a loosely typed language (or a language that doesn't differentiate between number types, i.e.: Typescript)
    /// is part of your workflow, use the string representation to avoid issues regarding
    /// floating-point underflow and rounding.
    /// </remarks>
    [CLSCompliant(false)]
    public static implicit operator ulong(SnowflakeGuid s) => s?.Id ?? default;

    /// <summary>
    /// Converts the current <see cref="Snowflake"/> instance to its unsigned long integer representation.
    /// </summary>
    /// <returns>An unsigned long integer representation of the current <see cref="Snowflake"/> instance.</returns>
    /// <remarks>
    /// If a loosely typed language (or a language that doesn't differentiate between number types, i.e.: Typescript)
    /// is part of your workflow, use <see cref="ToString()"/> to avoid issues regarding
    /// floating-point underflow and rounding.
    /// </remarks>
    [CLSCompliant(false)]
    public ulong ToUInt64() => this;
    #endregion




    public Guid Guid { get; internal set; }


    /// <summary>
    /// Converts the current <see cref="Snowflake"/> instance to Guid object
    /// </summary>
    /// <returns></returns>
    public Guid ToGuid(){
        string hex = ToHex(Id);

        var rand = GenerateRandomHexadecimal(12);
        
        // Divide em partes para formar um UUID
        string uuidString = $"{hex.Substring(0, 8)}-{hex.Substring(8, 4)}-{hex.Substring(12, 4)}-0000-{rand}";
        //Console.WriteLine(hex);
        return new Guid(uuidString);
    }


    
    string ToHex(ulong number)
    {
        const string chars = "abcdef0123456789";
        StringBuilder result = new StringBuilder();

        do
        {
            result.Insert(0, chars[(int)(number % 16)]);
            number /= 16;
        } while (number > 0);

        return result.ToString();
    }


    string GenerateRandomHexadecimal(int size)
    {
        const string chars = "0123456789abcdef";
        Random random = new Random();
        char[] hexArray = new char[size];

        for (int i = 0; i < size; i++)
        {
            hexArray[i] = chars[random.Next(chars.Length)];
        }

        return new string(hexArray);
    }



    /// <summary>
    /// Converts a Guid to a SnowflakeGuid object
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ulong FromGuid(Guid guid){
        string guidString = guid.ToString("N"); // Remove os hifens, formato "N" é o hexadecimal puro do Guid

        if (guidString.Length < 16)
        {
            throw new ArgumentException("The provided Guid is not valid for conversion.", nameof(guid));
        }

        string hex = guidString.Substring(0, 16); // Apenas a parte relevante para Id
        return FromHex(hex);
    }

    ulong FromHex(string hex)
    {
        if (string.IsNullOrEmpty(hex))
        {
            throw new ArgumentException("Input string cannot be null or empty.", nameof(hex));
        }

        const string chars = "abcdef0123456789";
        ulong result = 0;

        foreach (char c in hex.ToLower())
        {
            int value = chars.IndexOf(c);
            if (value == -1)
            {
                throw new FormatException("Input string contains invalid hexadecimal characters.");
            }

            result = (result * 16) + (ulong)value;
        }

        return result;
    }

    
    public static SnowflakeGuid NewGuid(){

        return SnowflakeGuidGenerator.GetSnowflake();
    }




    #region GENERATOR

    #if NET9_0_OR_GREATER
    private static readonly Lock lockObject = new();
    #else
    private static readonly object lockObject = new();
    #endif

    private static readonly ulong MACHINE_ID;


    /// <summary>
    /// Generates the next SnowflakeGuid ID.
    /// </summary>
    /// <returns>A <see cref="Snowflake"/> object containing the generated ID.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the system clock is moved backwards.
    /// </exception>
    public static SnowflakeGuid GetSnowflake()
    {
        lock (lockObject)
        {
            ulong currentTimestampMillis = DateTimeHelper.TimestampMillisFromEpoch(DateTime.UtcNow, GlobalConstants.DefaultEpoch);

            if (Sequencer == 0 && currentTimestampMillis == LastTimeStamp)
            {
                do
                {
                    Thread.Sleep(1);
                    currentTimestampMillis = DateTimeHelper.TimestampMillisFromEpoch(DateTime.UtcNow, GlobalConstants.DefaultEpoch);
                } while (currentTimestampMillis == LastTimeStamp);
            }
            else if (currentTimestampMillis < LastTimeStamp)
            {
                throw new InvalidOperationException("Time moved backwards!");
            }
            else if (currentTimestampMillis > LastTimeStamp)
            {
                Sequencer = 0;
            }
            SetLastTimestampDriftCorrected(currentTimestampMillis, GlobalConstants.DefaultEpoch);

            SnowflakeGuid snowflake = new(GlobalConstants.DefaultEpoch)
            {
                Timestamp = currentTimestampMillis,
                MachineId = MACHINE_ID,
                Sequence = Sequencer,
            };

            snowflake.Guid = snowflake.ToGuid();

            Sequencer++;

            return snowflake;
        }
    }

    private static void SetLastTimestampDriftCorrected(ulong timestamp, DateTime epoch) =>
        LastTimestampDriftCorrected = timestamp + DateTimeHelper.TimestampMillisFromEpoch(epoch, GlobalConstants.DefaultEpoch);

    private static ulong LastTimeStamp => LastTimestampDriftCorrected - DateTimeHelper.TimestampMillisFromEpoch(GlobalConstants.DefaultEpoch, GlobalConstants.DefaultEpoch);

    private static ulong LastTimestampDriftCorrected;

    private static ulong Sequencer;

    #endregion
}

