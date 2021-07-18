using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace MyDictionary
{
    public class MyDictionary<TKey, TValue>
    {
        private int count;
        private int lastIndex = 0;
        public TValue[] values = new TValue[128];
        public int[] keys = new int[128];
        private object dicLock = new object();
        private object readLock = new object();
        public void Add(TKey key, TValue value)
        {
            var index = FindKeyIndex(key);

            if (index >= 0)
            {
                throw new ArgumentException("An element with the same key already exists in dictionary", nameof(key));
            }

            var hashCode = key.GetHashCode();

            lock (dicLock)
            {
                ResizeIfNeeded(1);

                keys[lastIndex] = hashCode;
                values[lastIndex] = value;
                count++;
                lastIndex++;
            }
        }

        private void ResizeIfNeeded(int requiredCapacity)
        {
            if (keys.Length < (lastIndex + 1 + requiredCapacity))
            {
                lock (readLock)
                {
                    lock (dicLock)
                    {
                        var newSize = Math.Min(1024, values.Length * 2);
                        Array.Resize(ref values, newSize);
                        Array.Resize(ref keys, newSize);
                    }
                }
            }
        }

        public object Get(TKey key)
        {
            if (Monitor.IsEntered(readLock))
            {
                Monitor.Wait(readLock);
            }
            int index = FindKeyIndex(key);

            if (index < 0)
            {
                throw new KeyNotFoundException($"Key {key} doesn't exist in dictionary");
            }

            var value = values[index];

            return value;

        }

        public void Remove(TKey key)
        {
            var index = FindKeyIndex(key);

            if (index < 0)
            {
                throw new KeyNotFoundException($"Key {key} doesn't exist in dictionary");
            }

            lock (dicLock)
            {
                keys[index] = 0;
                values[index] = default;
                count--;
            }
        }

        private int FindKeyIndex(TKey key)
        {
            var hashCode = key.GetHashCode();

            var index = Array.IndexOf(keys, hashCode);

            return index;
        }

        public bool ContainsKey(TKey key)
        {
            var index = FindKeyIndex(key);

            return index >= 0;
        }

        public int Count => count;
    }
}
