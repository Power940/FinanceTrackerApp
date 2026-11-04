using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PotatoUnitedCreditUnion
{
    public class DailyGoal : ICSV, INotifyPropertyChanged
    {
        public string? Message { get; set { field = value; OnPropertyChanged(); } }
        public string GoalCashString { get; set { field = value; OnPropertyChanged(); } } = "";
        public string RealCashString { get; set { field = value; OnPropertyChanged(); } } = "";

        public DateTime DateOfGoal { get; set { field = value; OnPropertyChanged(); } }

        public Color RealCashStringColor { get; set { field = value; OnPropertyChanged(); } } = Colors.Green;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string GetCSVString()
        {
            return $"daily,{Message},{GoalCashString},{DateOfGoal.ToBinary()}";
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
