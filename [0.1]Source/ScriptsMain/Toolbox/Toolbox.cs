using System;
using System.Collections.Generic;
using System.Linq;
using _Temp;
using Utility.Singleton;
using Utility.Toolbox;

namespace Scripts.Components.Toolbox
{
    public class Toolbox : SingletonForMono<Toolbox>
    {
        private static readonly MyAction SafelyGettingAction = new MyAction();

        private readonly Dictionary<Type, object> _data = new Dictionary<Type, object>();

        public static void RemoveAll()
        {
            Instance._data.Clear();
            SafelyGettingAction.ClearListener();
        }

        public static void AddManager(object obj)
        {
            var manager = obj as ManagerBase;
            if (manager != null)
            {
                obj = Instantiate(manager);
            }
            else return;

            Add(obj);
        }

        public static void Add(object obj)
        {
            Instance._data.Add(obj.GetType(), obj);

            if (obj is IAwake awake)
                awake.OnAwake();
        }

        public static void Initialize()
        {
            // Получаем все значения в виде листа.
            var keys = Instance._data.Values.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                var item = keys[i];
                // Вызов метода инициализации если класс помечен интерфейсом.
                if (item is IInitialization init)
                {
                    init.Initialization();
                    // Если метод инициализации добавил в тулбокс новое значение, то для их инициализации требуется обновить список ключей. 
                    if (keys.Count != Instance._data.Count)
                        keys = Instance._data.Values.ToList();
                }
            }

            SafelyGettingAction.Publish();
        }

        public static void SafelyGettingAfterInitialization(Action action)
        {
            SafelyGettingAction.Subscribe(action);
        }

        public static T Get<T>()
        {
            Instance._data.TryGetValue(typeof(T), out var resolve);
            return (T) resolve;
        }

        public void ClearScene()
        {
        }
    }
}