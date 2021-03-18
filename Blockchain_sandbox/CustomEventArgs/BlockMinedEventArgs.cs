using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEventArgs
{
    public class BlockMinedEventArgs : EventArgs
    {
        public Block block { get; set; }

        public BlockMinedEventArgs(Block _block)
        {
            this.block = _block;
        }
    }
}
