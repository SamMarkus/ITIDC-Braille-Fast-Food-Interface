using System;
using System.Collections.Generic;

// BiDictionary data type. Allows forward and reverse lookup of
// unique key/value pairs
class BiDictionary<TKey, TValue>
{
    // Variables 
    private Dictionary<TKey, TValue> forward = new();
    private Dictionary<TValue, TKey> reverse = new();

    // Constructor 
    public void Add(TKey key, TValue value)
    {
        // Throw error if duplicate key or value is found.
        if (forward.ContainsKey(key) || reverse.ContainsKey(value))
            throw new System.Exception("Duplicate key or value");

        forward[key] = value; // key = key, value = value
        reverse[value] = key; // key = value, value = key
    }

    public TValue GetByKey(TKey key) { return forward[key]; }
    public TKey GetByValue(TValue value) { return reverse[value]; }
    public bool ContainsKey(TKey key) { return forward.ContainsKey(key); }
    public bool ContainsValue(TValue value) { return reverse.ContainsKey(value); } 

    public TValue this[TKey key]
    {
        get => forward[key];
    }

    public TKey this[TValue value]
    {
        get => reverse[value];
    }
}