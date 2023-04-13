namespace System;

[Serializable]
public class AtomicLong : IEquatable<AtomicLong>, IEquatable<long>, IComparable<AtomicLong>
{
    private long value;

    public AtomicLong(long value)
    {
        this.value = value;
    }

    public long Value
    {
        get => Interlocked.CompareExchange(ref value, 0, 0);
        set => Interlocked.Exchange(ref this.value, value);
    }

    public long GetAndSet(long newValue)
    {
        return Interlocked.Exchange(ref value, newValue);
    }

    public bool CompareAndSet(long expect, long update)
    {
        long rc = Interlocked.CompareExchange(ref value, update, expect);
        return rc == expect;
    }

    public long GetAndIncrement()
    {
        return Interlocked.Increment(ref value) - 1;
    }

    public long GetAndDecrement()
    {
        return Interlocked.Decrement(ref value) + 1;
    }

    public long GetAndAdd(long value)
    {
        return Interlocked.Add(ref this.value, value) - value;
    }

    public long IncrementAndGet()
    {
        return Interlocked.Increment(ref value);
    }

    public long DecrementAndGet()
    {
        return Interlocked.Decrement(ref value);
    }

    public long AddAndGet(long value)
    {
        return Interlocked.Add(ref this.value, value);
    }

    public bool Equals(AtomicLong other)
    {
        if (other == null)
            return false;
        return Value == other.Value;
    }

    public bool Equals(long other)
    {
        return Value == other;
    }

    public override bool Equals(object other)
    {
        if (other is AtomicLong ai)
            return Equals(ai);
        if (other is long i)
            return Equals(i);
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public int CompareTo(AtomicLong other)
    {
        return Value.CompareTo(other.Value);
    }

    #region Operator Overrides

    public static implicit operator long(AtomicLong atomicInt32)
    {
        return atomicInt32.Value;
    }

    public static bool operator ==(AtomicLong a1, AtomicLong a2)
    {
        return a1.Value == a2.Value;
    }

    public static bool operator !=(AtomicLong a1, AtomicLong a2)
    {
        return !(a1 == a2);
    }

    public static bool operator ==(AtomicLong a1, long a2)
    {
        return a1.Value == a2;
    }

    public static bool operator !=(AtomicLong a1, long a2)
    {
        return !(a1 == a2);
    }

    public static bool operator ==(long a1, AtomicLong a2)
    {
        return a1 == a2.Value;
    }

    public static bool operator !=(long a1, AtomicLong a2)
    {
        return !(a1 == a2);
    }

    public static bool operator ==(AtomicLong a1, long? a2)
    {
        return a1.Value == a2.GetValueOrDefault();
    }

    public static bool operator !=(AtomicLong a1, long? a2)
    {
        return !(a1 == a2);
    }

    public static bool operator ==(long? a1, AtomicLong a2)
    {
        return a1.GetValueOrDefault() == a2.Value;
    }

    public static bool operator !=(long? a1, AtomicLong a2)
    {
        return !(a1 == a2);
    }

    #endregion Operator Overrides
}
