using System;
using Command;

namespace _Temp.Command
{
    public class Invoker
    {
        public readonly MyAction action = new MyAction();

        private ICommand onStart;
        private ICommand onFinish;

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
            if (onStart != null)
                throw new Exception("Команда уже инициализирована");
            onStart = command;
        }

        public void SetOnFinish(ICommand command)
        {
            if (onStart != null)
                throw new Exception("Команда уже инициализирована");
            onFinish = command;
        }

        public void Invoke()
        {
            onStart?.Execute();
            onFinish?.Execute();
            action.Publish();
        }
    }
}