using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PotatoUnitedCreditUnion
{
    public class TransactionItem : ICSV, INotifyPropertyChanged
    {
        public string? Message { get; set; }

        // stores whole part of the cash as a uint
        // stores the cents as the first 2 digits of a byte, the 3rd digit represents if the ammount is positive or negative (0 == positive, else negative)
        public uint AmmountPrimary { get; set { field = value; OnPropertyChanged(); } } = 0;
        public byte AmountSecondary { get; set { field = value; OnPropertyChanged(); } } = 0;
        public string CashString { get; set { field = value; OnPropertyChanged(); } } = "";

        public DateTime DateTimeOfItem { get; set { field = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void UpdateCashString()
        {
            if (AmmountPrimary == 0 && AmountSecondary % 100 == 0) { CashString = "$0.00"; return; }
            CashString = $"${((AmountSecondary / 100 == 0) ? "+" : "-")}{AmmountPrimary}.{(AmountSecondary % 100).ToString().PadLeft(2,'0')}";
        }

        public string GetCSVString()
        {
            return $"{Message},{AmmountPrimary},{AmountSecondary},{DateTimeOfItem.ToBinary()}";
        }

        public long ToLong()
        {
            if (AmountSecondary / 100 == 0)
                return AmmountPrimary * 100 + AmountSecondary;
            return -((AmmountPrimary * 100) + (AmountSecondary % 100));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    };
}
