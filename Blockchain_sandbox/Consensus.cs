using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain_sandbox
{
    public class Consensus
    {
        public int votesAccepted { get; set; }
        public int votesRejected { get; set; }
        public bool pending { get; set; }
        public bool reached { get; set; }

        public Consensus()
        {
            this.votesAccepted = 0;
            this.votesRejected = 0;
            this.pending = true;
            this.reached = false;
        }

    }
}
