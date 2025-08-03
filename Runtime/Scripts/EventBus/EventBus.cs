using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<string, List<object>> handlers = new Dictionary<string, List<object>>();

        public void Subscribe<T>(string topic, Action<T> handler)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Topic cannot be null or empty", nameof(topic));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (handlers.ContainsKey(topic) == false)
            {
                handlers[topic] = new List<object>();
            }

            HandlerWrapper<T> wrapper = new HandlerWrapper<T>(handler);
            handlers[topic].Add(wrapper);
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

            if (handlers.ContainsKey(topic) == false)
            {
                throw new KeyNotFoundException($"No handlers found for topic '{topic}'");
            }

            List<object> handlersToRemove = handlers[topic]
                .OfType<HandlerWrapper<T>>()
                .Where(w => ReferenceEquals(w.Handler, handler))
                .Cast<object>()
                .ToList();

            foreach (var handlerToRemove in handlersToRemove)
            {
                handlers[topic].Remove(handlerToRemove);
            }

            if (handlers[topic].Count == 0)
            {
                handlers.Remove(topic);
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

            var dataType = data?.GetType() ?? typeof(T);
            var executedHandlers = new HashSet<object>();

            foreach (var kvp in handlers.ToList())
            {
                bool isMatch = kvp.Key == topic || IsWildcardMatch(kvp.Key, topic);

                if (isMatch)
                {
                    foreach (var handler in kvp.Value.ToList())
                    {
                        if (executedHandlers.Contains(handler))
                        {
                            Debug.LogWarning($"Handler for topic '{topic}' (pattern: '{kvp.Key}') has already been executed. Skipping duplicate execution. handler: {handler}");
                            continue;
                        }

                        try
                        {
                            var handlerType = handler.GetType();
                            if (handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof(HandlerWrapper<>))
                            {
                                var expectedType = handlerType.GetGenericArguments()[0];

                                if (expectedType.IsAssignableFrom(dataType))
                                {
                                    var handlerProperty = handlerType.GetProperty("Handler");
                                    var handlerDelegate = handlerProperty?.GetValue(handler) as Delegate;
                                    handlerDelegate?.DynamicInvoke(data);

                                    executedHandlers.Add(handler);
                                }
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

        private bool IsWildcardMatch(string pattern, string topic)
        {
            if (pattern.Contains("*") == false)
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

        public bool HasTopic(string topic)
        {
            return handlers.ContainsKey(topic) && handlers[topic].Count > 0;
        }

        public int GetHandlerCount(string topic)
        {
            return handlers.ContainsKey(topic) ? handlers[topic].Count : 0;
        }
    }
}
