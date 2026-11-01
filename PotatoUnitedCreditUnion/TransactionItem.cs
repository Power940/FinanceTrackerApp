using System;
using System.Collections.Generic;
using System.Text;

namespace PotatoUnitedCreditUnion
{
    public class TransactionItem
    {
        public string? Message { get; set; }

        // stores whole part of the cash as a uint
        // stores the cents as the first 2 digits of a byte, the 3rd digit represents if the ammount is positive or negative (0 == positive, else negative)
        public uint AmmountPrimary { get; set; } = 0;
        public byte AmountSecondary { get; set; } = 0;
        public string CashString { get; set; } = "";

        public DateTime DateTimeOfItem { get; set; }


        public void UpdateCashString()
        {
            if (AmmountPrimary == 0 && AmountSecondary % 100 == 0) { CashString = ""; return; }
            CashString = $"${((AmountSecondary / 100 == 0) ? "+" : "-")}{AmmountPrimary}.{AmountSecondary % 100}";
        }

        public string GetCSVString()
        {
            return $"{Message},{AmmountPrimary},{AmountSecondary},{DateTimeOfItem.ToBinary()}";
        }
    };
}
