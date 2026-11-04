using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PotatoUnitedCreditUnion
{
    public class CustomGoal : ICSV
    {
        public string? Message { get; set; }
        public string GoalCashString { get; set; } = "";
        public string RealCashString { get; set; } = "";

        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }

        public string GetCSVString()
        {
            return $"custom,{Message},{GoalCashString},{DateTimeStart.ToBinary()},{DateTimeEnd.ToBinary()}";
        }
    }
}
