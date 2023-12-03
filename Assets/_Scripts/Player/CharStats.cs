using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BerkeAksoyCode
{
    public class CharStats : MonoBehaviour
    {

        // Default Values

        private const int mageBookSlotCount = 3, startLevel = 1, maxLevel = 99;
        private const float _XPMultiplier = 1.06f, _CritMultiplier = 1.5f, _dmgMagnitude = 0.4f, _mgcMagnitude = 0.4f, _attackSpeed = 1.2f, _castSpeed = 0.4f;
        private const int _Vita = 10, _Str = 10, _Dex = 10, _Energy = 10, _Intel = 10, _HP = 100, _Mana = 100, _Breath = 10, _phyDmg = 4, _mgcDmg = 4,
            _critChance = 3, _missChance = 10, _manaRegen = 1, _HPRegen = 1;

        public int[] XPToNextLevel = new int[maxLevel - 1];
        public string characterName, selectedAttribute;

        // Current Values
        public int currentHP, currentMana, maxHP, maxMana, currentBreath, maxBreath, currentLevel = startLevel, totalGainedXP, currentXP, availablePoints, minDPS = 0, maxDPS = 0;
        private int sVita = 0, sStr = 0, sDex = 0, sEnergy = 0, sIntel = 0;
        public int cVita, cStr, cDex, cEnergy, cIntel, cPhyDmg, cMgcDmg, cDef, cCritChance, cMissChance, cManaCostReduce, cManaRegen, cHPRegen;
        public int chosenSpellSlot = 0;
        public float cAttackSpeed, cCastSpeed, cXPMultiplier, cCritMultiplier, cDmgMagnitude, cMgcMagnitude;

        // Temp Values
        private int tempVita, tempStr, tempDex, tempEnergy, tempIntel, tempPoints;
        public int tempCVita, tempCStr, tempCDex, tempCEnergy, tempCIntel;
        public bool inSession, canUndo;

        // Timer
        private float breathTimer = 0;
        public bool fullBreath = true;

        public Item eqpHelmet, eqpArmor, eqpAmulet, eqpLegArmor, eqpGloves, eqpRing, eqpBoots;
        public Weapon eqpWpn;
        public MagicBook[] eqpMagicBooks = new MagicBook[mageBookSlotCount];

        public GameObject levelUp;
        //private HealthBar myHB;
        //private ManaBar myMB;
        //private LevelBar myLB;
        //private BreathCounter myBC;


        void Start()
        {
            //myHB = GameObject.Find("Main Canvas/HUD/Health Bar").GetComponent<HealthBar>();
            //myMB = GameObject.Find("Main Canvas/HUD/Mana Bar").GetComponent<ManaBar>();
            //myLB = GameObject.Find("Main Canvas/HUD/Level Bar").GetComponent<LevelBar>();
            //myBC = GetComponentInChildren<BreathCounter>();

            currentHP = _HP;
            currentMana = _Mana;
            currentBreath = _Breath;

            //alttaki silincek
            maxBreath = _Breath;

            XPToNextLevel[0] = 100;

            for (int i = 1; i < maxLevel - 1; i++)
            {
                XPToNextLevel[i] = Mathf.FloorToInt(XPToNextLevel[i - 1] * 1.10f);
            }

            StartCoroutine(regen());

            calculateStats();
            //updateXP();
            //updateHealth();
            //updateMana();

            //myBC.deActivateIndicator();
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                addXP(5);
            }

            if (currentMana > 100)
            {
                //Debug.LogError("asdsad");
            }
        }

        /*public void flipBreathSprite()
        {
            if (myBC.isActiveAndEnabled)
            {
                myBC.transform.rotation = myBC.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            }
        }

        public void updateBreathUI()
        {
            if (!myBC.isActiveAndEnabled)
            {
                myBC.activateIndicator();
            }

            myBC.updateBreathIndicator(maxBreath, currentBreath);
        }*/

        /*public void refillBreath()
        {
            breathTimer += Time.deltaTime / 0.5f; // Divided by 0.5 to make it 0.5 seconds.

            if (breathTimer > 0.5f)
            {
                breathTimer = 0;
                currentBreath++;
                Debug.Log("my breath: " + currentBreath);
                updateBreathUI();
            }

            if (currentBreath >= maxBreath)
            {
                fullBreath = true;
                myBC.deActivateIndicator();
            }
        }*/

        public void addXP(int XP)
        {
            XP = Mathf.CeilToInt(XP * cXPMultiplier);
            totalGainedXP += XP;
            currentXP += XP;

            if (currentXP >= XPToNextLevel[currentLevel - 1] && currentLevel + 1 != maxLevel)
            {
                currentXP -= XPToNextLevel[currentLevel - 1];

                currentLevel++;
                Instantiate(levelUp, transform.position, Quaternion.identity); // Create LevelUp gameObject for animating
                                                                               //player.myLevelUp = GameObject.Find("/" + name + "/LevelUp");

                availablePoints += 3;
                currentHP = maxHP;
                currentMana = maxMana;

                //updateHealth();
                //updateMana();
            }

            //updateXP();
        }

        public void setEquippedItems()
        {
            GameManager gameManager = GameManager.Instance;

            eqpHelmet = gameManager.itemEquipped[0];
            eqpAmulet = gameManager.itemEquipped[1];
            eqpWpn = (Weapon)gameManager.itemEquipped[2];
            eqpArmor = gameManager.itemEquipped[3];
            for (int i = 0; i < mageBookSlotCount; i++)
            {
                eqpMagicBooks[i] = (MagicBook)GameManager.Instance.itemEquipped[i + 4];
            }
            eqpLegArmor = gameManager.itemEquipped[7];
            eqpGloves = gameManager.itemEquipped[8];
            eqpRing = gameManager.itemEquipped[9];
            eqpBoots = gameManager.itemEquipped[10];
        }

        public void calculateStats()
        {
            setEquippedItems();

            int str = 0, dex = 0, vita = 0, energy = 0, intelligence = 0, hpBuff = 0, manaBuff = 0, phyDmg = 0, mgcDmg = 0, def = 0, critChance = 0, missChance = 0, mCR = 0;
            float aS = 0f, cS = 0f, cM = 0f, eM = 0f;
            bool spellDmgAdded = false;

            if (eqpHelmet != null)
            {
                vita += eqpHelmet.itemData.afVita;
                str += eqpHelmet.itemData.afStr;
                dex += eqpHelmet.itemData.afDex;
                energy += eqpHelmet.itemData.afEnergy;
                intelligence += eqpHelmet.itemData.afIntelligence;

                phyDmg += eqpHelmet.itemData.afPhysicalDmg;
                mgcDmg += eqpHelmet.itemData.afMagicalDmg;
                def += eqpHelmet.itemData.afDefence;
                critChance += eqpHelmet.itemData.afCritChance;
                missChance += eqpHelmet.itemData.afMissChance;
                mCR += eqpHelmet.itemData.afManaCostReduce;

                aS += eqpHelmet.itemData.afAttackSpeed;
                cS += eqpHelmet.itemData.afCastSpeed;

                cM += eqpHelmet.itemData.afCritMultiplier;
                eM += eqpHelmet.itemData.afXPMultiplier;

                hpBuff += eqpHelmet.itemData.afMaxHP;
                manaBuff += eqpHelmet.itemData.afMaxMana;
            }

            if (eqpArmor != null)
            {
                vita += eqpArmor.itemData.afVita;
                str += eqpArmor.itemData.afStr;
                dex += eqpArmor.itemData.afDex;
                energy += eqpArmor.itemData.afEnergy;
                intelligence += eqpArmor.itemData.afIntelligence;

                phyDmg += eqpArmor.itemData.afPhysicalDmg;
                mgcDmg += eqpArmor.itemData.afMagicalDmg;
                def += eqpArmor.itemData.afDefence;
                critChance += eqpArmor.itemData.afCritChance;
                missChance += eqpArmor.itemData.afMissChance;
                mCR += eqpArmor.itemData.afManaCostReduce;

                aS += eqpArmor.itemData.afAttackSpeed;
                cS += eqpArmor.itemData.afCastSpeed;

                cM += eqpArmor.itemData.afCritMultiplier;
                eM += eqpArmor.itemData.afXPMultiplier;

                hpBuff += eqpArmor.itemData.afMaxHP;
                manaBuff += eqpArmor.itemData.afMaxMana;
            }

            if (eqpWpn != null)
            {
                cAttackSpeed = eqpWpn.swordRate;
                vita += eqpWpn.itemData.afVita;
                str += eqpWpn.itemData.afStr;
                dex += eqpWpn.itemData.afDex;
                energy += eqpWpn.itemData.afEnergy;
                intelligence += eqpWpn.itemData.afIntelligence;

                phyDmg += eqpWpn.itemData.afPhysicalDmg;
                mgcDmg += eqpWpn.itemData.afMagicalDmg;
                def += eqpWpn.itemData.afDefence;
                critChance += eqpWpn.itemData.afCritChance;
                missChance += eqpWpn.itemData.afMissChance;
                mCR += eqpWpn.itemData.afManaCostReduce;

                aS += eqpWpn.itemData.afAttackSpeed;
                cS += eqpWpn.itemData.afCastSpeed;

                cM += eqpWpn.itemData.afCritMultiplier;
                eM += eqpWpn.itemData.afXPMultiplier;

                hpBuff += eqpWpn.itemData.afMaxHP;
                manaBuff += eqpWpn.itemData.afMaxMana;
            }

            if (eqpAmulet != null)
            {
                vita += eqpAmulet.itemData.afVita;
                str += eqpAmulet.itemData.afStr;
                dex += eqpAmulet.itemData.afDex;
                energy += eqpAmulet.itemData.afEnergy;
                intelligence += eqpAmulet.itemData.afIntelligence;

                phyDmg += eqpAmulet.itemData.afPhysicalDmg;
                mgcDmg += eqpAmulet.itemData.afMagicalDmg;
                def += eqpAmulet.itemData.afDefence;
                critChance += eqpAmulet.itemData.afCritChance;
                missChance += eqpAmulet.itemData.afMissChance;
                mCR += eqpAmulet.itemData.afManaCostReduce;

                aS += eqpAmulet.itemData.afAttackSpeed;
                cS += eqpAmulet.itemData.afCastSpeed;

                cM += eqpAmulet.itemData.afCritMultiplier;
                eM += eqpAmulet.itemData.afXPMultiplier;

                hpBuff += eqpAmulet.itemData.afMaxHP;
                manaBuff += eqpAmulet.itemData.afMaxMana;
            }

            if (eqpRing != null)
            {
                vita += eqpRing.itemData.afVita;
                str += eqpRing.itemData.afStr;
                dex += eqpRing.itemData.afDex;
                energy += eqpRing.itemData.afEnergy;
                intelligence += eqpRing.itemData.afIntelligence;

                phyDmg += eqpRing.itemData.afPhysicalDmg;
                mgcDmg += eqpRing.itemData.afMagicalDmg;
                def += eqpRing.itemData.afDefence;
                critChance += eqpRing.itemData.afCritChance;
                missChance += eqpRing.itemData.afMissChance;
                mCR += eqpRing.itemData.afManaCostReduce;

                aS += eqpRing.itemData.afAttackSpeed;
                cS += eqpRing.itemData.afCastSpeed;

                cM += eqpRing.itemData.afCritMultiplier;
                eM += eqpRing.itemData.afXPMultiplier;

                hpBuff += eqpRing.itemData.afMaxHP;
                manaBuff += eqpRing.itemData.afMaxMana;
            }

            foreach (MagicBook book in eqpMagicBooks)
            {
                if (book != null)
                {
                    vita += book.itemData.afVita;
                    str += book.itemData.afStr;
                    dex += book.itemData.afDex;
                    energy += book.itemData.afEnergy;
                    intelligence += book.itemData.afIntelligence;

                    phyDmg += book.itemData.afPhysicalDmg;

                    mgcDmg += book.itemData.afMagicalDmg;

                    if (eqpMagicBooks[chosenSpellSlot] != null && !spellDmgAdded)
                    {
                        mgcDmg += eqpMagicBooks[chosenSpellSlot].spellDamage;
                        spellDmgAdded = true;
                    }

                    def += book.itemData.afDefence;
                    critChance += book.itemData.afCritChance;
                    missChance += book.itemData.afMissChance;
                    mCR += book.itemData.afManaCostReduce;

                    aS += book.itemData.afAttackSpeed;
                    cS += book.itemData.afCastSpeed;

                    cM += book.itemData.afCritMultiplier;
                    eM += book.itemData.afXPMultiplier;

                    hpBuff += book.itemData.afMaxHP;
                    manaBuff += book.itemData.afMaxMana;
                }
            }

            if (eqpLegArmor != null)
            {
                vita += eqpLegArmor.itemData.afVita;
                str += eqpLegArmor.itemData.afStr;
                dex += eqpLegArmor.itemData.afDex;
                energy += eqpLegArmor.itemData.afEnergy;
                intelligence += eqpLegArmor.itemData.afIntelligence;

                phyDmg += eqpLegArmor.itemData.afPhysicalDmg;
                mgcDmg += eqpLegArmor.itemData.afMagicalDmg;
                def += eqpLegArmor.itemData.afDefence;
                critChance += eqpLegArmor.itemData.afCritChance;
                missChance += eqpLegArmor.itemData.afMissChance;
                mCR += eqpLegArmor.itemData.afManaCostReduce;

                aS += eqpLegArmor.itemData.afAttackSpeed;
                cS += eqpLegArmor.itemData.afCastSpeed;

                cM += eqpLegArmor.itemData.afCritMultiplier;
                eM += eqpLegArmor.itemData.afXPMultiplier;

                hpBuff += eqpLegArmor.itemData.afMaxHP;
                manaBuff += eqpLegArmor.itemData.afMaxMana;
            }

            if (eqpGloves != null)
            {
                vita += eqpGloves.itemData.afVita;
                str += eqpGloves.itemData.afStr;
                dex += eqpGloves.itemData.afDex;
                energy += eqpGloves.itemData.afEnergy;
                intelligence += eqpGloves.itemData.afIntelligence;

                phyDmg += eqpGloves.itemData.afPhysicalDmg;
                mgcDmg += eqpGloves.itemData.afMagicalDmg;
                def += eqpGloves.itemData.afDefence;
                critChance += eqpGloves.itemData.afCritChance;
                missChance += eqpGloves.itemData.afMissChance;
                mCR += eqpGloves.itemData.afManaCostReduce;

                aS += eqpGloves.itemData.afAttackSpeed;
                cS += eqpGloves.itemData.afCastSpeed;

                cM += eqpGloves.itemData.afCritMultiplier;
                eM += eqpGloves.itemData.afXPMultiplier;

                hpBuff += eqpGloves.itemData.afMaxHP;
                manaBuff += eqpGloves.itemData.afMaxMana;
            }

            if (eqpBoots != null)
            {
                vita += eqpBoots.itemData.afVita;
                str += eqpBoots.itemData.afStr;
                dex += eqpBoots.itemData.afDex;
                energy += eqpBoots.itemData.afEnergy;
                intelligence += eqpBoots.itemData.afIntelligence;

                phyDmg += eqpBoots.itemData.afPhysicalDmg;
                mgcDmg += eqpBoots.itemData.afMagicalDmg;
                def += eqpBoots.itemData.afDefence;
                critChance += eqpBoots.itemData.afCritChance;
                missChance += eqpBoots.itemData.afMissChance;
                mCR += eqpBoots.itemData.afManaCostReduce;

                aS += eqpBoots.itemData.afAttackSpeed;
                cS += eqpBoots.itemData.afCastSpeed;

                cM += eqpBoots.itemData.afCritMultiplier;
                eM += eqpBoots.itemData.afXPMultiplier;

                hpBuff += eqpBoots.itemData.afMaxHP;
                manaBuff += eqpBoots.itemData.afMaxMana;
            }

            cVita = vita + _Vita + sVita;
            cStr = str + _Str + sStr;
            cDex = dex + _Dex + sDex;
            cEnergy = energy + _Energy + sEnergy;
            cIntel = intelligence + _Intel + sIntel;

            cPhyDmg = Mathf.FloorToInt((float)(phyDmg + _phyDmg) * _dmgMagnitude) + Mathf.FloorToInt((float)cStr / 4f);
            cMgcDmg = Mathf.FloorToInt((float)(mgcDmg + _mgcDmg) * _mgcMagnitude) + Mathf.FloorToInt((float)cIntel / 4f);
            cDef = def + Mathf.FloorToInt((float)cStr / 4f);
            cCritChance = critChance + _critChance + Mathf.FloorToInt((float)cDex / 10f);
            cMissChance = missChance + _missChance - Mathf.FloorToInt((float)cDex / 10f);
            cManaCostReduce = mCR + Mathf.FloorToInt((float)cIntel / 4f);

            cAttackSpeed = cAttackSpeed + aS - (float)cDex / 50f;
            cCastSpeed = _castSpeed + cS;

            cCritMultiplier = _CritMultiplier + cM + (float)cStr / 30f;
            cXPMultiplier = _XPMultiplier + eM;

            maxHP = hpBuff + _HP + Mathf.FloorToInt((float)cVita / 4f);
            maxMana = manaBuff + _Mana + Mathf.FloorToInt((float)cEnergy / 4f);

            cHPRegen = _HPRegen + Mathf.FloorToInt((float)cVita / 20f);
            cManaRegen = _manaRegen + Mathf.FloorToInt((float)cEnergy / 20f);


            minDPS = Mathf.FloorToInt((float)cPhyDmg * 0.8f / cAttackSpeed);
            if (minDPS < 0)
            {
                minDPS = 0;
            }
            maxDPS = Mathf.CeilToInt((float)cPhyDmg * 1.2f / cAttackSpeed * cCritMultiplier);
            if (maxDPS < 0)
            {
                maxDPS = 0;
            }
            // Speciallar neden yukarıda eklenmiyor // Eklendiler
            saveTempSpecialPoints();
            //updateSpecials();

            //updateHealth();
            //updateMana();
        }

        public void saveTempSpecialPoints()
        {
            tempPoints = availablePoints;

            tempVita = sVita;
            tempStr = sStr;
            tempDex = sDex;
            tempEnergy = sEnergy;
            tempIntel = sIntel;
            //to Show
            tempCVita = cVita;
            tempCStr = cStr;
            tempCDex = cDex;
            tempCEnergy = cEnergy;
            tempCIntel = cIntel;
        }

        public void uP()
        {
            canUndo = false;
            UIManager.Instance.activateUndoButton(false);

            availablePoints = tempPoints;

            sVita = tempVita;
            sStr = tempStr;
            sDex = tempDex;
            sEnergy = tempEnergy;
            sIntel = tempIntel;
            //to Show
            cVita = tempCVita;
            cStr = tempCStr;
            cDex = tempCDex;
            cEnergy = tempCEnergy;
            cIntel = tempCIntel;
        }

        public void sP()
        {
            if (availablePoints > 0 && inSession)
            {
                switch (selectedAttribute)
                {
                    case "Vitality":
                        sVita++;
                        // to Show
                        cVita++;
                        break;
                    case "Strength":
                        sStr++;
                        // to Show
                        cStr++;
                        break;
                    case "Dexterity":
                        sDex++;
                        // to Show
                        cDex++;
                        break;
                    case "Energy":
                        sEnergy++;
                        // to Show
                        cEnergy++;
                        break;
                    case "Intelligence":
                        sIntel++;
                        // to Show
                        cIntel++;
                        break;
                }

                availablePoints--;
                canUndo = true;
                UIManager.Instance.activateUndoButton(true);

                if (availablePoints <= 0)
                {
                    UIManager.Instance.activateAddPointButton(false);
                    UIManager.Instance.selectSpecial("");
                }

                //updateChanges();
            }
        }

        public void finalizeSpecials()
        {
            canUndo = false;
            UIManager.Instance.activateUndoButton(false);
            selectedAttribute = null;
        }

        public float getDamageRandomizer()
        {
            return Random.Range(0.8f, 1.2f);
        }

        /*public void updateHealth()
        {
            myHB.updateHealth(this);
        }

        public void updateMana()
        {
            myMB.updateMana(this);
        }

        public void updateXP()
        {
            myLB.updateXP(this);
        }*/

        IEnumerator regen()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.4f);
                if (currentMana < maxMana)
                {
                    currentMana += cManaRegen;
                }

                if (currentHP < maxHP)
                {
                    currentHP += cHPRegen;
                }

                if (currentMana > maxMana)
                {
                    currentMana = maxMana;
                }

                if (currentHP > maxHP)
                {
                    currentHP = maxHP;
                }

                //updateMana();
                //updateHealth();
            }
        }
    }
}