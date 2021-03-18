using System;
using System.Collections.Generic;
using System.Text;
using Blockchain_sandbox.Exceptions;
using Blockchain_sandbox.CustomEventArgs;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain_sandbox
{
    class Node
    {
        public Guid Id { get; }
        public bool isMining { get; set; }
        public List<Block> blockChain { get; set; }
        public int totalNodeCount { get; set; }
        public Consensus consensus { get; set; }
        public List<TransactionValidationObject> memPool { get; set; }

        public event EventHandler blockSubmitted;

        public event EventHandler transactionSubmitted;

        public event EventHandler transactionVoteSubmitted;

        public event EventHandler blockVoteSubmitted;

        public event EventHandler blockMined;

        public Node(List<Block> _blockChain, bool _isMining, int _totalNodeCount)
        {
            this.Id = Guid.NewGuid();
            this.isMining = _isMining;
            this.blockChain = _blockChain;
            this.totalNodeCount = _totalNodeCount;
            this.consensus = new Consensus();
        }

        public void Mine()
        {
            // is there a better way to run a timer? Not sure there are enough threads for this
            Task.WaitAny(new Task(() => {
                Random rnd = new Random();
                int time = rnd.Next(50);
                Thread.Sleep(time * 1000);
            }));

            createBlock();
        }

        public void createBlock()
        {
            this.pruneMempool();

            Block minedBlock = new Block(this.blockChain[-1].Hash, this.memPool.Select((t) => t.transaction).ToList());

            EventHandler handler = blockMined;
            BlockMinedEventArgs bArgs = new BlockMinedEventArgs(minedBlock);
            handler?.Invoke(this, bArgs);
        }

        public void receiveBlock(object sender, BlockMinedEventArgs bArgs)
        {
            if (validateBlock(bArgs.block))
            {
                castBlockVote(true);
            } else
            {
                castBlockVote(false);
            }

            awaitConsensus();

            if (this.consensus.reached)
            {
                if(this.consensus.votesAccepted > this.consensus.votesRejected)
                {
                    this.memPool.Clear();
                    this.blockChain.Add(bArgs.block);
                }
            }
        }

        public void castBlockVote(bool accepted)
        {
            EventHandler handler = blockVoteSubmitted;
            BlockVoteEventArgs bArgs = new BlockVoteEventArgs(accepted);
            handler?.Invoke(this, bArgs);
        }

        public void receiveBlockVote(object sender, BlockVoteEventArgs bArgs)
        {
            if (bArgs.accepted)
            {
                this.consensus.votesAccepted++;
            } else
            {
                this.consensus.votesRejected++;
            }
        }

        public void awaitConsensus()
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(50);
                if ((this.consensus.votesAccepted + this.consensus.votesRejected - 1) == this.totalNodeCount)
                {
                    this.consensus.reached = true;
                    return;
                }
            }
        }

        public bool validateBlock(Block block)
        {
            
            if (block.PrevHash != this.blockChain[-1].Hash)
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

            EventHandler handler = transactionSubmitted;
            TransactionEventArgs tArgs = new TransactionEventArgs(t);
            handler?.Invoke(this, tArgs);
        }

        public void recieveTransaction(object sender, TransactionEventArgs tArgs)
        {
            this.memPool.Add(new TransactionValidationObject(tArgs.transaction));

            EventHandler handler = transactionVoteSubmitted;
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



       /* public void submitBlock(Block b)
        {
            EventHandler handler = blockSubmitted;
            BlockSubmittedEventArgs bArgs = new BlockSubmittedEventArgs();
            handler?.Invoke(this, bArgs);
        }

         

        public void validateBlock(Block b)
        {
            bool validHashes;
            bool validTransactions;
            bool valid;
            

            

            if (b.Id == this.blockChain[-1].Id)
            { // the latest block is being updated
                if (b.PrevHash == this.blockChain[-2].Hash)
                {

                }

            } else 
            { // a new block has been created



            }

            for (int i = blockChain.Count-1; i > -1; i--)
            {
                
            }
        }*/
    }
}
