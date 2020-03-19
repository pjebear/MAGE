using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


abstract class DBEntryBase<T> where T : new()
{
    public T Entry = new T();

    public abstract void Copy(T from, T to);

    public void Fill(T entry)
    {
        Copy(entry, Entry);
    }

    public void CopyTo(T to)
    {
        Copy(Entry, to);
    }
}

