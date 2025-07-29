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
                throw new ArgumentException("Topic cannot be null or empty", nameof(topic));
            }

            PublishToTopic<T>(topic, data);
            PublishToWildcardTopics<T>(topic, data);
        }

        private void PublishToTopic<T>(string topic, T data)
        {
            if (handlers.ContainsKey(topic) == false)
            {
                return;
            }

            List<HandlerWrapper<T>> typedHandlers = handlers[topic]
                .OfType<HandlerWrapper<T>>()
                .ToList();

            foreach (var wrapper in typedHandlers)
            {
                try
                {
                    wrapper.Handler?.Invoke(data);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error executing handler for topic '{topic}': {ex.Message}");
                }
            }
        }

        private void PublishToWildcardTopics<T>(string topic, T data)
        {
            foreach (var kvp in handlers.ToList())
            {
                if (IsWildcardMatch(kvp.Key, topic))
                {
                    var typedHandlers = kvp.Value
                        .OfType<HandlerWrapper<T>>()
                        .ToList();

                    foreach (var wrapper in typedHandlers)
                    {
                        try
                        {
                            wrapper.Handler?.Invoke(data);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error executing wildcard handler for pattern '{kvp.Key}' matching topic '{topic}': {ex.Message}");
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

        private class HandlerWrapper<T>
        {
            public Action<T> Handler { get; }

            public HandlerWrapper(Action<T> handler)
            {
                Handler = handler;
            }
        }
    }
}
