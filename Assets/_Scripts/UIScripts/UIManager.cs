using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BerkeAksoyCode
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;

        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("UI Manager is null.");
                }
                return instance;
            }
        }

        public Text coinText, invDesText, statusDesText;
        public Text levelText, vitaText, expText, strText, lifeText, dexText, manaText, energyText, phyDmgText, intelText, mgcDmgText, critMText, defText, critCText, DPSText, missCText, aPText;
        public GameObject coinRep, inventoryPanel, equipPanel, insideMenuPanel, map, backlight, statusPanel, statusCharm;
        private SwitchButton mapButton, armoryButton, weaponButton, accesoriesButton, potButton;
        public ItemSlot[] itemSlots;
        public equipSlot[] equipSlots;
        private Item slctdItem;
        private int slctdSlotID;
        private Player player;
        private GameManager gameManager;

        public int currentLine;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            gameManager = GameManager.Instance;

            getLevelUIElements();

            closeLevelMenu();
            closeInventoryPanel();
            closeMap();
            statusPanel.SetActive(false);
            statusCharm.SetActive(false);

            updateCoins();
        }

        public void getLevelUIElements()
        {
            inventoryPanel = GameObject.Find("Inventory Panel");
            insideMenuPanel = GameObject.Find("Level Inside Menu");
            equipPanel = GameObject.Find("Equip Panel");
            map = GameObject.Find("Map");
            coinRep = GameObject.Find("HUD/Coin Represent");
            backlight = GameObject.Find("Backlight");
            statusPanel = GameObject.Find("Status Panel");
            statusCharm = GameObject.Find("Status Charm");

            mapButton = GameObject.Find("HUD/Map Button").GetComponent<SwitchButton>();
            armoryButton = GameObject.Find("Inventory Panel/Armory_Button").GetComponent<SwitchButton>();
            weaponButton = GameObject.Find("Inventory Panel/Weaponry_Button").GetComponent<SwitchButton>();
            accesoriesButton = GameObject.Find("Inventory Panel/Accessory_Button").GetComponent<SwitchButton>();
            potButton = GameObject.Find("Inventory Panel/Alchemy_Button").GetComponent<SwitchButton>();

            levelText = GameObject.Find("Status Panel/Status Texts/Level Button").GetComponentInChildren<Text>();
            vitaText = GameObject.Find("Status Panel/Status Texts/Vitality Button").GetComponentInChildren<Text>();
            expText = GameObject.Find("Status Panel/Status Texts/Experience Button").GetComponentInChildren<Text>();
            strText = GameObject.Find("Status Panel/Status Texts/Strength Button").GetComponentInChildren<Text>();
            lifeText = GameObject.Find("Status Panel/Status Texts/Life Button").GetComponentInChildren<Text>();
            dexText = GameObject.Find("Status Panel/Status Texts/Dexterity Button").GetComponentInChildren<Text>();
            manaText = GameObject.Find("Status Panel/Status Texts/Mana Button").GetComponentInChildren<Text>();
            energyText = GameObject.Find("Status Panel/Status Texts/Energy Button").GetComponentInChildren<Text>();
            phyDmgText = GameObject.Find("Status Panel/Status Texts/Physical Damage Button").GetComponentInChildren<Text>();
            intelText = GameObject.Find("Status Panel/Status Texts/Intelligence Button").GetComponentInChildren<Text>();
            mgcDmgText = GameObject.Find("Status Panel/Status Texts/Magic Damage Button").GetComponentInChildren<Text>();
            critMText = GameObject.Find("Status Panel/Status Texts/Critical Multiplier Button").GetComponentInChildren<Text>();
            defText = GameObject.Find("Status Panel/Status Texts/Defence Button").GetComponentInChildren<Text>();
            critCText = GameObject.Find("Status Panel/Status Texts/Critical Chance Button").GetComponentInChildren<Text>();
            DPSText = GameObject.Find("Status Panel/Status Texts/DPS Button").GetComponentInChildren<Text>();
            missCText = GameObject.Find("Status Panel/Status Texts/Miss Chance Button").GetComponentInChildren<Text>();
            aPText = GameObject.Find("Status Charm/Available Points").GetComponent<Text>();
            statusDesText = GameObject.Find("Status Panel/Description Text").GetComponent<Text>();

            invDesText = inventoryPanel.GetComponentsInChildren<Text>()[0];
            itemSlots = inventoryPanel.GetComponentsInChildren<ItemSlot>();
            equipSlots = equipPanel.GetComponentsInChildren<equipSlot>();
            coinText = coinRep.GetComponentInChildren<Text>();
        }



        // Status

        public void openStatus()
        {
            statusPanel.SetActive(true);
            statusCharm.SetActive(true);
            player.stats.inSession = true;

            Time.timeScale = 0;
            //player.canMove = false;
            backlight.SetActive(true);

            player.stats.calculateStats();
            updateStatusTexts();

            selectSpecial("");
        }

        public void closeStatus()
        {
            player.stats.finalizeSpecials();

            statusPanel.SetActive(false);
            statusCharm.SetActive(false);
            player.stats.inSession = false;
            backlight.SetActive(false);

            Time.timeScale = 1;
            //player.canMove = true;
        }

        public void selectSpecial(string slctdSpecial)
        {
            player.stats.selectedAttribute = slctdSpecial;
            string specialInfo = "";

            SwitchButton Vitality = GameObject.Find("Status Charm/Vitality").GetComponent<SwitchButton>();
            SwitchButton Strength = GameObject.Find("Status Charm/Strength").GetComponent<SwitchButton>();
            SwitchButton Dexterity = GameObject.Find("Status Charm/Dexterity").GetComponent<SwitchButton>();
            SwitchButton Energy = GameObject.Find("Status Charm/Energy").GetComponent<SwitchButton>();
            SwitchButton Intelligence = GameObject.Find("Status Charm/Intelligence").GetComponent<SwitchButton>();

            if (player.stats.availablePoints > 0)
            {
                // Interactable
                switch (slctdSpecial)
                {
                    case "Vitality":
                        Vitality.selectedImage();
                        Strength.notSelectedImage();
                        Dexterity.notSelectedImage();
                        Energy.notSelectedImage();
                        Intelligence.notSelectedImage();
                        activateAddPointButton(true);
                        specialInfo = "Vitality is the heroes key point";
                        break;
                    case "Strength":
                        Vitality.notSelectedImage();
                        Strength.selectedImage();
                        Dexterity.notSelectedImage();
                        Energy.notSelectedImage();
                        Intelligence.notSelectedImage();
                        specialInfo = "Strength is in your arms!";
                        activateAddPointButton(true);
                        break;
                    case "Dexterity":
                        Vitality.notSelectedImage();
                        Strength.notSelectedImage();
                        Dexterity.selectedImage();
                        Energy.notSelectedImage();
                        Intelligence.notSelectedImage();
                        specialInfo = "Dexterity is in your hands!";
                        activateAddPointButton(true);
                        break;
                    case "Energy":
                        Vitality.notSelectedImage();
                        Strength.notSelectedImage();
                        Dexterity.notSelectedImage();
                        Energy.selectedImage();
                        Intelligence.notSelectedImage();
                        specialInfo = "Energy is the magic's soul";
                        activateAddPointButton(true);
                        break;
                    case "Intelligence":
                        Vitality.notSelectedImage();
                        Strength.notSelectedImage();
                        Dexterity.notSelectedImage();
                        Energy.notSelectedImage();
                        Intelligence.selectedImage();
                        specialInfo = "Without it you cannot defeat the Devil!";
                        activateAddPointButton(true);
                        break;
                    default:
                        Vitality.notSelectedImage();
                        Strength.notSelectedImage();
                        Dexterity.notSelectedImage();
                        Energy.notSelectedImage();
                        Intelligence.notSelectedImage();
                        specialInfo = "";
                        activateAddPointButton(false);
                        break;
                }
            }
            else
            {
                Vitality.notSelectedImage();
                Strength.notSelectedImage();
                Dexterity.notSelectedImage();
                Energy.notSelectedImage();
                Intelligence.notSelectedImage();
                specialInfo = "";
                activateAddPointButton(false);
            }

            statusDesText.text = specialInfo;
        }

        public void spendPoints()
        {
            player.stats.sP();
            updateStatusTexts();
        }

        public void undo()
        {
            player.stats.uP();
            updateStatusTexts();
        }

        public void activateUndoButton(bool activation)
        {
            GameObject.Find("Status Panel/Undo_Button").GetComponent<Button>().interactable = activation;
        }
        public void activateAddPointButton(bool activation)
        {
            GameObject.Find("Status Panel/Add Point Button").GetComponent<Button>().interactable = activation;
        }


        public void updateStatusTexts()
        {
            CharStats stats = player.stats;
            int tcritChance = 0, tmC = 0, tphyDmg = 0, tdefence = 0, thp = 0, tmana = 0, tmgcDmg = 0;
            float tcritMultiplier = 0;
            if (stats.cDex > stats.tempCDex)
            {
                dexText.color = Color.green;
                tcritChance = Mathf.FloorToInt((float)stats.cDex / 10f);
                tmC = -Mathf.FloorToInt((float)stats.cDex / 10f);
            }
            else
            {
                dexText.color = Color.white;
            }

            if (stats.cStr > stats.tempCStr)
            {
                strText.color = Color.green;
                tphyDmg = Mathf.FloorToInt((float)stats.cStr / 4f);
                tdefence = Mathf.FloorToInt((float)stats.cStr / 4f);
                tcritMultiplier = (float)stats.cStr / 30f;
            }
            else
            {
                strText.color = Color.white;
            }

            if (stats.cVita > stats.tempCVita)
            {
                vitaText.color = Color.green;
                thp = Mathf.FloorToInt((float)stats.cVita / 4f);
            }
            else
            {
                vitaText.color = Color.white;
            }

            if (stats.cEnergy > stats.tempCEnergy)
            {
                energyText.color = Color.green;
                tmana = Mathf.FloorToInt((float)stats.cEnergy / 4f);
            }
            else
            {
                energyText.color = Color.white;
            }

            if (stats.cIntel > stats.tempCIntel)
            {
                intelText.color = Color.green;
                tmgcDmg = Mathf.FloorToInt((float)stats.cIntel / 4f);
            }
            else
            {
                intelText.color = Color.white;
            }

            int tminDPS = Mathf.FloorToInt((float)stats.cPhyDmg * 0.8f / stats.cAttackSpeed);
            if (tminDPS < 0)
            {
                tminDPS = 0;
            }
            int tmaxDPS = Mathf.CeilToInt((float)stats.cPhyDmg * 1.2f / stats.cAttackSpeed * stats.cCritMultiplier);
            if (tmaxDPS < 0)
            {
                tmaxDPS = 0;
            }

            if (tcritChance > 0)
            {
                critCText.color = Color.green;
            }
            else
            {
                critCText.color = Color.white;
            }

            if (tmC < 0)
            {
                missCText.color = Color.green;
            }
            else
            {
                missCText.color = Color.white;
            }

            if (tphyDmg > 0)
            {
                phyDmgText.color = Color.green;
            }
            else
            {
                phyDmgText.color = Color.white;
            }

            if (tdefence > 0)
            {
                defText.color = Color.green;
            }
            else
            {
                defText.color = Color.white;
            }

            if (tcritMultiplier > 0)
            {
                critCText.color = Color.green;
            }
            else
            {
                critCText.color = Color.white;
            }

            if (thp > 0)
            {
                lifeText.color = Color.green;
            }
            else
            {
                lifeText.color = Color.white;
            }

            if (tmana > 0)
            {
                manaText.color = Color.green;
            }
            else
            {
                manaText.color = Color.white;
            }

            if (tmgcDmg > 0)
            {
                mgcDmgText.color = Color.green;
            }
            else
            {
                mgcDmgText.color = Color.white;
            }

            if (tminDPS > stats.minDPS || tmaxDPS > stats.maxDPS)
            {
                DPSText.color = Color.green;
            }
            else
            {
                DPSText.color = Color.white;
            }

            // artıs temsil edilmektedir
            // Ekleme yapma bulunanlarda artan varsa text e ekle ve sun

            levelText.text = " Level " + stats.currentLevel.ToString();
            vitaText.text = " Vita " + stats.cVita.ToString();
            expText.text = " Exp " + stats.currentXP.ToString() + " / " + stats.XPToNextLevel[stats.currentLevel - 1].ToString();
            strText.text = " Str " + stats.cStr.ToString();
            lifeText.text = " Life " + stats.currentHP.ToString() + " / " + (stats.maxHP + thp).ToString();
            dexText.text = " Dex " + (stats.cDex + tdefence).ToString();
            manaText.text = " Mana " + stats.currentMana.ToString() + " / " + (stats.maxMana + tmana).ToString();
            energyText.text = " Energy " + stats.cEnergy.ToString();
            phyDmgText.text = " Phy Dmg " + Mathf.FloorToInt((stats.cPhyDmg + tphyDmg) * 0.8f).ToString() + " - " + Mathf.CeilToInt((stats.cPhyDmg + tphyDmg) * 1.2f * (stats.cCritMultiplier + tcritMultiplier));
            intelText.text = " Intel " + stats.cIntel.ToString();
            mgcDmgText.text = " Mgc Dmg " + (stats.cMgcDmg + tmgcDmg).ToString();
            critMText.text = " Crit Mul " + (stats.cCritMultiplier + tcritMultiplier).ToString();
            defText.text = " Def " + stats.cDef.ToString();
            critCText.text = " Crit Chance " + (stats.cCritChance + tcritChance).ToString() + "%";
            DPSText.text = " DPS " + stats.minDPS.ToString() + " - " + stats.maxDPS.ToString();
            missCText.text = " Miss Chance " + (stats.cMissChance + tmC).ToString();
            aPText.text = stats.availablePoints.ToString();
        }

        public void showStatusInfo(string statusName)
        {
            string infoText = "";
            selectSpecial("");

            switch (statusName)
            {
                case "Level":
                    infoText = "Shows current level";
                    break;
                case "Experience":
                    infoText = "";
                    break;
                case "Life":
                    infoText = "";
                    break;
                case "Mana":
                    infoText = "";
                    break;
                case "Vitality":
                    infoText = "";
                    break;
                case "Strength":
                    infoText = "";
                    break;
                case "Dexterity":
                    infoText = "";
                    break;
                case "Energy":
                    infoText = "";
                    break;
                case "Intelligence":
                    infoText = "";
                    break;
                case "Physical Damage":
                    infoText = "";
                    break;
                case "Magical Damage":
                    infoText = "";
                    break;
                case "Defence":
                    infoText = "";
                    break;
                case "Critical Multiplier":
                    infoText = "";
                    break;
                case "Critical Chance":
                    infoText = "";
                    break;
                case "Damage Per Second":
                    infoText = "";
                    break;
                case "Miss Chance":
                    infoText = "";
                    break;
                default:
                    infoText = "";
                    break;
            }

            statusDesText.text = infoText;
        }


        // Map
        public void openMap()
        {
            if (!map.activeInHierarchy)
            {
                mapButton.selectedImage();
                GameObject.Find("Camera").GetComponent<Camera>().orthographicSize = 12;
                map.SetActive(true);
            }
            else
            {
                mapButton.notSelectedImage();
                GameObject.Find("Camera").GetComponent<Camera>().orthographicSize = 6;
                map.SetActive(false);
            }
        }

        public void closeMap()
        {
            map.SetActive(false);
        }








        // Level menu
        public void openLevelMenu()
        {
            if (!insideMenuPanel.activeInHierarchy)
            {
                insideMenuPanel.SetActive(true);
                Time.timeScale = 0;
                //player.canMove = false;
                backlight.SetActive(true);
            }
        }

        public void resume()
        {
            if (insideMenuPanel.activeInHierarchy)
            {
                closeLevelMenu();
                Time.timeScale = 1;
                //player.canMove = true;
                backlight.SetActive(false);
            }
        }

        public void closeLevelMenu()
        {
            insideMenuPanel.SetActive(false);
        }












        // Inventory
        public void openInventoryPanel()
        {
            Time.timeScale = 0;
            backlight.SetActive(true);
            inventoryPanel.SetActive(true);
            openEquipPanel();
            loadItems("Armory");
            showSelectedItem(null, -1);
            //inventoryPanel.GetComponent<Animator>().SetTrigger("Open");
        }

        public void closeInventoryPanel()
        {
            inventoryPanel.SetActive(false);
            closeEquipPanel();
            backlight.SetActive(false);
            Time.timeScale = 1;
            //player.canMove = true;
        }

        public void loadItems(string classOfItems)
        {
            switch (classOfItems)
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
                case "Accessory":
                    armoryButton.notSelectedImage();
                    weaponButton.notSelectedImage();
                    accesoriesButton.selectedImage();
                    potButton.notSelectedImage();
                    break;
                case "Alchemy":
                    armoryButton.notSelectedImage();
                    weaponButton.notSelectedImage();
                    accesoriesButton.notSelectedImage();
                    potButton.selectedImage();
                    break;
            }

            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].id = i;
            }

            int count = 0;

            for (int k = 0; k < gameManager.itemHeld.Length; k++)
            {
                if (gameManager.itemHeld[k] != null)
                {
                    gameManager.itemHeld[k].itemData.ItemHeldIn = k;

                    if (gameManager.itemHeld[k].itemData.itemClass == classOfItems)
                    {
                        updateSlot(itemSlots[count], gameManager.itemHeld[k]);
                        count++;
                    }
                }
            }

            for (int r = count; r < itemSlots.Length; r++)
            {
                updateSlot(itemSlots[r], null);
            }
        }

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

        public bool addItemToInventory(Item itemIn)
        {
            loadItems(itemIn.itemData.itemClass);

            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (!itemSlots[i].hasItem)
                {
                    for (int k = 0; k < gameManager.referenceItems.Length; k++)
                    {
                        if (gameManager.referenceItems[k].itemData.id == itemIn.itemData.id)
                        {
                            for (int r = 0; r < gameManager.itemHeld.Length; r++)
                            {
                                if (gameManager.itemHeld[r] == null)
                                {
                                    gameManager.itemHeld[r] = gameManager.referenceItems[k];
                                    break;
                                }
                            }
                            loadItems(itemIn.itemData.itemClass);
                            return true;
                        }
                    }

                    Debug.Log("Item reference missing!");
                }
            }

            Debug.Log("Inventory is full!");

            return false;
        }

        public void discardItem()
        {
            if (slctdItem != null)
            {
                gameManager.itemHeld[slctdItem.itemData.ItemHeldIn] = null;
                loadItems(slctdItem.itemData.itemClass);
                showSelectedItem(null, -1);
            }
        }

        public void showSelectedItem(Item item, int slotID)
        {
            if (item != null)
            {
                slctdItem = item;
                slctdSlotID = slotID;
                invDesText.text = item.itemData.itemName + ": \n" + item.itemData.description;
            }
            else
            {
                slctdItem = null;
                slctdSlotID = -1;
                invDesText.text = "Select Item";
            }
        }



        // Equip System

        public void openEquipPanel()
        {
            equipPanel.SetActive(true);
            loadEquippedItems();
        }

        public void closeEquipPanel()
        {
            equipPanel.SetActive(false);
        }

        public void loadEquippedItems()
        {
            for (int i = 0; i < equipSlots.Length; i++)
            {
                equipSlots[i].id = i;

                if (gameManager.itemEquipped[i] != null)
                {
                    updateEquipSlot(equipSlots[i], gameManager.itemEquipped[i]);
                }
                else
                {
                    updateEquipSlot(equipSlots[i], null);
                }
            }
        }

        public void updateEquipSlot(equipSlot slotIn, Item itemIn)
        {
            if (itemIn != null)
            {
                slotIn.item = itemIn;
                slotIn.itemImage.sprite = itemIn.itemData.itemSprite;
                Color tempColor = slotIn.itemImage.color;
                tempColor.a = 1f;
                slotIn.itemImage.color = tempColor;
                slotIn.hasItem = true;
            }
            else
            {
                slotIn.item = null;
                slotIn.itemImage.sprite = null;
                Color tempColor = slotIn.itemImage.color;
                tempColor.a = 0f;
                slotIn.itemImage.color = tempColor;
                slotIn.hasItem = false;
            }
        }

        public void equipItem()
        {
            if (slctdItem != null)
            {
                switch (slctdItem.itemData.itemType)
                {
                    case "Helmet":
                        if (!equipSlots[0].hasItem)
                        {
                            equip(0, slctdItem);
                        }
                        else
                        {
                            Item itemToEquip = slctdItem;
                            Item itemToKeep = equipSlots[0].item;
                            int keepSlotID = slctdSlotID;
                            equip(0, itemToEquip);
                            dropToInventory(itemToKeep, keepSlotID);
                        }
                        break;
                    case "Amulet":
                        if (!equipSlots[1].hasItem)
                        {
                            equip(1, slctdItem);
                        }
                        else
                        {
                            Item itemToEquip = slctdItem;
                            Item itemToKeep = equipSlots[1].item;
                            int keepSlotID = slctdSlotID;
                            equip(1, itemToEquip);
                            dropToInventory(itemToKeep, keepSlotID);
                        }
                        break;
                    case "Weapon":
                        if (!equipSlots[2].hasItem)
                        {
                            equip(2, slctdItem);
                        }
                        else
                        {
                            Item itemToEquip = slctdItem;
                            Item itemToKeep = equipSlots[2].item;
                            int keepSlotID = slctdSlotID;
                            equip(2, itemToEquip);
                            dropToInventory(itemToKeep, keepSlotID);
                        }
                        break;
                    case "Armor":
                        if (!equipSlots[3].hasItem)
                        {
                            equip(3, slctdItem);
                        }
                        else
                        {
                            Item itemToEquip = slctdItem;
                            Item itemToKeep = equipSlots[3].item;
                            int keepSlotID = slctdSlotID;
                            equip(3, itemToEquip);
                            dropToInventory(itemToKeep, keepSlotID);
                        }
                        break;
                    case "Book":
                        if (!equipSlots[4].hasItem)
                        {
                            equip(4, slctdItem);
                        }
                        else if (!equipSlots[5].hasItem)
                        {
                            equip(5, slctdItem);
                        }
                        else if (!equipSlots[6].hasItem)
                        {
                            equip(6, slctdItem);
                        }
                        else
                        {
                            Debug.Log("You should remove and reinsert your skils");
                        }
                        break;
                    case "Leg Armor":
                        if (!equipSlots[7].hasItem)
                        {
                            equip(7, slctdItem);
                        }
                        else
                        {
                            Item itemToEquip = slctdItem;
                            Item itemToKeep = equipSlots[7].item;
                            int keepSlotID = slctdSlotID;
                            equip(7, itemToEquip);
                            dropToInventory(itemToKeep, keepSlotID);
                        }
                        break;
                    case "Gloves":
                        if (!equipSlots[8].hasItem)
                        {
                            equip(8, slctdItem);
                        }
                        else
                        {
                            Item itemToEquip = slctdItem;
                            Item itemToKeep = equipSlots[8].item;
                            int keepSlotID = slctdSlotID;
                            equip(8, itemToEquip);
                            dropToInventory(itemToKeep, keepSlotID);
                        }
                        break;
                    case "Ring":
                        if (!equipSlots[9].hasItem)
                        {
                            equip(9, slctdItem);
                        }
                        else
                        {
                            Item itemToEquip = slctdItem;
                            Item itemToKeep = equipSlots[9].item;
                            int keepSlotID = slctdSlotID;
                            equip(9, itemToEquip);
                            dropToInventory(itemToKeep, keepSlotID);
                        }
                        break;
                    case "Boots":
                        if (!equipSlots[10].hasItem)
                        {
                            equip(10, slctdItem);
                        }
                        else
                        {
                            Item itemToEquip = slctdItem;
                            Item itemToKeep = equipSlots[10].item;
                            int keepSlotID = slctdSlotID;
                            equip(10, itemToEquip);
                            dropToInventory(itemToKeep, keepSlotID);
                        }
                        break;
                }
            }

        }

        public void equip(int caseID, Item itemIn)
        {
            for (int k = 0; k < gameManager.referenceItems.Length; k++)
            {
                if (gameManager.referenceItems[k].itemData.id == itemIn.itemData.id)
                {
                    gameManager.itemEquipped[caseID] = gameManager.referenceItems[k];
                    discardItem();
                    loadEquippedItems();
                    player.stats.calculateStats();
                    return;
                }
            }
        }

        public void dropToInventory(Item itemIn, int SlotID)
        {
            for (int i = 0; i < gameManager.itemHeld.Length; i++)
            {
                if (gameManager.itemHeld[i] == null)
                {
                    gameManager.itemHeld[i] = itemIn;
                    break;
                }
            }
            loadItems(itemIn.itemData.itemClass);
        }

        public void deEquip(equipSlot slotIn)
        {
            bool successful = false;

            loadItems(slotIn.item.itemData.itemClass);

            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (!itemSlots[i].hasItem)
                {
                    for (int k = 0; k < gameManager.referenceItems.Length; k++)
                    {
                        if (gameManager.referenceItems[k].itemData.id == slotIn.item.itemData.id)
                        {
                            for (int r = 0; r < gameManager.itemHeld.Length; r++)
                            {
                                if (gameManager.itemHeld[r] == null)
                                {
                                    gameManager.itemHeld[r] = gameManager.referenceItems[k];
                                    successful = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            if (successful)
            {
                MagicBook book = null;
                if (slotIn.item.itemData.itemType.Equals("Book"))
                {
                    book = (MagicBook)slotIn.item;
                }

                gameManager.itemEquipped[slotIn.id] = null;
                loadItems(slotIn.item.itemData.itemClass);
                loadEquippedItems();
                player.stats.calculateStats();

                if (book != null)
                {
                    player.changeSpellTo(player.stats.chosenSpellSlot);
                }
            }
            else
            {
                Debug.Log("Inventory is full item not deequipped!");
            }

            showSelectedItem(null, -1);
        }




        // Coin

        public void updateCoins()
        {
            coinRep.GetComponent<Animator>().SetTrigger("CoinPicked");

            coinText.text = gameManager.heldCoins.ToString();
        }
    }
}