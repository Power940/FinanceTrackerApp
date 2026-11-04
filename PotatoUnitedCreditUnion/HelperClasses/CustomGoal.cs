using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PotatoUnitedCreditUnion
{
    public class CustomGoal : ICSV, INotifyPropertyChanged
    {
        public string? Message { get; set; }
        public string GoalCashString { get; set; } = "";
        public string RealCashString { get; set; } = "";

        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }

        public Color RealCashStringColor { get; set; } = Colors.Green;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string GetCSVString()
        {
            return $"custom,{Message},{GoalCashString},{DateTimeStart.ToBinary()},{DateTimeEnd.ToBinary()}";
        }
    }
}
