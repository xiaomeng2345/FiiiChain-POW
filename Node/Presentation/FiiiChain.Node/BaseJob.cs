using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Node
{
    public abstract class BaseJob
    {
        public abstract void Start();
        public abstract void Stop();
        public abstract JobStatus Status { get;}
    }
}
