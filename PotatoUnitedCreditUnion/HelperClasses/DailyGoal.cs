using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PotatoUnitedCreditUnion
{
    public class DailyGoal : ICSV
    {
        public string? Message { get; set; }
        public string GoalCashString { get; set; } = "";
        public string RealCashString { get; set; } = "";

        public DateTime DateOfGoal { get; set; }

        public string GetCSVString()
        {
            return $"daily,{Message},{GoalCashString},{DateOfGoal.ToBinary()}";
        }
    }
}
