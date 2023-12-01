using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemDataSO : ScriptableObject
{
    private int itemHeldIn;

    public int ItemHeldIn { get => itemHeldIn; set => itemHeldIn = value; }

    [Header("Item Details")]
    public int id;
    public string itemName;
    public string description;
    public string itemType;
    public string itemClass;
    public int itemAct;
    public string rarity;
    public string itemPriceType;
    public int priceToBuy;
    public int priceToSell;
    public string itemSeller;
    public Sprite itemSprite;
    public int dropChance;

    [Header("Item Affects")]
    public int afMaxHP;
    public int afMaxMana;
    public int afVita;
    public int afStr;
    public int afDex;
    public int afEnergy;
    public int afIntelligence;
    public int afDefence;
    public int afPhysicalDmg;
    public int afMagicalDmg;
    public int afCritChance;
    public int afMissChance;
    public int afManaCostReduce;
    public float afCritMultiplier;
    public float afXPMultiplier;
    public float afAttackSpeed;
    public float afCastSpeed;
}
