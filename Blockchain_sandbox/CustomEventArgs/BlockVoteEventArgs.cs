using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEventArgs
{
    public class BlockVoteEventArgs : EventArgs
    {
        public bool accepted { get; set; }

        public BlockVoteEventArgs(bool _accepted)
        {
            this.accepted = _accepted;
        }
    }
}
