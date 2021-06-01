using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox.CustomEvents
{
    public class TransactionSubmittedEventHandler 
    {
        public Block block { get; set; }

        public TransactionSubmittedEventHandler(Block _block)
        {
            this.block = _block;
        }
    }
}
