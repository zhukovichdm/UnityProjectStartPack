using System;
using Scripts.Components.Toolbox;
using Utility.Toolbox;

namespace _Temp
{
    public abstract class SystemBase<T>
    {
        public static T Instance => Toolbox.Get<T>();

        protected SystemBase()
        {
            if (Instance != null)
                throw new Exception("Уже имеется в ToolBox");
        }
    }
}