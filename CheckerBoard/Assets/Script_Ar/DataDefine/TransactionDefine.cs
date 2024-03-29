using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionDefine
{
    public int ID { get; set; }
    public bool IsBlackMarket { get; set; }
    public int PurchaseOrSell { get; set; }
    public Transaction_Type TransactionType { get; set; }
    public int Subtype { get; set; }
    public int Amount { get; set; }
    public int Price { get; set; }
    public int CoolingRounds { get; set; }
}
