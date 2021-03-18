using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEventArgs
{
    class BlockSubmittedEventArgs : EventArgs
    {
        Block block { get; set; }
    }
}
