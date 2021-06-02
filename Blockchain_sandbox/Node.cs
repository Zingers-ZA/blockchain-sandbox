using System;
using System.Collections.Generic;
using System.Text;
using Blockchain_sandbox.Exceptions;
using Blockchain_sandbox.CustomEventArgs;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Blockchain_sandbox
{
    class Node
    {
        public Guid Id { get; }
        public bool isMining { get; set; }
        public List<Block> blockChain { get; set; }
        public Block latestBlock { get; set; }
        public int totalNodeCount { get; set; }
        public Consensus consensus { get; set; }
        public List<TransactionValidationObject> memPool { get; set; }

        private System.Timers.Timer t { get; set; }


        public delegate void TransactionSubmittedEventHanlder(object sender, TransactionSubmittedEventArgs tArgs);
        public event TransactionSubmittedEventHanlder transactionSubmitted;

        public delegate void TransactionVoteSubmittedEventHanlder(object sender, TransacionVoteEventArgs tArgs);
        public event TransactionVoteSubmittedEventHanlder transactionVoteSubmitted;

        public delegate void BlockMinedEventHandler(object sender, BlockMinedEventArgs bArgs);
        public event BlockMinedEventHandler blockMined;

        public delegate void BlockSubmittedEventHandler(object sender, BlockSubmittedEventArgs bArgs);
        public event BlockSubmittedEventHandler blockSubmitted;

        public delegate void BlockVoteSubmittedEventHandler(object sender, BlockVoteEventArgs bArgs);
        public event BlockVoteSubmittedEventHandler blockVoteSubmitted;


        public Node(List<Block> _blockChain, bool _isMining, int _totalNodeCount)
        {
            this.Id = Guid.NewGuid();
            this.isMining = _isMining;
            this.blockChain = _blockChain;
            this.latestBlock = this.blockChain.Last();
            this.totalNodeCount = _totalNodeCount;
            this.consensus = new Consensus();
            this.memPool = new List<TransactionValidationObject>();

            this.t = new System.Timers.Timer();
            t.Elapsed += createBlock;
           
            this.Mine();
        }

        public void Mine()
        {
            this.consensus = new Consensus();
            if (t.Enabled)
            {
                t.Stop();
            }

            setInterval();
           
            this.t.Start();
            Console.WriteLine($"Node-{this.Id}: Mining...");
        }

        public void setInterval()
        {
            Random rnd = new Random();
            int time = rnd.Next(2, 25);
            t.Interval = time*1000;
        }

        private void createBlock(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"Node-{this.Id}: Nounce Found");
            this.pruneMempool();

            Block minedBlock = new Block(this.latestBlock.Hash, this.memPool.Select((t) => t.transaction).ToList());
            Console.WriteLine($"Node-{this.Id}: Block created: \n{minedBlock.toString()}");
            

            BlockMinedEventHandler handler = blockMined;
            BlockMinedEventArgs bArgs = new BlockMinedEventArgs(minedBlock);

            castBlockVote(true, minedBlock);
            this.consensus.votesAccepted++;

            handler?.Invoke(this, bArgs);

        }

        public void checkConsensus(Block block)
        {
            if (this.consensus.votesRejected + this.consensus.votesAccepted == this.totalNodeCount)
            {
                Console.WriteLine($"Node-{this.Id}: Consensus Reached. Votes: a-{this.consensus.votesAccepted}, r-{this.consensus.votesRejected}");

                this.consensus.reached = true;
                if (this.consensus.votesAccepted > this.consensus.votesRejected)
                {
                    this.memPool.Clear();
                    this.blockChain.Add(block);
                    this.latestBlock = block;

                    Console.WriteLine($"Node-{this.Id}: Block added to chain. ");
                    Mine();
                }
            }
            else
            {
                Console.WriteLine($"Node-{this.Id}: Consensus not yet reached. Votes: a-{this.consensus.votesAccepted}, r-{this.consensus.votesRejected}");
            }
        }

        public void receiveBlock(object sender, BlockMinedEventArgs bArgs)
        {
            if (validateBlock(bArgs.block))
            {
                Console.WriteLine($"Node-{this.Id}: Validated block");
                this.consensus.votesAccepted++;
                castBlockVote(true, bArgs.block);
                
            } else
            {
                Console.WriteLine($"Node-{this.Id}: Rejected block");
                this.consensus.votesRejected++;
                castBlockVote(false, bArgs.block);
            }

            checkConsensus(bArgs.block);
        }

        public void castBlockVote(bool accepted, Block block)
        {
            BlockVoteSubmittedEventHandler handler = blockVoteSubmitted;
            BlockVoteEventArgs bArgs = new BlockVoteEventArgs(accepted, block);
            handler?.Invoke(this, bArgs);
        }

        public void receiveBlockVote(object sender, BlockVoteEventArgs bArgs)
        {
            Console.WriteLine($"Node-{this.Id}: Block vote received: {(bArgs.accepted ? "Accepted" : "Rejected")}");
            if (bArgs.accepted)
            {
                this.consensus.votesAccepted++;
            } else
            {
                this.consensus.votesRejected++;
            }

            checkConsensus(bArgs.block);
        }

        public bool validateBlock(Block block)
        {
            
            if (block.PrevHash != this.latestBlock.Hash)
            {
                return false;
            }

            // blocks with 0 transactions could potentially get added because of this for,
            // is that wrong?

            foreach (Transaction t in block.Transactions)
            {
                if (!validateTransaction(t))
                    return false;
            }

            return true;

        }

        public void pruneMempool()
        {
             this.memPool = this.memPool.Where((t) => { return t.validVotes > t.invalidVotes; }).ToList();
        }

        public void test()
        {
            Task.WaitAny(new Task(this.Mine));
        }

        public void submitTransaction(Decimal amount, Guid RecieverId)
        {
            Transaction t = new Transaction(amount, RecieverId, this.Id);

            TransactionSubmittedEventHanlder handler = transactionSubmitted;
            TransactionSubmittedEventArgs tArgs = new TransactionSubmittedEventArgs(t);
            handler?.Invoke(this, tArgs);
        }

        public void recieveTransaction(object sender, TransactionSubmittedEventArgs tArgs)
        {
            this.memPool.Add(new TransactionValidationObject(tArgs.transaction));

            TransactionVoteSubmittedEventHanlder handler = transactionVoteSubmitted;
            TransacionVoteEventArgs vArgs;

            if (this.validateTransaction(tArgs.transaction))
            {
                vArgs = new TransacionVoteEventArgs(tArgs.transaction.Id, true);
            }
            else
            {
                vArgs = new TransacionVoteEventArgs(tArgs.transaction.Id, true);
            }
            handler?.Invoke(this, vArgs);
        }

        public void receiveTransactionVote(object sender, TransacionVoteEventArgs vArgs)
        {
            for (int i = 0; i < memPool.Count; i++)
            {
                if (memPool[i].transaction.Id == vArgs.transactionId)
                {
                    if (vArgs.accepted)
                        memPool[i].validVotes++;
                    else
                        memPool[i].invalidVotes++;
                }
            }
        }

        public bool validateTransaction(Transaction t)
        {
            Decimal senderAccount = 0M;
            List<Transaction> senderTransactions = new List<Transaction>();

            for (int i = 0; i < blockChain.Count; i++)
            {
                for (int j = 0; j < blockChain[i].Transactions.Count; j++)
                {
                    if (blockChain[i].Transactions[j].RecieverId == t.SenderId ||
                        blockChain[i].Transactions[j].SenderId == t.SenderId)
                    {
                        senderTransactions.Add(blockChain[i].Transactions[j]);
                    }
                }
            }

            for (int i = 0; i < senderTransactions.Count; i++)
            {
                if (senderTransactions[i].RecieverId == t.SenderId)
                {
                    senderAccount += senderTransactions[i].Amount;
                }
                if (senderTransactions[i].SenderId == t.SenderId)
                {
                    senderAccount -= senderTransactions[i].Amount;
                }
            }

            if (senderAccount > t.Amount)
                return true;

            return false;
        }

    }
}
