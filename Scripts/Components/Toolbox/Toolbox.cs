using System;
using System.Collections.Generic;
using System.Linq;
using _Temp;
using UnityEngine;
using Utility.Singleton;

namespace Utility.Toolbox
{
    public class Toolbox : SingletonForMono<Toolbox>
    {
        private readonly Dictionary<Type, object> data = new Dictionary<Type, object>();
        public static void RemoveAll() => Instance.data.Clear();

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
            Instance.data.Add(obj.GetType(), obj);

            if (obj is IAwake awake)
                awake.OnAwake();
        }

        public static void Initialize()
        {
            // Получаем все значения в виде листа.
            var keys = Instance.data.Values.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                var item = keys[i];
                // Вызов метода инициализации если класс помечен интерфейсом.
                if (item is IInitialization init)
                {
                    init.Initialization();
                    // Если метод инициализации добавил в тулбокс новое значение, то для их инициализации требуется обновить список ключей. 
                    if (keys.Count != Instance.data.Count)
                        keys = Instance.data.Values.ToList();
                }
            }

//            foreach (var item in Instance.data)
//            {
//                if (item.Value is IInitialization init)
//                {
//                    init.Initialization();
//                }
//            }
        }

        public static T Get<T>()
        {
            Instance.data.TryGetValue(typeof(T), out var resolve);
            return (T) resolve;
        }

        public void ClearScene()
        {
        }
    }
}