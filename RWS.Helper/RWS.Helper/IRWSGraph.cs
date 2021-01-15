using System;
using System.Collections.Generic;
using System.Text;

namespace RWS.Helper
{
    interface IRWSGraph<T> where T: class
    {
        T Data { get; set; }
        List<IRWSGraph<T>> Edges { get; set; }
    }
}
