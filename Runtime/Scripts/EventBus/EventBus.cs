using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<string, Dictionary<Type, SortedDictionary<int, List<object>>>> handlers = 
            new Dictionary<string, Dictionary<Type, SortedDictionary<int, List<object>>>>();

        public void Subscribe<T>(string topic, Action<T> handler)
        {
            Subscribe(topic, handler, 0);
        }

        public void Subscribe<T>(string topic, Action<T> handler, int priority)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Topic cannot be null or empty", nameof(topic));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Type handlerType = typeof(T);

            if (!handlers.ContainsKey(topic))
            {
                handlers[topic] = new Dictionary<Type, SortedDictionary<int, List<object>>>();
            }

            if (!handlers[topic].ContainsKey(handlerType))
            {
                handlers[topic][handlerType] = new SortedDictionary<int, List<object>>();
            }

            if (!handlers[topic][handlerType].ContainsKey(priority))
            {
                handlers[topic][handlerType][priority] = new List<object>();
            }

            HandlerWrapper<T> wrapper = new HandlerWrapper<T>(handler);
            handlers[topic][handlerType][priority].Add(wrapper);
        }

        public void Unsubscribe<T>(string topic, Action<T> handler)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Topic cannot be null or empty", nameof(topic));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!handlers.ContainsKey(topic))
            {
                throw new KeyNotFoundException($"No handlers found for topic '{topic}'");
            }

            Type handlerType = typeof(T);

            if (!handlers[topic].ContainsKey(handlerType))
            {
                throw new KeyNotFoundException($"No handlers found for topic '{topic}' and type '{handlerType}'");
            }

            var priorityDict = handlers[topic][handlerType];
            var prioritiesToRemove = new List<int>();

            foreach (var kvp in priorityDict)
            {
                var priority = kvp.Key;
                var handlerList = kvp.Value;

                var handlersToRemove = handlerList
                    .OfType<HandlerWrapper<T>>()
                    .Where(w => ReferenceEquals(w.Handler, handler))
                    .Cast<object>()
                    .ToList();

                foreach (var handlerToRemove in handlersToRemove)
                {
                    handlerList.Remove(handlerToRemove);
                }

                if (handlerList.Count == 0)
                {
                    prioritiesToRemove.Add(priority);
                }
            }

            foreach (var priority in prioritiesToRemove)
            {
                priorityDict.Remove(priority);
            }

            if (priorityDict.Count == 0)
            {
                handlers[topic].Remove(handlerType);

                if (handlers[topic].Count == 0)
                {
                    handlers.Remove(topic);
                }
            }
        }

        public void UnsubscribeAll(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Topic cannot be null or empty", nameof(topic));
            }

            handlers.Remove(topic);
        }

        public void Publish<T>(string topic, T data)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Topic cannot be null or empty!");
            }

            Type dataType = data.GetType();
            HashSet<object> executedHandlers = new HashSet<object>();

            foreach (var kvp in handlers.ToList())
            {
                bool isMatch = kvp.Key == topic || IsWildcardMatch(kvp.Key, topic);
                if (!isMatch)
                {
                    continue;
                }

                if (kvp.Value.TryGetValue(dataType, out var priorityDict))
                {
                    foreach (var handlerList in priorityDict.Values.ToList())
                    {
                        foreach (var handler in handlerList.ToList())
                        {
                            if (executedHandlers.Contains(handler))
                            {
                                Debug.LogWarning($"Handler for topic '{topic}' (pattern: '{kvp.Key}') has already been executed. Skipping duplicate execution. handler: {handler}");
                                continue;
                            }

                            try
                            {
                                Type handlerType = handler.GetType();
                                if (handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof(HandlerWrapper<>))
                                {
                                    var handlerProperty = handlerType.GetProperty("Handler");
                                    var handlerDelegate = handlerProperty?.GetValue(handler) as Delegate;
                                    handlerDelegate?.DynamicInvoke(data);

                                    executedHandlers.Add(handler);
                                }
                            }
                            catch (System.Reflection.TargetInvocationException e)
                            {
                                var innerException = e.InnerException ?? e;
                                Debug.LogError($"Error executing handler for topic '{topic}' (pattern: '{kvp.Key}'): {innerException.Message}\n" +
                                               $"Handler Type: {handler.GetType()}\n" +
                                               $"Data Type: {dataType}\n" +
                                               $"Inner Exception: {innerException}\n" +
                                               $"Stack Trace: {innerException.StackTrace}");
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"Error executing handler for topic '{topic}' (pattern: '{kvp.Key}'): {e.Message}\n" +
                                               $"Handler Type: {handler.GetType()}\n" +
                                               $"Data Type: {dataType}\n" +
                                               $"Exception: {e}\n" +
                                               $"Stack Trace: {e.StackTrace}");
                            }
                        }
                    }
                }
            }
        }

        private bool IsWildcardMatch(string pattern, string topic)
        {
            if (!pattern.Contains("*"))
            {
                return false;
            }

            if (pattern.EndsWith("*"))
            {
                string prefix = pattern.Substring(0, pattern.Length - 1);
                return topic.StartsWith(prefix);
            }

            if (pattern.StartsWith("*"))
            {
                string suffix = pattern.Substring(1);
                return topic.EndsWith(suffix);
            }

            string[] tokens = pattern.Split('*');
            if (tokens.Length == 2)
            {
                return topic.StartsWith(tokens[0]) && topic.EndsWith(tokens[1]);
            }

            return false;
        }

        public void Clear()
        {
            handlers.Clear();
        }
    }
}
