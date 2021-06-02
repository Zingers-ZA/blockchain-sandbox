using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEventArgs
{
    public class BlockVoteEventArgs : EventArgs
    {
        public bool accepted { get; set; }
        public Block block { get; set; }

        public BlockVoteEventArgs(bool _accepted, Block _block)
        {
            this.accepted = _accepted;
            this.block = _block;
        }
    }
}
