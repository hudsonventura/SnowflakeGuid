﻿using System;
using System.Globalization;

namespace SnowflakeID
{
    public class Snowflake : IEquatable<Snowflake>, IComparable<Snowflake>, IComparable
    {
        #region constants
        public const ulong MAXIMO_SECUENCIA = 4096ul; //poner en 0 cuando se lleque a este valor. seq % MAXIMO_SECUENCIA
        public const ulong MAXIMO_TERMINAL = 1024ul; //cantidad. Rango: [0..1024) = [0..1023]
        //public const ulong MAXIMO_HASH = 16ul;

        private const int BITS_SHIFT_DATETIMEMILLIS = 22;
        private const int BITS_SHIFT_ESTACION = 12;
        //private const int BITS_SHIFT_HASH = 8;
        private const int BITS_SHIFT_SECUENCIA = 0;

        private const ulong MASK_DATETIMEMILLIS = 0b1111_1111_1111_1111_1111_1111_1111_1111_1111_1111_1100_0000_0000_0000_0000_0000;
        private const ulong MASK_ESTACION = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0011_1111_1111_0000_0000_0000;
        //private const ulong MASK_APP_HASH       = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_0000_0000;
        private const ulong MASK_SECUENCIA = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111;
        private const ulong MASK_DATETIMEMILLIS_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0011_1111_1111_1111_1111_1111_1111_1111_1111_1111_1111;
        private const ulong MASK_ESTACION_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0011_1111_1111;
        //private const ulong MASK_APP_HASH_RIGHT_ALIGNED
        //                                        = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111;
        private const ulong MASK_SECUENCIA_RIGHT_ALIGNED
                                                = 0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1111_1111_1111;


        private static readonly DateTime epoch = new DateTime(1970, 1, 1);
        public static readonly int CantidadDigitos = ulong.MaxValue.ToString(CultureInfo.InvariantCulture).Length;
        #endregion


        public virtual DateTime UtcDateTime { get; set; }
        public virtual ulong MachineId
        {
            get => _MachineId;
            set
            {
                if (value >= MAXIMO_TERMINAL)
                {
                    throw new ArgumentOutOfRangeException(nameof(MachineId), $"{nameof(MachineId)} must be less than {MAXIMO_TERMINAL}. Got: {value}.");
                }
                else
                {
                    _MachineId = value;
                }
            }
        }
        private ulong _MachineId;
        public virtual ulong Secuencia
        {
            get => _Secuencia;
            set
            {
                if (value >= MAXIMO_SECUENCIA)
                {
                    throw new ArgumentOutOfRangeException(nameof(Secuencia), $"{nameof(Secuencia)} must be less than {MAXIMO_SECUENCIA}. Got: {value}.");
                }
                else
                {
                    _Secuencia = value;
                }
            }
        }
        private ulong _Secuencia;
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

        public virtual ulong Id
        {
            get
            {
                ulong timestamp = ((ulong)UtcDateTime.Subtract(epoch).Ticks) / ((ulong)TimeSpan.TicksPerMillisecond);
                return ((timestamp & MASK_DATETIMEMILLIS_RIGHT_ALIGNED) << BITS_SHIFT_DATETIMEMILLIS)
                                        | ((MachineId & MASK_ESTACION_RIGHT_ALIGNED) << BITS_SHIFT_ESTACION)
                                        //| ((APP_HASH & MASK_APP_HASH_RIGHT_ALIGNED) << BITS_SHIFT_HASH)
                                        | ((Secuencia & MASK_SECUENCIA_RIGHT_ALIGNED) << BITS_SHIFT_SECUENCIA);
            }
            private set
            {
                ulong val = value >> BITS_SHIFT_SECUENCIA;
                Secuencia = val & MASK_SECUENCIA_RIGHT_ALIGNED;
                val >>= BITS_SHIFT_ESTACION - BITS_SHIFT_SECUENCIA;
                MachineId = val & MASK_ESTACION_RIGHT_ALIGNED;
                val >>= BITS_SHIFT_DATETIMEMILLIS - BITS_SHIFT_ESTACION - BITS_SHIFT_SECUENCIA;
                Timestamp = val & MASK_DATETIMEMILLIS_RIGHT_ALIGNED;
            }
        }

        public virtual string Barcode
        {
            get
            {
                return Id.ToString(CultureInfo.InvariantCulture).PadLeft(CantidadDigitos, '0');
            }
            private set
            {
                Id = ulong.Parse(value,CultureInfo.InvariantCulture);
            }
        }


        public static Snowflake Parse(string s)
        {
            return new Snowflake() { Barcode = s };
        }

        public static Snowflake Parse(ulong b)
        {
            return new Snowflake() { Id = b };
        }



        public override string ToString()
        {
            return Barcode;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Snowflake other)) { return false; }
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
            if (!(obj is Snowflake other)) { return 1; }
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

        public static bool operator >(Snowflake s1, Snowflake s2) => (s1.CompareTo(s2) > 0);
        public static bool operator <(Snowflake s1, Snowflake s2) => (s1.CompareTo(s2) < 0);
        public static bool operator >=(Snowflake s1, Snowflake s2) => (s1.CompareTo(s2) >= 0);
        public static bool operator <=(Snowflake s1, Snowflake s2) => (s1.CompareTo(s2) <= 0);
    }
}
