    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 自定义简易事件中心，统一进行事件的订阅、取消订阅和发布。
    /// 采用模块为Key->检索事件Key->检索委托链所有Action<T> listener的方式控制。
    /// </summary>
    public class EventCenter
    {
        private static EventCenter instance;
        public static EventCenter Instance => instance ??= new EventCenter();

        // 存储了所有事件，Key为模块名，根据模块名获得其下所有事件，之后根据事件名获得委托
        private readonly Dictionary<string, Dictionary<string, Delegate>> eventTable = new();
        private EventCenter() { }

        /// <summary>
        /// 传入自定义模块名称，事件名称和Action<T>作为listener即可订阅事件。
        /// 示例用法：EventCenter.Instance.Subscribe<string>(EventModule.Player, PlayerEvent.StateChanged, OnStateChanged);
        /// </summary>
        public void Subscribe<T>(Enum moduleName, Enum eventName, Action<T> listener)
        {
            if (moduleName == null) throw new ArgumentNullException(nameof(moduleName));
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            string moduleKey = moduleName.ToString();
            string eventKey = eventName.ToString();

            // 创建模块
            if (!eventTable.TryGetValue(moduleKey, out var moduleEvents))
            {
                moduleEvents = new Dictionary<string, Delegate>();
                eventTable[moduleKey] = moduleEvents;
            }

            // 把订阅者和事件绑定，事件存在直接加一个listener，不存在就创建事件字段然后加一个listener
            if (moduleEvents.TryGetValue(eventKey, out Delegate eventDelegate))
            {
                moduleEvents[eventKey] = Delegate.Combine(eventDelegate, listener);
            }
            else
            {
                moduleEvents[eventKey] = listener;
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe<T>(Enum moduleName, Enum eventName, Action<T> listener)
        {
            if (moduleName == null) throw new ArgumentNullException(nameof(moduleName));
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            string moduleKey = moduleName.ToString();
            string eventKey = eventName.ToString();

            if (eventTable.TryGetValue(moduleKey, out var moduleEvents))
            {
                if (moduleEvents.TryGetValue(eventKey, out var existingDelegate))
                {
                    var newDelegate = Delegate.Remove(existingDelegate, listener);
                    if (newDelegate == null)
                    {
                        moduleEvents.Remove(eventKey);
                        if (moduleEvents.Count == 0)
                        {
                            eventTable.Remove(moduleKey);
                        }
                    }
                    else
                    {
                        moduleEvents[eventKey] = newDelegate;
                    }
                }
            }
        }

        /// <summary>
        /// 指定模块、事件名，发布事件
        /// 示例：EventCenter.Instance.Publish(EventModule.Player, PlayerEvent.StateChanged, newState.GetType().Name);
        /// </summary>
        public void Publish<T>(Enum moduleName, Enum eventName, T param)
        {
            if (moduleName == null) throw new ArgumentNullException(nameof(moduleName));
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));

            string moduleKey = moduleName.ToString();
            string eventKey = eventName.ToString();

            if (eventTable.TryGetValue(moduleKey, out var moduleEvents))
            {
                if (moduleEvents.TryGetValue(eventKey, out Delegate curDelegate))
                {
                    if (curDelegate is Action<T> callback)
                    {
                        callback.Invoke(param);
                    }
                    else
                    {
                        throw new InvalidOperationException($"模块：{moduleName} 中的事件：{eventName} 不符合Action<T>");
                    }
                }
            }
        }

        public enum EventModule
        {
            Player,
        }

        public enum PlayerEvent
        {
            StateChanged,
        }
    }
