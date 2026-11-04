using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PotatoUnitedCreditUnion
{
    public class CustomGoal : ICSV, INotifyPropertyChanged
    {
        public string? Message { get; set { field = value; OnPropertyChanged(); } }
        public string GoalCashString { get; set { field = value; OnPropertyChanged(); } } = "";
        public string RealCashString { get; set { field = value; OnPropertyChanged(); } } = "";

        public DateTime DateTimeStart { get; set { field = value; OnPropertyChanged(); } }
        public DateTime DateTimeEnd { get; set { field = value; OnPropertyChanged(); } }

        public Color RealCashStringColor { get; set { field = value; OnPropertyChanged(); } } = Colors.Green;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string GetCSVString()
        {
            return $"custom,{Message},{GoalCashString},{DateTimeStart.ToBinary()},{DateTimeEnd.ToBinary()}";
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
