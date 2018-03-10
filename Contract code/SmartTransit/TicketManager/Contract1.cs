using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using Neo.SmartContract.Framework;
using System.Numerics;
using System;
using System.ComponentModel;

namespace Neo.SmartContract
{
    /* Author: D. Sfounis, for the NEO Microsoft Contest
     *  License: GNU GPL v3
     *  https://dsfounis.com
     */
    public class Contract1 : Framework.SmartContract
    {
        /* Defines */
        //NEO Asset Hex: c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b - NOTE: It should be reversed.
        private static readonly byte[] NEO_ASSET_ID = { 155, 124, 255, 218, 166, 116, 190, 174, 15, 147, 14, 190, 96, 133, 175, 144, 147, 229, 254, 86, 179, 74, 92, 34, 12, 205, 207, 110, 252, 51, 111, 197 };
        //GAS Asset Hex: 602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7 - NOTE: Again, should be reversed.
        private static readonly byte[] GAS_ASSET_ID = { 231, 45, 40, 105, 121, 238, 108, 177, 183, 230, 93, 253, 223, 178, 227, 132, 16, 11, 141, 20, 142, 119, 88, 222, 66, 228, 22, 139, 113, 121, 44, 96 };

        private static readonly byte[] OPERATION_FAULT = { 5, 4, 3, 2, 1 };
        private static readonly byte[] REFUND_FLAG = { 9, 8, 7 };
        private static readonly int TICKET_DURATION = 3600;
        private static readonly int TICKET_PRICE = 20000000; //In GAS*10^8, so that's 0.2 GAS

        public static readonly byte[] owner_address = "AehieVzYk9zajxsUqM3vwXj2YtVPucZ1Xt".ToScriptHash();

        /* Events */
        [DisplayName("DoRefund")]
        public static event Action<byte[], BigInteger> Refunded; // Params: (address, amount)

        /*public struct Commuter //Unused
        {
            public byte[] addressHash;
            public byte[] toRefund;
            public int ticketsPurchasedTotal;
        }*/

        /* Params: 0710
         * Returns: 10
         */
        public static Object Main(string operation, params object[] args)
        {
            /* DEBUG */
            /*
            Runtime.Notify("TicketManager invoked, operation: ", operation);
            Runtime.Notify("OWNER_ADDRESS: ", owner_address, " OF LENGTH: ", owner_address.Length);
            Runtime.Notify("Single byte - pos0: ", owner_address[0]);
            */

            /* During Verification phase, the contract will reject (and refund) any transaction that isn't invoked
             * by the "owner" of the contract, the one that holds its private key. This is crucial so that you can
             * transfer gas and other assets OUT of the contract
             * TODO:
             * 1. Accept only owner transactions OR
               2. Accept public transactions if they have an outstanding Refund amount.
             */
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return Runtime.CheckWitness(owner_address);
            }
            /* Application type invocations are different. Here, returning false does not trigger a refund of transaction.
             * Therefore, we need to be extra careful here, as we're receiving somebody else's assets, and he/she expects
             * something back.
             * TODO:
             *      a) Deny transactions less than the "required" amount for a ticket
             *      b) Refund excess amounts in case of already-active-ticket or overtransfer
             */
             /* ### NOTE: Invoke Contract with GAS both a Verification and Application transaction? */
            else if (Runtime.Trigger == TriggerType.Application)
            {
                //Returns the total number of issued tickets
                //TODO: Store and count only ACTIVE tickets (diff in time >= 0)
                if (operation == "ticket_stats")
                {
                    byte[] greetings = Storage.Get(Storage.CurrentContext, "TicketCount");
                    return greetings.AsBigInteger();
                }
                else if (operation == "ticket_purchase")
                {
                    //Before we do anything, we check if there's a transaction attached to the invocation (You need to send GAS if you want to buy a ticket, eh?)
                    Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
                    //TransactionOutput reference = tx.GetReferences()[0]; //RPX ICO used pos[0] of the array to get the Sender, but I doubt this is universallly correct.
                    //We have to iterate the References to find the sender.
                    TransactionOutput[] references = tx.GetReferences();
                    if(references.Length == 0)  //No transaction was attached.
                    {
                        Runtime.Notify("Transaction not attached to invocation!");
                        return 0x1;
                    }
                    byte[] this_commuter = CheckTxRefsForWitness(references);
                    Runtime.Notify("COMMUTER_ADDRESS_LENGTH: ", this_commuter.Length);
                    Runtime.Notify("COMMMUTER_ADDRESS SCRIPTHASH: ", this_commuter);
                    //Additionally, we dynamically create a Refund address in case we need it later. It's a concatenation of 3 FLAG bytes, followed by the sender's actual address.
                    byte[] toRefund = Framework.Helper.Concat(REFUND_FLAG, this_commuter);

                    //We only accept GAS, nothing else.
                    //NOTE: Not needed, we count the GAS given later on, anyway, so this is superfluous.
                    //TODO: Need to find a solution for refunding tokens other than GAS, or combined transactions
                    /*if (reference.AssetId != GAS_ASSET_ID)
                    {
                        //TODO: Refund()
                        return 0x2;
                    }*/

                    //First, let's check the witness against the maker of the transaction. If the caller isn't the transaction input, we don't continue.
                    if (!Runtime.CheckWitness(this_commuter))
                    {
                        //TODO: Don't use magic numbers.
                        return 0x3;
                    }
                    //Let's see if our commuter already has a ticket already issued.
                    
                    //User is indeed the invoker and doesn't have an active ticket. Let's get him one.
                    ulong gas_given = GetGasGiven();
                    //If the gas is enough, issue a ticket for the commuter. If not, refund the change.
                    if (CheckForActiveTicket(this_commuter))
                    {
                        Refund(this_commuter, gas_given);
                        Refunded(this_commuter, gas_given);
                        return 0x4;
                    }
                    if ((int)gas_given >= TICKET_PRICE)
                    {
                        IssueTicket(this_commuter);
                        //Refund any difference
                        int diff = (int)gas_given - TICKET_PRICE;
                        if ( diff > 0)
                        {
                            Refund(this_commuter, (ulong)diff);
                            Refunded(this_commuter, (ulong)diff);
                        }
                        AddToActiveTickets();
                        return 0x5;
                    }
                    else
                    {
                        Refund(this_commuter, gas_given);
                        Refunded(this_commuter, gas_given);
                        return 0x6;
                    }
                }
                else if(operation == "check_for_ticket")
                {
                    //The address for which we look for an active ticket, is passed in args[0]
                    byte[] commuter = (byte[])args[0];
                    return CheckForActiveTicket(commuter);
                }
                else if(operation == "get_total_tickets")
                {
                    BigInteger total = Storage.Get(Storage.CurrentContext, "TicketCount").AsBigInteger();
                    return total;
                }
            }
            return false;
        }

