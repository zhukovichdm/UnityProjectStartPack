using System;
using System.Collections;
using UnityEngine;
using Utility.Toolbox;

namespace Scripts.Components.Command
{
    public class InvokerEnumerator
    {
        public readonly MyAction action = new MyAction();

        private Coroutine _mainCoroutine;
        private Coroutine _commandCoroutine;
        private ICommandEnumerator _onStart;
        private ICommandEnumerator _onFinish;
        private readonly ManagerUpdateComponent _managerUpdate;

        public InvokerEnumerator()
        {
            _managerUpdate = Toolbox.Get<ManagerUpdateComponent>();
        }

        public InvokerEnumerator(ICommandEnumerator startCommand)
        {
            _managerUpdate = Toolbox.Get<ManagerUpdateComponent>();
            SetOnStart(startCommand);
        }

        public InvokerEnumerator(ICommandEnumerator startCommand, ICommandEnumerator finishCommand)
        {
            _managerUpdate = Toolbox.Get<ManagerUpdateComponent>();
            SetOnStart(startCommand);
            SetOnFinish(finishCommand);
        }


        public void SetOnStart(ICommandEnumerator command)
        {
            if (_onStart != null)
                throw new Exception("Команда уже инициализирована");
            _onStart = command;
        }

        public void SetOnFinish(ICommandEnumerator command)
        {
            if (_onStart != null)
                throw new Exception("Команда уже инициализирована");
            _onFinish = command;
        }

        public void Invoke()
        {
            // Если еще обрабатывается предыдущее нажатие, сбрасываем.
            if (InProgress())
            {
                if (_mainCoroutine != null)
                    _managerUpdate.StopCoroutine(_mainCoroutine);

                if (_commandCoroutine != null)
                    _managerUpdate.StopCoroutine(_commandCoroutine);
            }

            _mainCoroutine = _managerUpdate.StartCoroutine(Invoke());

            IEnumerator Invoke()
            {
                if (_onStart != null)
                {
                    _onStart.InProgress = true;
                    _commandCoroutine = _managerUpdate.StartCoroutine(_onStart.Execute());
                    yield return new WaitUntil(() => _onStart.InProgress == false);
                }

                if (_onFinish != null)
                {
                    _onFinish.InProgress = true;
                    _commandCoroutine = _managerUpdate.StartCoroutine(_onFinish.Execute());
                    yield return new WaitUntil(() => _onFinish.InProgress == false);
                }

                action.Publish();
            }
        }

        public bool InProgress()
        {
            bool value = false;
            if (_onStart != null)
                value = _onStart.InProgress;
            if (_onFinish != null)
                value = value || _onFinish.InProgress;

            return value;
        }
    }
}