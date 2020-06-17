using System;

namespace Scripts.Components.Command
{
    public interface IInterrupting
    {
        Action Interrupter { get; set; }
    }
}