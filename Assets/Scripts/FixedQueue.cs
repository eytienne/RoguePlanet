using System;
using System.Collections.Generic;
using System.Linq;

// not extending Queue but LinkedList because we want the last at start
public class FixedQueue<T> : LinkedList<T> where T : struct
{
    public int Limit { get; private set; }

    public FixedQueue(int limit) : base() {
        Limit = limit;
    }
    public FixedQueue(int limit, IEnumerable<T> collection) : base(collection) {
        Limit = limit;
    }

    public void Enqueue(T item) {
        base.AddFirst(item);
        if (base.Count > Limit) Dequeue();
    }

    public T Dequeue() {
        T ret = base.Last.Value;
        base.RemoveLast();
        return ret;
    }

    public T? Get(int index) {
        try {
            return this.ElementAt(index);
        }
        catch (ArgumentOutOfRangeException) {
            return null;
        }
    }
}