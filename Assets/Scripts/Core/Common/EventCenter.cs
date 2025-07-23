    using System;
    using System.Collections.Generic;

    /// <summary>
    /// �Զ�������¼����ģ�ͳһ�����¼��Ķ��ġ�ȡ�����ĺͷ�����
    /// ����ģ��ΪKey->�����¼�Key->����ί��������Action<T> listener�ķ�ʽ���ơ�
    /// </summary>
    public class EventCenter
    {
        private static EventCenter instance;
        public static EventCenter Instance => instance ??= new EventCenter();

        // �洢�������¼���KeyΪģ����������ģ����������������¼���֮������¼������ί��
        private readonly Dictionary<string, Dictionary<string, Delegate>> eventTable = new();
        private EventCenter() { }

        /// <summary>
        /// �����Զ���ģ�����ƣ��¼����ƺ�Action<T>��Ϊlistener���ɶ����¼���
        /// ʾ���÷���EventCenter.Instance.Subscribe<string>(EventModule.Player, PlayerEvent.StateChanged, OnStateChanged);
        /// </summary>
        public void Subscribe<T>(Enum moduleName, Enum eventName, Action<T> listener)
        {
            if (moduleName == null) throw new ArgumentNullException(nameof(moduleName));
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            string moduleKey = moduleName.ToString();
            string eventKey = eventName.ToString();

            // ����ģ��
            if (!eventTable.TryGetValue(moduleKey, out var moduleEvents))
            {
                moduleEvents = new Dictionary<string, Delegate>();
                eventTable[moduleKey] = moduleEvents;
            }

            // �Ѷ����ߺ��¼��󶨣��¼�����ֱ�Ӽ�һ��listener�������ھʹ����¼��ֶ�Ȼ���һ��listener
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
        /// ȡ�������¼�
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
        /// ָ��ģ�顢�¼����������¼�
        /// ʾ����EventCenter.Instance.Publish(EventModule.Player, PlayerEvent.StateChanged, newState.GetType().Name);
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
                        throw new InvalidOperationException($"ģ�飺{moduleName} �е��¼���{eventName} ������Action<T>");
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