        /* Setters and Getters for Ticket duration and price,
         * so we don't have to redeploy the contract every time the Bus Manager wants to change these properties
         */
        public void SetTicketDuration(int d)
        {
            if (Runtime.CheckWitness(owner_address))
            {
                Storage.Put(Storage.CurrentContext, "TICKET_DURATION", d);
            }
        }

        public void SetTicketPrice(int p)
        {
            if (Runtime.CheckWitness(owner_address))
            {
                Storage.Put(Storage.CurrentContext, "TICKET_PRICE", p);
            }
        }

        public int GetTicketDuration()
        {
            return TICKET_DURATION;
        }

        public int GetTicketPrice()
        {
            return TICKET_PRICE;
        }

        //Marks an amount to be refunded to the user.
        //Called when the user sends not-enough or too-much GAS.
        private static void Refund(byte[] toRefund, ulong amount)
        {
            byte[] sanitized = toRefund.Range(3, 23);
            BigInteger existing = Storage.Get(Storage.CurrentContext, toRefund).AsBigInteger();
            existing += amount;
            Storage.Put(Storage.CurrentContext, "TICKET_DURATION", existing);
        }

        //Iterates through our TransactionReferences (if attached), and returns the first address that is
        //a witness and a transaction sender at the same time. This validates the returned address as the true invoker and sender of funds.
        private static byte[] CheckTxRefsForWitness(TransactionOutput[] refs)
        {
            foreach(TransactionOutput this_ref in refs)
            {
                if (Runtime.CheckWitness(this_ref.ScriptHash))
                {
                    return this_ref.ScriptHash;
                }
            }
            return OPERATION_FAULT;
        }

        /* Checks if there's already an issued and active ticket for the specified commmuter */
        private static bool CheckForActiveTicket(byte[] guy)
        {
            BigInteger last = Storage.Get(Storage.CurrentContext, guy).AsBigInteger();
            uint now = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
            Runtime.Notify(now);
            int diff = ((int)last + TICKET_DURATION) - (int)now;
            Runtime.Notify(diff);
            if (diff > 0) { return true; }
            return false;
        }

        private static ulong GetGasGiven()
        {
            Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
            TransactionOutput reference = tx.GetReferences()[0];
            TransactionOutput[] outputs = tx.GetOutputs();
            ulong value = 0;
            //Get the total amount of GAS sent
            foreach (TransactionOutput output in outputs)
            {
                /* DEBUG */
                /*Runtime.Notify("InLoop: output.scriptHash: ",output.ScriptHash);
                Runtime.Notify("InLoop: ExecutionEngine.ExecutingScriptHash: ", GetReceiver());
                */
                if (output.ScriptHash == GetReceiver() && output.AssetId == GAS_ASSET_ID)
                {
                    value += (ulong)output.Value;
                }
            }
            //Value is multiplied by 10^8 to signify decimals. Keep that in mind!
            //Runtime.Notify(value);
            return value;
        }

        private static void IssueTicket(byte[] user)
        {
            uint now = Blockchain.GetHeader(Blockchain.GetHeight()).Timestamp;
            Storage.Put(Storage.CurrentContext, user, now);
        }

        private static void AddToActiveTickets()
        {
            BigInteger current = Storage.Get(Storage.CurrentContext, "TicketCount").AsBigInteger();
            current += 1;
            Storage.Put(Storage.CurrentContext, "TicketCount", current);
        }

        //Get SmartContract script-hash
        private static byte[] GetReceiver()
        {
            return ExecutionEngine.ExecutingScriptHash;
        }
    }
}
