using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionUpgradeDefine
{
    public int Level { get; set; }
    public Upgrade_Type UpgradeType { get; set; }
    public int TransactionUpgradeCost { get; set; }
    public int PurchasePriceReduce { get; set; }
    public int SellPriceIncrease { get; set; }
    public string TransactionSpecialEffectDescription { get; set; }
    public int TransactionSpecialEffectValue { get; set; }
}
