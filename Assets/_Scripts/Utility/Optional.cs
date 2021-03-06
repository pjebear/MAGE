﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


struct Empty
{

}

struct Optional<T>
{
    public static Empty Empty { get { return new Empty(); } }
    public bool HasValue { get; private set; }
    private T value;
    public T Value
    {
        get
        {
            if (HasValue)
                return value;
            else
                throw new InvalidOperationException();
        }
    }

    public Optional(T value)
    {
        this.value = value;
        HasValue = true;
    }

    public static explicit operator T(Optional<T> optional)
    {
        return optional.Value;
    }

    public static implicit operator Optional<T>(T value)
    {
        return new Optional<T>(value);
    }

    public static implicit operator Optional<T>(Empty value)
    {
        return new Optional<T>();
    }

    public override bool Equals(object obj)
    {
        if (obj is Optional<T>)
            return this.Equals((Optional<T>)obj);
        else
            return false;
    }

    public void Reset()
    {
        HasValue = false;
        value = default;
    }

    public bool Equals(Optional<T> other)
    {
        if (HasValue && other.HasValue)
            return object.Equals(value, other.value);
        else
            return HasValue == other.HasValue;
    }

    public T ValueOr(T or)
    {
        if (HasValue)
        {
            return Value;
        }
        else
        {
            return or;
        }
    }

    public override int GetHashCode()
    {
        var hashCode = -282591772;
        hashCode = hashCode * -1521134295 + HasValue.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(value);
        hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Value);
        return hashCode;
    }
}
