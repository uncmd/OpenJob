namespace System;

[Serializable]
public class AtomicInt : IEquatable<AtomicInt>, IEquatable<int>
{
    private int value;

    public AtomicInt(int value)
    {
        this.value = value;
    }

    public int Value
    {
        get => Interlocked.CompareExchange(ref value, 0, 0);
        set => Interlocked.Exchange(ref this.value, value);
    }

    public int GetAndSet(int newValue)
    {
        return Interlocked.Exchange(ref value, newValue);
    }

    public bool CompareAndSet(int expect, int update)
    {
        int rc = Interlocked.CompareExchange(ref value, update, expect);
        return rc == expect;
    }

    public int GetAndIncrement()
    {
        return Interlocked.Increment(ref value) - 1;
    }

    public int GetAndDecrement()
    {
        return Interlocked.Decrement(ref value) + 1;
    }

    public int GetAndAdd(int value)
    {
        return Interlocked.Add(ref this.value, value) - value;
    }

    public int IncrementAndGet()
    {
        return Interlocked.Increment(ref value);
    }

    public int DecrementAndGet()
    {
        return Interlocked.Decrement(ref value);
    }

    public int AddAndGet(int value)
    {
        return Interlocked.Add(ref this.value, value);
    }

    public bool Equals(AtomicInt other)
    {
        if (other == null)
            return false;
        return Value == other.Value;
    }

    public bool Equals(int other)
    {
        return Value == other;
    }

    public override bool Equals(object other)
    {
        if (other is AtomicInt ai)
            return Equals(ai);
        if (other is int i)
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

    #region Operator Overrides

    public static implicit operator int(AtomicInt atomicInt32)
    {
        return atomicInt32.Value;
    }

    public static bool operator ==(AtomicInt a1, AtomicInt a2)
    {
        return a1.Value == a2.Value;
    }

    public static bool operator !=(AtomicInt a1, AtomicInt a2)
    {
        return !(a1 == a2);
    }

    public static bool operator ==(AtomicInt a1, int a2)
    {
        return a1.Value == a2;
    }

    public static bool operator !=(AtomicInt a1, int a2)
    {
        return !(a1 == a2);
    }

    public static bool operator ==(int a1, AtomicInt a2)
    {
        return a1 == a2.Value;
    }

    public static bool operator !=(int a1, AtomicInt a2)
    {
        return !(a1 == a2);
    }

    public static bool operator ==(AtomicInt a1, int? a2)
    {
        return a1.Value == a2.GetValueOrDefault();
    }

    public static bool operator !=(AtomicInt a1, int? a2)
    {
        return !(a1 == a2);
    }

    public static bool operator ==(int? a1, AtomicInt a2)
    {
        return a1.GetValueOrDefault() == a2.Value;
    }

    public static bool operator !=(int? a1, AtomicInt a2)
    {
        return !(a1 == a2);
    }

    #endregion Operator Overrides
}
