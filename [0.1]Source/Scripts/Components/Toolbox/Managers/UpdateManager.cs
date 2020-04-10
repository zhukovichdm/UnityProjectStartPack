using System;
using System.Collections.Generic;
using _Temp;
using UnityEngine;

namespace Utility.Toolbox
{
    [CreateAssetMenu(fileName = "UpdateManager", menuName = "Managers/UpdateManager")]
    public partial class UpdateManager : ManagerBase, IAwake
    {
        public void Tick()
        {
            for (int i = 0; i < ticks.Count; i++)
                ticks[i].Tick();
            tickAction.Publish();
        }

        public void FixedTick()
        {
            for (int i = 0; i < fixedTicks.Count; i++)
                fixedTicks[i].FixedTick();
            fixedTickAction.Publish();
        }

        public void OnAwake()
        {
            var go = new GameObject("[ManagerUpdate]");
            var mngComponent = go.AddComponent<ManagerUpdateComponent>();
            mngComponent.Setup(this);
            Toolbox.Add(mngComponent);
        }
    }

    // Реализация на списках. Добавляет только методы интерфейсов ITick, IFixedTick и т.д. 
    public partial class UpdateManager
    {
        private readonly List<ITick> ticks = new List<ITick>();
        private readonly List<IFixedTick> fixedTicks = new List<IFixedTick>();

        public static void AddTo(object updatable)
        {
            var mngUpdate = Toolbox.Get<UpdateManager>();

            if (updatable is ITick tick)
                mngUpdate.ticks.Add(tick);

            if (updatable is IFixedTick fixedTick)
                mngUpdate.fixedTicks.Add(fixedTick);
        }

        public static void RemoveFrom(object updatable)
        {
            var mngUpdate = Toolbox.Get<UpdateManager>();

            if (updatable is ITick tick)
                mngUpdate.ticks.Remove(tick);

            if (updatable is IFixedTick fixedTick)
                mngUpdate.fixedTicks.Remove(fixedTick);
        }
    }

    // Реализация на событиях. Позволяет добавлять в апдейт лямбды и сторонние методы с классов
    // не реализующих ITick, IFixedTick и т.д. 
    public partial class UpdateManager
    {
        private readonly MyAction tickAction = new MyAction();
        private readonly MyAction fixedTickAction = new MyAction();

        private readonly List<Action> allTicks = new List<Action>();

        public static void SubscribeTo(object updatable)
        {
            var mngUpdate = Toolbox.Get<UpdateManager>();

            if (updatable is ITick tick)
            {
                mngUpdate.tickAction.Subscribe(tick.Tick);
                mngUpdate.allTicks.Add(tick.Tick);
            }

            if (updatable is IFixedTick fixedTick)
            {
                mngUpdate.fixedTickAction.Subscribe(fixedTick.FixedTick);
                mngUpdate.allTicks.Add(fixedTick.FixedTick);
            }
        }

        public static void UnSubscribeFrom(object updatable)
        {
            var mngUpdate = Toolbox.Get<UpdateManager>();

            if (updatable is ITick tick)
            {
                mngUpdate.tickAction.Unsubscribe(tick.Tick);
                mngUpdate.allTicks.Remove(tick.Tick);
            }

            if (updatable is IFixedTick fixedTick)
            {
                mngUpdate.fixedTickAction.Unsubscribe(fixedTick.FixedTick);
                mngUpdate.allTicks.Remove(fixedTick.FixedTick);
            }
        }

        public static void SubscribeTo(Type type, Action action)
        {
            var mngUpdate = Toolbox.Get<UpdateManager>();

            if (type == typeof(ITick))
            {
                mngUpdate.tickAction.Subscribe(action);
                mngUpdate.allTicks.Add(action);
            }

            if (type == typeof(IFixedTick))
            {
                mngUpdate.fixedTickAction.Subscribe(action);
                mngUpdate.allTicks.Add(action);
            }
        }

        public static void UnSubscribeFrom(Type type, Action action)
        {
            var mngUpdate = Toolbox.Get<UpdateManager>();

            if (type == typeof(ITick))
            {
                mngUpdate.tickAction.Unsubscribe(action);
                mngUpdate.allTicks.Remove(action);
            }

            if (type == typeof(IFixedTick))
            {
                mngUpdate.fixedTickAction.Unsubscribe(action);
                mngUpdate.allTicks.Remove(action);
            }
        }
    }
}