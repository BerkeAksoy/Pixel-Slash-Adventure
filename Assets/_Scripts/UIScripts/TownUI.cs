using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BerkeAksoyCode
{
    public class TownUI : MonoBehaviour
    {

        private SwitchButton goldSellButton, gemSellButton, soldButton, armoryButton, weaponButton, accesoriesButton, potButton;
        private ItemSlot[] shopItemSlots, invItemSlots;
        private Item[] goldSItems, gemSItems, soldItems;
        private Text shopNameText, priceText, coinText, invDescText, shopDescText, heldCoinsText, heldGemsText;
        public GameObject inventoryPanel, shopPanel, insideMenuPanel, backlight;
        public Sprite[] currencySprites;
        private Item slctdShopItem, slctdInvItem;
        private int slctdShopSlotID, slctdInvSlotID;
        private GameManager gameManagerInstance;

        private void Start()
        {
            getUIElements();
            closeShop();
        }

        public void getUIElements()
        {
            gameManagerInstance = GameManager.Instance;

            inventoryPanel = GameObject.Find("Inventory Panel");
            shopPanel = GameObject.Find("Shop Panel");
            backlight = GameObject.Find("Backlight");

            goldSellButton = GameObject.Find("Shop Panel/Gold Items Button").GetComponent<SwitchButton>();
            gemSellButton = GameObject.Find("Shop Panel/Gem Items Button").GetComponent<SwitchButton>();
            soldButton = GameObject.Find("Shop Panel/Sold Items Button").GetComponent<SwitchButton>();

            armoryButton = GameObject.Find("Inventory Panel/Armory_Button").GetComponent<SwitchButton>();
            weaponButton = GameObject.Find("Inventory Panel/Weapon_Button").GetComponent<SwitchButton>();
            accesoriesButton = GameObject.Find("Inventory Panel/Accesories_Button").GetComponent<SwitchButton>();
            potButton = GameObject.Find("Inventory Panel/Pot_Button").GetComponent<SwitchButton>();

            invDescText = GameObject.Find("Inventory Panel/Description Text").GetComponent<Text>();
            shopDescText = GameObject.Find("Shop Panel/Description Text").GetComponent<Text>();
            shopNameText = GameObject.Find("Shop Panel/Shop Text").GetComponent<Text>();
            coinText = GameObject.Find("Inventory Panel/Coin Box/Coin Text").GetComponent<Text>();
            priceText = GameObject.Find("Shop Panel/Price Text").GetComponent<Text>();

            heldCoinsText = GameObject.Find("Shop Panel/Current Gold/Hold Coins").GetComponent<Text>();
            heldGemsText = GameObject.Find("Shop Panel/Current Gem/Hold Gems").GetComponent<Text>();

            invItemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();
            shopItemSlots = shopPanel.GetComponentsInChildren<ItemSlot>();

            goldSItems = new Item[30];
            gemSItems = new Item[30];
            soldItems = new Item[30];
        }

        private void openShop()
        {
            updateWealth();
            loadInvItems("Armory");

            shopPanel.SetActive(true);
            inventoryPanel.SetActive(true);
            backlight.SetActive(true);
        }

        public void closeShop()
        {
            shopPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            backlight.SetActive(false);
        }



        // Shop Panel

        public void loadSellerArrays(string seller)
        {
            if (!seller.Equals("Pet Store"))
            {
                openShop();
                shopNameText.text = seller;

                int actTag = gameManagerInstance.actTag;
                Debug.Log("In act " + actTag);

                List<Item> act1goldSItems = new List<Item>(), act1gemSItems = new List<Item>(), act2goldSItems = new List<Item>(), act2gemSItems = new List<Item>(),
                    act3goldSItems = new List<Item>(), act3gemSItems = new List<Item>();

                for (int i = 0; i < gameManagerInstance.referenceItems.Length; i++)
                {
                    Item itemX = gameManagerInstance.referenceItems[i];
                    string itemPriceType = itemX.itemData.itemPriceType;
                    int itemAct = itemX.itemData.itemAct;

                    if (itemAct == 1 && itemPriceType.Equals("gold"))
                    {
                        act1goldSItems.Add(itemX);
                    }
                    else if (itemAct == 1 && itemPriceType.Equals("gem"))
                    {
                        act1gemSItems.Add(itemX);
                    }
                    else if (itemAct == 2 && itemPriceType.Equals("gold"))
                    {
                        act2goldSItems.Add(itemX);
                    }
                    else if (itemAct == 2 && itemPriceType.Equals("gem"))
                    {
                        act2gemSItems.Add(itemX);
                    }
                    else if (itemAct == 3 && itemPriceType.Equals("gold"))
                    {
                        act3goldSItems.Add(itemX);
                    }
                    else if (itemAct == 3 && itemPriceType.Equals("gem"))
                    {
                        act3gemSItems.Add(itemX);
                    }
                }

                if (actTag == 1)
                {
                    slotter(seller, act1goldSItems, act1gemSItems);
                }
                else if (actTag == 2)
                {

                    slotter(seller, act2goldSItems, act2gemSItems);
                }
                else
                {
                    slotter(seller, act3goldSItems, act3gemSItems);
                }

                loadShopItems("goldSell");
            }
            else
            {


                //loadPets("goldSell");
            }
        }

        private void refreshSlots()
        {
            for (int i = 0; i < goldSItems.Length; i++)
            {
                goldSItems[i] = null;
                gemSItems[i] = null;
            }
        }

        private void slotter(string seller, List<Item> goldNotSlotItems, List<Item> gemNotSlotItems)
        {
            refreshSlots();

            int count = 0;
            foreach (Item itemX in goldNotSlotItems)
            {
                if (itemX.itemData.itemSeller.Equals(seller))
                {
                    goldSItems[count] = itemX;
                    count++;
                }
            }

            count = 0;
            foreach (Item itemX in gemNotSlotItems)
            {
                if (itemX.itemData.itemSeller.Equals(seller))
                {
                    gemSItems[count] = itemX;
                    count++;
                }
            }
        }

        public void loadShopItems(string sellType)
        {
            for (int i = 0; i < shopItemSlots.Length; i++)
            {
                shopItemSlots[i].id = i;
                updateSlot(shopItemSlots[i], null);
            }

            showSlctdShopItem(null, -1);

            switch (sellType)
            {
                case "goldSell":
                    goldSellButton.selectedImage();
                    gemSellButton.notSelectedImage();
                    soldButton.notSelectedImage();

                    for (int k = 0; k < goldSItems.Length; k++)
                    {
                        if (goldSItems[k] != null)
                        {
                            updateSlot(shopItemSlots[k], goldSItems[k]);
                        }
                    }

                    break;
                case "gemSell":
                    goldSellButton.notSelectedImage();
                    gemSellButton.selectedImage();
                    soldButton.notSelectedImage();

                    for (int k = 0; k < gemSItems.Length; k++)
                    {
                        if (gemSItems[k] != null)
                        {
                            updateSlot(shopItemSlots[k], gemSItems[k]);
                        }
                    }

                    break;
                case "solds":
                    goldSellButton.notSelectedImage();
                    gemSellButton.notSelectedImage();
                    soldButton.selectedImage();

                    for (int k = 0; k < soldItems.Length; k++)
                    {
                        if (soldItems[k] != null)
                        {
                            updateSlot(shopItemSlots[k], soldItems[k]);
                        }
                    }

                    break;
            }
        }

        public void showSlctdShopItem(Item item, int slotID)
        {
            if (item != null)
            {
                Image currencyImage = GameObject.Find("Shop Panel/Currency").GetComponent<Image>();

                slctdShopItem = item;
                slctdShopSlotID = slotID;
                shopDescText.text = item.itemData.itemName + ": \n" + item.itemData.description;

                if (item.itemData.itemPriceType.Equals("gold"))
                {
                    currencyImage.sprite = currencySprites[0];
                }
                else
                {
                    currencyImage.sprite = currencySprites[1];
                }

                priceText.text = item.itemData.priceToBuy.ToString();
            }
            else
            {
                slctdShopItem = null;
                slctdShopSlotID = -1;
                shopDescText.text = "Select Item";
                priceText.text = "0";
            }
        }























        // Inventory

        public void loadInvItems(string classOfitem)
        {
            switch (classOfitem)
            {
                case "Armory":
                    armoryButton.selectedImage();
                    weaponButton.notSelectedImage();
                    accesoriesButton.notSelectedImage();
                    potButton.notSelectedImage();
                    break;
                case "Weaponry":
                    armoryButton.notSelectedImage();
                    weaponButton.selectedImage();
                    accesoriesButton.notSelectedImage();
                    potButton.notSelectedImage();
                    break;
                case "Accesories":
                    armoryButton.notSelectedImage();
                    weaponButton.notSelectedImage();
                    accesoriesButton.selectedImage();
                    potButton.notSelectedImage();
                    break;
                case "Pot":
                    armoryButton.notSelectedImage();
                    weaponButton.notSelectedImage();
                    accesoriesButton.notSelectedImage();
                    potButton.selectedImage();
                    break;
            }

            for (int i = 0; i < invItemSlots.Length; i++)
            {
                invItemSlots[i].id = i;
            }

            int count = 0;

            for (int k = 0; k < gameManagerInstance.itemHeld.Length; k++)
            {
                if (gameManagerInstance.itemHeld[k] != null)
                {
                    gameManagerInstance.itemHeld[k].itemData.ItemHeldIn = k;

                    if (gameManagerInstance.itemHeld[k].itemData.itemClass == classOfitem)
                    {
                        updateSlot(invItemSlots[count], gameManagerInstance.itemHeld[k]);
                        count++;
                    }
                }
            }

            for (int r = count; r < invItemSlots.Length; r++)
            {
                updateSlot(invItemSlots[r], null);
            }
        }

        private bool canBuy(Item itemIn)
        {
            int price = itemIn.itemData.priceToBuy;
            int curGems = gameManagerInstance.heldGems;
            int curCoins = gameManagerInstance.heldCoins;

            if (itemIn.itemData.itemPriceType.Equals("gold") && price <= curCoins)
            {
                gameManagerInstance.heldCoins = curCoins - price;
                return true;
            }
            else if (itemIn.itemData.itemPriceType.Equals("gem") && price <= curGems)
            {
                gameManagerInstance.heldGems = curGems - price;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Buy_Add_ItemToInventory(Item itemIn)
        {
            loadInvItems(itemIn.itemData.itemClass);

            for (int i = 0; i < invItemSlots.Length; i++)
            {
                if (!invItemSlots[i].hasItem)
                {
                    if (canBuy(itemIn))
                    {
                        for (int k = 0; k < gameManagerInstance.referenceItems.Length; k++)
                        {
                            if (gameManagerInstance.referenceItems[k].itemData.id == itemIn.itemData.id)
                            {
                                for (int r = 0; r < gameManagerInstance.itemHeld.Length; r++)
                                {
                                    if (gameManagerInstance.itemHeld[r] == null)
                                    {
                                        gameManagerInstance.itemHeld[r] = gameManagerInstance.referenceItems[k];
                                        break;
                                    }
                                }
                                loadInvItems(itemIn.itemData.itemClass);
                                updateWealth();
                                Debug.Log("You bought a new item!");
                                return;
                            }
                        }

                        Debug.Log("Item reference missing!");
                    }
                    else
                    {
                        Debug.Log("Not enough resources!");
                    }
                }
            }

            Debug.Log("Inventory is full!");
        }

        public void showSlctdInvItem(Item item, int slotID)
        {
            Image currencyImage = GameObject.Find("Shop Panel/Currency").GetComponent<Image>();

            if (item != null)
            {
                slctdInvItem = item;
                slctdInvSlotID = slotID;
                invDescText.text = item.itemData.itemName + ": \n" + item.itemData.description;

                if (item.itemData.itemPriceType.Equals("gold"))
                {
                    currencyImage.sprite = currencySprites[0];
                }
                else
                {
                    currencyImage.sprite = currencySprites[1];
                }

                coinText.text = item.itemData.priceToSell.ToString();
            }
            else
            {
                slctdInvItem = null;
                slctdInvSlotID = -1;
                invDescText.text = "Select Item";
            }
        }






        // Common

        public void updateSlot(ItemSlot slotIn, Item itemIn)
        {
            if (itemIn != null)
            {
                slotIn.item = itemIn;
                slotIn.itemImage.sprite = itemIn.itemData.itemSprite;
                slotIn.rarityText.text = itemIn.itemData.rarity;
                slotIn.hasItem = true;
                slotIn.itemImage.enabled = true;
            }
            else
            {
                slotIn.item = null;
                slotIn.itemImage.sprite = null;
                slotIn.hasItem = false;
                slotIn.itemImage.enabled = false;
                slotIn.rarityText.text = "";
            }
        }

        private void updateWealth()
        {
            heldCoinsText.text = gameManagerInstance.heldCoins.ToString();
            heldGemsText.text = gameManagerInstance.heldGems.ToString();
        }


    }
}