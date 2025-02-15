﻿using System;
using System.Collections.Generic;
using System.Text;
using Blockchain_sandbox.Exceptions;

namespace Blockchain_sandbox
{
    public class Block
    {
        public Guid Id { get; private set; }
        public int Hash { get; private set; }
        public int PrevHash { get; }
        public List<Transaction> Transactions { get; private set; }

        private int TransactionCount;
        private int MaxTransactions;
        public Block(int _PrevHash, List<Transaction> _Transactions)
        {
            this.Id = Guid.NewGuid();
            this.PrevHash = _PrevHash;
            this.TransactionCount = _Transactions.Count;
        }

        public void addTransaction(Transaction t)
        {
            if (this.TransactionCount < 25) {
                this.Transactions.Add(t);
                this.Hash = this.GetHashCode();
            } else {
                throw new FullBlockException();
            }
        }
    }
}
