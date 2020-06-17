using System;

namespace Scripts.Component
{
    [Serializable]
    public enum CameraModes
    {
        LooksAt, // Выставляется через LookAt объекты. Двигает камеру без возможности управления ей.
        Player,
        Free,
        Pivot
    }
}