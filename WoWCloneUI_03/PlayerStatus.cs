using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    public string playerName;

    public int level, experience, maxXp;

    public bool addXp;
    public int addXpAmount;

    public SliderBar xpSlider;
    public TextMeshProUGUI levelText;

    //Stats
    //secondary
    public SecondaryStats currentSecondary;
    [HideInInspector]
    public SecondaryStats maxSecondary;

    //primary
    public PrimaryStats currentPrimary;
    [HideInInspector]
    public PrimaryStats basePrimary;

    //UI references
    //name
    public TextMeshProUGUI nameText;

    //status bars
    public SliderBar healthSlider;
    public SliderBar magicSlider;
    public SliderBar energySlider;

    // Start is called before the first frame update
    void Start()
    {
        StatSetup(10, 10, 10, 10, 10);
        SetPlayerName();

        levelText.SetText(level.ToString());
        UpdateXP();
    }

    // Update is called once per frame
    void Update()
    {
        LimitStatValues();
        UpdateStatus();

        if(addXp)
        {
            UpdateXP();
            addXp = false;
        }
    }

    void StatSetup(int str, int agi, int intl, int stam, int spr)
    {
        basePrimary = new PrimaryStats(str, agi, intl, stam, spr);
        currentPrimary = new PrimaryStats(basePrimary.strength, basePrimary.agility, basePrimary.intellect, basePrimary.stamina, basePrimary.spirit);

        maxSecondary = new SecondaryStats(currentPrimary.stamina * 10, currentPrimary.intellect * 10, currentPrimary.agility * 10);
        currentSecondary = new SecondaryStats(maxSecondary.health, maxSecondary.magic, maxSecondary.energy);
    }

    void SetPlayerName()
    {
        nameText.SetText(playerName);
    }

    void UpdateStatus()
    {
        healthSlider.SetSliderValues(0, maxSecondary.health, currentSecondary.health);
        magicSlider.SetSliderValues(0, maxSecondary.magic, currentSecondary.magic);
        energySlider.SetSliderValues(0, maxSecondary.energy, currentSecondary.energy);
    }

    void LimitStatValues()
    {
        currentSecondary.health = Mathf.Clamp(currentSecondary.health, 0, maxSecondary.health);
        currentSecondary.magic = Mathf.Clamp(currentSecondary.magic, 0, maxSecondary.magic);
        currentSecondary.energy = Mathf.Clamp(currentSecondary.energy, 0, maxSecondary.energy);
    }

    void UpdateXP()
    {
        experience += addXpAmount;

        if(experience >= maxXp)
        {
            int levelsGained = experience / maxXp;

            experience -= maxXp * levelsGained;
            level += levelsGained;

            levelText.SetText(level.ToString());
        }

        xpSlider.SetSliderValues(0, maxXp, experience);
    }
}

[System.Serializable]
public class PrimaryStats
{
    public int strength, agility, intellect, stamina, spirit;

    public PrimaryStats(int str, int agi, int intl, int stam, int spr)
    {
        strength = str;
        agility = agi;
        intellect = intl;
        stamina = stam;
        spirit = spr;
    }
}

[System.Serializable]
public class SecondaryStats
{
    public int health, magic, energy;

    public SecondaryStats(int heal, int mag, int nrg)
    {
        health = heal;
        magic = mag;
        energy = nrg;
    }
}