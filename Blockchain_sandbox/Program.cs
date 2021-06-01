using System;
using System.Collections.Generic;
using System.Threading;

namespace Blockchain_sandbox
{
    class Program
    {
        static void Main(string[] args)
        {

            Transaction t = new Transaction(5.0M, Guid.NewGuid(), Guid.NewGuid());

            Console.WriteLine(t.toString());
            Console.ReadLine();

            Transaction t2 = new Transaction(10.0M, Guid.NewGuid(), Guid.NewGuid());

            Block genisis = new Block(1220421, new List<Transaction> { t , t2 });

            Console.WriteLine(genisis.toString());
            Console.ReadLine();

            List<Block> chain = new List<Block>(){ genisis };

            Node n = new Node(chain, true, 2, 30, true);

            Node n2 = new Node(chain, true, 2, 10000000, false);

            n.transactionSubmitted += n2.recieveTransaction;

            n2.transactionVoteSubmitted += n.receiveTransactionVote;

            n.blockMined += n2.receiveBlock;

            n2.blockVoteSubmitted += n.receiveBlockVote;

            n2.transactionSubmitted += n.recieveTransaction;

            n.transactionVoteSubmitted += n.receiveTransactionVote;

            n2.blockMined += n.receiveBlock;

            n.blockVoteSubmitted += n.receiveBlockVote;

            /*linkEvents(new List<Node>() { n, n2 });*/

            Console.WriteLine("Events linked");
            Console.WriteLine("Mining...");

            n.Mine();

            Console.ReadLine();
            

        }

        /*public static void linkEvents(List<Node> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].transactionSubmitted += nodes[i + 1].recieveTransaction;

                n2.transactionVoteSubmitted += n.receiveTransactionVote;

                n.blockMined += n2.receiveBlock;

                n2.blockVoteSubmitted += n.receiveBlockVote;

                n2.transactionSubmitted += n.recieveTransaction;

                n.transactionVoteSubmitted += n.receiveTransactionVote;

                n2.blockMined += n.receiveBlock;

                n.blockVoteSubmitted += n.receiveBlockVote;
            }

            n.transactionSubmitted += n2.recieveTransaction;

            n2.transactionVoteSubmitted += n.receiveTransactionVote;

            n.blockMined += n2.receiveBlock;

            n2.blockVoteSubmitted += n.receiveBlockVote;

            n2.transactionSubmitted += n.recieveTransaction;

            n.transactionVoteSubmitted += n.receiveTransactionVote;

            n2.blockMined += n.receiveBlock;

            n.blockVoteSubmitted += n.receiveBlockVote;
        }*/
    }
}
