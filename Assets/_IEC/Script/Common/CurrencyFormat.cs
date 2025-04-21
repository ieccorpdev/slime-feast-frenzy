using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyFormat
{
    public static string FormatMoney(int amount)
    {
        var formattedAmount = amount.ToString();
        var divide = 1f;
        var symbol = "";
        // switch (amount)
        // {
        //     case < 10_000:
        //         return formattedAmount;
        //     case < 1_000_000:
        //         divide = 1000f;
        //         symbol = "k";
        //         break;
        //     case < 1_000_000_000:
        //         divide = 1_000_000f;
        //         symbol = "m";
        //         break;
        //     default:
        //         divide = 1_000_000_000f;
        //         symbol = "b";
        //         break;
        // }

        var div = amount / divide;

        // formattedAmount = div switch
        // {
        //     < 10 => $"{div:0.00}{symbol}",
        //     < 100 => $"{div:0.0}{symbol}",
        //     _ => $"{div}{symbol}"
        // };
        return formattedAmount;
    }

    public static string FormatTime(int seconds)
    {
        var days = seconds / 86400;
        var hours = (seconds % 86400) / 3600;
        var minutes = (seconds % 3600) / 60;
        var remainingSeconds = seconds % 60;

        return 
            days > 0 ? $"{days:0}d {hours:00}h" :
            hours > 0 ? $"{hours:0}h {minutes:00}m" : $"{minutes:0}:{remainingSeconds:00}";
    }
}
