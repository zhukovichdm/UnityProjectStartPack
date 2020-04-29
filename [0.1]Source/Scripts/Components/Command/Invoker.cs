using System;
using Command;

namespace _Temp.Command
{
    public class Invoker
    {
        public readonly MyAction action = new MyAction();

        private ICommand _onStart;
        private ICommand _onFinish;

        public Invoker()
        {
        }

        public Invoker(ICommand startCommand)
        {
            SetOnStart(startCommand);
        }

        public Invoker(ICommand startCommand, ICommand finishCommand)
        {
            SetOnStart(startCommand);
            SetOnFinish(finishCommand);
        }


        public void SetOnStart(ICommand command)
        {
            if (_onStart != null)
                throw new Exception("Команда уже инициализирована");
            _onStart = command;
        }

        public void SetOnFinish(ICommand command)
        {
            if (_onStart != null)
                throw new Exception("Команда уже инициализирована");
            _onFinish = command;
        }

        public void Invoke()
        {
            _onStart?.Execute();
            _onFinish?.Execute();
            action.Publish();
        }
    }
}