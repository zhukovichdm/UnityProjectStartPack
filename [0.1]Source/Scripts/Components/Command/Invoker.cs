using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Toolbox;

namespace Scripts.Components.Command
{
    public partial class Invoker
    {
        public Invoker() => _invoke = ExecuteAction;
        public Invoker(ISettable iSettable) => Initialize(iSettable);
        public Invoker(ISettable iSettable, Invoker interrupt) => Initialize(iSettable, interrupt);
        public Invoker(ISettable iSettable, List<Invoker> interrupt) => Initialize(iSettable, interrupt);
        public Invoker(List<ISettable> iSettable) => Initialize(iSettable);
        public Invoker(List<ISettable> iSettable, Invoker interrupt) => Initialize(iSettable, interrupt);
        public Invoker(List<ISettable> iSettable, List<Invoker> interrupt) => Initialize(iSettable, interrupt);

        public Invoker Initialize(ISettable iSettable)
        {
            SetISettable(new List<ISettable> {iSettable});
            return this;
        }

        public Invoker Initialize(ISettable iSettable, Invoker interrupt)
        {
            SetISettable(new List<ISettable> {iSettable});
            SetInterrupt(new List<Invoker> {interrupt});
            return this;
        }

        public Invoker Initialize(ISettable iSettable, List<Invoker> interrupt)
        {
            SetISettable(new List<ISettable> {iSettable});
            SetInterrupt(interrupt);
            return this;
        }

        public Invoker Initialize(List<ISettable> iSettable)
        {
            SetISettable(iSettable);
            return this;
        }

        public Invoker Initialize(List<ISettable> iSettable, Invoker interrupt)
        {
            SetISettable(iSettable);
            SetInterrupt(new List<Invoker> {interrupt});
            return this;
        }

        public Invoker Initialize(List<ISettable> iSettable, List<Invoker> interrupt)
        {
            SetISettable(iSettable);
            SetInterrupt(interrupt);
            return this;
        }

        private Invoker SetISettable(List<ISettable> iSettable)
        {
            if (_iSettableList != null)
                throw new Exception("Список ISettable'ов для прерывания уже инициализирован");
            _iSettableList = iSettable;
            _invoke = ExecuteISettableList;

            // INFO: Получение объекта для запуска корутин. Т.к. используется Toolbox, то получение объекта и следовательно инициализация класса Invoker должно произойти до вызова Toolbox.Initialize().
            // INFO: Если Invoker инициализирован в Static классе то этот класс желательно принудительно инициализировать до Toolbox.Initialize() вызвав любой метод.
            Toolbox.Toolbox.SafelyGettingAfterInitialization(() =>
                _managerUpdate = Toolbox.Toolbox.Get<ManagerUpdateComponent>());

            return this;
        }

        private Invoker SetInterrupt(List<Invoker> interrupt)
        {
            if (_interruptingList != null)
                throw new Exception("Список Invoker'ов для прерывания уже инициализирован");

            foreach (var invoker in interrupt)
            {
                if (invoker == this)
                    throw new Exception("Нельзя добавлять Invoker в свой же список для прерывания ");
            }

            _interruptingList = interrupt;
            return this;
        }
    }

    public partial class Invoker : ISettable
    {
        public readonly MyAction action = new MyAction();

        private delegate void InvokeDelegate();

        private InvokeDelegate _invoke;
        private List<ISettable> _iSettableList;
        private List<Invoker> _interruptingList;
        private Coroutine _coroutine;
        private bool _isExecuting;
        private ManagerUpdateComponent _managerUpdate;

        public void Invoke() => _invoke();
        private void ExecuteAction() => action.Publish();

        private void ExecuteISettableList()
        {
            InterruptThis();
            InterruptList();
            // if (_managerUpdate == null) _managerUpdate = Toolbox.Toolbox.Get<ManagerUpdateComponent>();
            _coroutine = _managerUpdate.StartCoroutine(Executing(InterruptThis));
        }

        public IEnumerator Executing(Action interrupter)
        {
            _isExecuting = true;
            foreach (var iSettable in _iSettableList)
            {
                if (iSettable is IInterrupting interrupting)
                    interrupting.Interrupter = interrupter;

                if (iSettable is ICommand command)
                {
                    yield return command.Execute();
                    continue;
                }

                if (iSettable is Invoker invoker)
                {
                    yield return invoker.Executing(interrupter);
                }
            }

            ExecuteAction();
            _isExecuting = false;
        }

        private void InterruptThis()
        {
            if (_isExecuting) _managerUpdate.StopCoroutine(_coroutine);
        }

        private void InterruptList()
        {
            if (_interruptingList == null) return;
            foreach (var invoker in _interruptingList)
                invoker.InterruptThis();
        }
    }
}