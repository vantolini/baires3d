using System;

namespace b3d
{
    internal interface ILayerBuilder : IDisposable
    {
        void Process();
    }
}