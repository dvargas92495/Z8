﻿using UnityEngine.Events;

public class List<T> : Util
{
    private T[] items;

    public List()
    {
        items = new T[0];
    }

    public List(params T[] i)
    {
        items = i;
    }

    public List(List<T> l)
    {
        items = l.items;
    }

    public T Get(int i)
    {

        return items[i];
    }

    public List<T> Get(int i, int size)
    {
        return new List<T>(Get(items, i, size));
    }

    public T[] ToArray()
    {
        return Get(items, 0, items.Length);
    }

    public List<R> Map<R>(ReturnAction<T, R> callback)
    {
        return new List<R>(Map(items, callback));
    }

    public List<R> MapFlattened<R>(ReturnAction<T, List<R>> callback)
    {
        ReturnAction<T, R[]> modifiedCallback = i => callback(i).items;
        return new List<R>(Flatten(Map(items, modifiedCallback)));
    }

    public List<T> Filter(ReturnAction<T, bool> callback)
    {
        return new List<T>(Filter(items, callback));
    }

    public T Find(ReturnAction<T, bool> callback)
    {
        return Find(items, callback);
    }

    public int FindIndex(T item)
    {
        return FindIndex(items, item);
    }

    public int FindIndex(ReturnAction<T, bool> callback)
    {
        return FindIndex(items, callback);
    }

    public void Remove(T item)
    {
        items = Remove(items, item);
    }

    public void RemoveAt(int i)
    {
        items = RemoveAt(items, i);
    }

    public void RemoveAt(int i, int size)
    {
        items = RemoveAt(items, i, size);
    }

    public void RemoveAll(ReturnAction<T, bool> callback)
    {
        items = Filter(items, (T item) => !callback(item));
    }

    public string ToString(string delim)
    {
        return ToArrayString(items, delim);
    }

    public List<T> Concat(List<T> newItems)
    {
        return new List<T>(Add(items, newItems.items));
    }

    public void Add(T item)
    {
        items = Add(items, item);
    }

    public void Add(List<T> l)
    {
        items = Add(items, l.items);
    }

    public void Add(T item, int i)
    {
        items = Add(items, item, i);
    }

    public void Add(List<T> l, int i)
    {
        items = Add(items, l.items, i);
    }

    public bool Any(ReturnAction<T, bool> callback)
    {
        return Any(items, callback);
    }

    public bool Contains(T item)
    {
        return Contains(items, item);
    }

    public int Count(ReturnAction<T, bool> callback)
    {
        return Count(items, callback);
    }

    public void ForEach(UnityAction<T> callback)
    {
        ForEach(items, callback);
    }

    public void ForEach<U>(List<U> other, UnityAction<T, U> callback)
    {
        ForEach(items, other.items, callback);
    }

    public void Clear()
    {
        items = new T[0];
    }

    public int GetLength()
    {
        return items.Length;
    }

    public bool IsEmpty()
    {
        return items.Length == 0;
    }

    public U Reduce<U>(U initialValue, ReturnAction<U, T, U> callback)
    {
        return Reduce(items, initialValue, callback);
    }

    public List<T> Reverse()
    {
        return new List<T>(Reverse(items));
    }
}