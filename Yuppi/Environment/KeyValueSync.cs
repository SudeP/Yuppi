using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Yuppi.Environment
{
    public class KeyValueSync<Key, Value> : ConcurrentDictionary<Key, Value>
    {
        public object lockerObject = new object();

        public void Add(Key key, Value value)
        {
            lock (lockerObject)
            {
                TryAdd(key, value);
            }
        }

        public Value Get(Key key)
        {
            lock (lockerObject)
            {
                TryGetValue(key, out Value value);

                return value;
            }
        }

        public void Remove(Key key)
        {
            lock (lockerObject)
            {
                TryRemove(key, out _);
            }
        }

        public bool Has(Key key)
        {
            lock (lockerObject)
            {
                return Keys.Contains(key);
            }
        }

        public Value First(Func<Key, Value, bool> func)
        {
            if (func is null)
                throw new ArgumentNullException(nameof(func));

            lock (lockerObject)
            {
                foreach (KeyValuePair<Key, Value> pair in this)
                {
                    if (func.Invoke(pair.Key, pair.Value))
                    {
                        return pair.Value;
                    }
                }

                return default;
            }
        }
    }
}
