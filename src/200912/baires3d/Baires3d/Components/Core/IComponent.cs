using System;

namespace b3d{
    internal interface IComponent : IDisposable
    {
        void Initialize();
    }
}