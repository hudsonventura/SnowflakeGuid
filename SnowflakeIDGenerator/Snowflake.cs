using System;
using System.Globalization;

namespace SnowflakeID
{
    public class Snowflake : IEquatable<Snowflake>, IComparable<Snowflake>, IComparable
    {
        #region constants
        public const long MaxSequence = 4096; //poner en 0 cuando se lleque a este valor. seq % MaxSequence
        public const long MaxMachineId = 1024; //cantidad. Rango: [0..1024) = [0..1023]


        private const int BITS_SHIFT_DATETIMEMILLIS = 22;
        private const int BITS_SHIFT_ESTACION = 12;
        private const int BITS_SHIFT_SECUENCIA = 0;


        private const ulong MASK_DATETIMEMILLIS_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0011_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111;
        private const ulong MASK_ESTACION_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0011_1111_1111;
        private const ulong MASK_SECUENCIA_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111;


        private static readonly DateTime epoch = new DateTime(1970, 1, 1);
        public static readonly int NumberOfDigits = ulong.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
        #endregion


        public virtual DateTime UtcDateTime { get; set; }

        [CLSCompliant(false)]
        public virtual ulong MachineId
        {
            get => _MachineId;
            set
            {
                if (value >= MaxMachineId)
                {
                    throw new ArgumentOutOfRangeException(nameof(MachineId), $"{nameof(MachineId)} must be less than {MaxMachineId}. Got: {value}.");
                }
                else
                {
                    _MachineId = value;
                }
            }
        }
        public int MachineIdInt32
        {
            get => (int)MachineId;
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(MachineIdInt32)); }
                MachineId = (ulong)value;
            }
        }
        private ulong _MachineId;


        [CLSCompliant(false)]
        public virtual ulong Sequence
        {
            get => _Sequence;
            set
            {
                if (value >= MaxSequence)
                {
                    throw new ArgumentOutOfRangeException(nameof(Sequence), $"{nameof(Sequence)} must be less than {MaxSequence}. Got: {value}.");
                }
                else
                {
                    _Sequence = value;
                }
            }
        }
        public int SequenceInt32
        {
            get => (int)Sequence;
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(SequenceInt32)); }
                Sequence = (ulong)value;
            }
        }
        private ulong _Sequence;


        [CLSCompliant(false)]
        public virtual ulong Timestamp
        {
            get
            {
                return ((ulong)UtcDateTime.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
            }
            set
            {
                UtcDateTime = DateTime.SpecifyKind(epoch.AddTicks((long)value * (TimeSpan.TicksPerMillisecond)), DateTimeKind.Utc);
            }
        }
        public long TimestampInt64
        {
            get => (long)Timestamp;
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(TimestampInt64)); }
                ulong newVal = (ulong)value & MASK_DATETIMEMILLIS_RIGHT_ALIGNED;
                if (newVal != (ulong)value) { throw new ArgumentOutOfRangeException(nameof(TimestampInt64)); }
                Timestamp = newVal;
            }
        }

        [CLSCompliant(false)]
        public virtual ulong Id
        {
            get
            {
                ulong timestamp = ((ulong)UtcDateTime.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
                return ((timestamp & MASK_DATETIMEMILLIS_RIGHT_ALIGNED) << BITS_SHIFT_DATETIMEMILLIS)
                                        | ((MachineId & MASK_ESTACION_RIGHT_ALIGNED) << BITS_SHIFT_ESTACION)
                                        | ((Sequence & MASK_SECUENCIA_RIGHT_ALIGNED) << BITS_SHIFT_SECUENCIA);
            }
            private set
            {
                ulong val = value >> BITS_SHIFT_SECUENCIA;
                Sequence = val & MASK_SECUENCIA_RIGHT_ALIGNED;
                val >>= BITS_SHIFT_ESTACION - BITS_SHIFT_SECUENCIA;
                MachineId = val & MASK_ESTACION_RIGHT_ALIGNED;
                val >>= BITS_SHIFT_DATETIMEMILLIS - BITS_SHIFT_ESTACION - BITS_SHIFT_SECUENCIA;
                Timestamp = val & MASK_DATETIMEMILLIS_RIGHT_ALIGNED;
            }
        }

        public virtual string Code
        {
            get
            {
                return Id.ToString(CultureInfo.InvariantCulture).PadLeft(NumberOfDigits, '0');
            }
            private set
            {
                Id = ulong.Parse(value, CultureInfo.InvariantCulture);
            }
        }


        public static Snowflake Parse(string s)
        {
            return new Snowflake() { Code = s };
        }

        [CLSCompliant(false)]
        public static Snowflake Parse(ulong b)
        {
            return new Snowflake() { Id = b };
        }



        public override string ToString()
        {
            return Code;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Snowflake other) { return false; }
            return Equals(other);
        }

        public bool Equals(Snowflake other)
        {
            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return (int)(Id >> 32);
        }

        public int CompareTo(object obj)
        {
            if (obj is not Snowflake other) { return 1; }
            return CompareTo(other);
        }

        public int CompareTo(Snowflake other)
        {
            if (other == null) { return 1; }
            return Id.CompareTo(other.Id);
        }

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

        public static bool operator !=(Snowflake s1, Snowflake s2) => !(s1 == s2);

        public static bool operator >(Snowflake s1, Snowflake s2) => (!(s1 is null ^ s2 is null) && s1.CompareTo(s2) > 0);

        public static bool operator <(Snowflake s1, Snowflake s2) => (!(s1 is null ^ s2 is null) && s1.CompareTo(s2) < 0);
        public static bool operator >=(Snowflake s1, Snowflake s2) => (!(s1 is null ^ s2 is null) && s1.CompareTo(s2) >= 0);
        public static bool operator <=(Snowflake s1, Snowflake s2) => (!(s1 is null ^ s2 is null) && s1.CompareTo(s2) <= 0);
    }
}
