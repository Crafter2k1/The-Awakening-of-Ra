using System;
using System.Collections.Generic;

namespace Core.EventBusSystem
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                _subscribers[type] = new List<Delegate>();

            _subscribers[type].Add(callback);
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                list.Remove(callback);
                if (list.Count == 0)
                    _subscribers.Remove(type);
            }
        }

        public static void Invoke<T>(T eventData)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                var snapshot = list.ToArray(); 
                for (int i = 0; i < snapshot.Length; i++)
                    (snapshot[i] as Action<T>)?.Invoke(eventData);
            }
        }

    }
}