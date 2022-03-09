using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SecurityLogin.Cache
{
    /// <summary>
    /// Describes a hash-field (a name/value pair).
    /// In <see cref="https://github.com/StackExchange/StackExchange.Cache"/>
    /// </summary>
    public readonly struct BufferEntry : IEquatable<BufferEntry>
    {
        internal readonly BufferValue name, value;

        /// <summary>
        /// Initializes a <see cref="BufferEntry"/> value.
        /// </summary>
        /// <param name="name">The name for this hash entry.</param>
        /// <param name="value">The value for this hash entry.</param>
        public BufferEntry(BufferValue name, BufferValue value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// The name of the hash field.
        /// </summary>
        public BufferValue Name => name;

        /// <summary>
        /// The value of the hash field.
        /// </summary>
        public BufferValue Value => value;

        /// <summary>
        /// Converts to a key/value pair.
        /// </summary>
        /// <param name="value">The <see cref="BufferEntry"/> to create a <see cref="KeyValuePair{TKey, TValue}"/> from.</param>
        public static implicit operator KeyValuePair<BufferValue, BufferValue>(BufferEntry value) =>
            new KeyValuePair<BufferValue, BufferValue>(value.name, value.value);

        /// <summary>
        /// Converts from a key/value pair.
        /// </summary>
        /// <param name="value">The <see cref="KeyValuePair{TKey, TValue}"/> to get a <see cref="BufferEntry"/> from.</param>
        public static implicit operator BufferEntry(KeyValuePair<BufferValue, BufferValue> value) =>
            new BufferEntry(value.Key, value.Value);

        /// <summary>
        /// A "{name}: {value}" string representation of this entry.
        /// </summary>
        public override string ToString() => name + ": " + value;

        /// <inheritdoc/>
        public override int GetHashCode() => name.GetHashCode() ^ value.GetHashCode();

        /// <summary>
        /// Compares two values for equality.
        /// </summary>
        /// <param name="obj">The <see cref="BufferEntry"/> to compare to.</param>
        public override bool Equals(object obj) => obj is BufferEntry heObj && Equals(heObj);

        /// <summary>
        /// Compares two values for equality.
        /// </summary>
        /// <param name="other">The <see cref="BufferEntry"/> to compare to.</param>
        public bool Equals(BufferEntry other) => name == other.name && value == other.value;

        /// <summary>
        /// Compares two values for equality.
        /// </summary>
        /// <param name="x">The first <see cref="BufferEntry"/> to compare.</param>
        /// <param name="y">The second <see cref="BufferEntry"/> to compare.</param>
        public static bool operator ==(BufferEntry x, BufferEntry y) => x.name == y.name && x.value == y.value;

        /// <summary>
        /// Compares two values for non-equality.
        /// </summary>
        /// <param name="x">The first <see cref="BufferEntry"/> to compare.</param>
        /// <param name="y">The second <see cref="BufferEntry"/> to compare.</param>
        public static bool operator !=(BufferEntry x, BufferEntry y) => x.name != y.name || x.value != y.value;
    }
}
