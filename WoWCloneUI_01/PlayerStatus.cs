using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    public string playerName;

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

    //Health
    public Image healthSlider;
    public TextMeshProUGUI healthText;

    //Magic
    public Image magicSlider;
    public TextMeshProUGUI magicText;

    //energy
    public Image energySlider;
    public TextMeshProUGUI energyText;

    // Start is called before the first frame update
    void Start()
    {
        StatSetup(10, 10, 10, 10, 10);
        SetPlayerName();
    }

    // Update is called once per frame
    void Update()
    {
        LimitStatValues();
        UpdateStatus();
    }

    void StatSetup(int str, int agi, int intl, int stam, int spr)
    {
        basePrimary = new PrimaryStats(str, agi, intl, stam, spr);
        currentPrimary = new PrimaryStats(basePrimary.strength, basePrimary.agility, basePrimary.intellect, basePrimary.stamina, basePrimary.spirit);

        maxSecondary = new SecondaryStats(currentPrimary.stamina * 10, currentPrimary.intellect * 10, currentPrimary.agility * 10);
        currentSecondary = new SecondaryStats(maxSecondary.health, maxSecondary.magic, maxSecondary.energy);
    }

    float Slider(int maxStat, int currentStat)
    {
        float sliderValue = 0;

        sliderValue = (float)currentStat / (float)maxStat;

        return sliderValue;
    }

    void SetPlayerName()
    {
        nameText.SetText(playerName);
    }

    void UpdateStatus()
    {
        //health
        healthSlider.fillAmount = Slider(maxSecondary.health, currentSecondary.health);
        healthText.SetText(currentSecondary.health.ToString() + "/" + maxSecondary.health.ToString());

        //magic
        magicSlider.fillAmount = Slider(maxSecondary.magic, currentSecondary.magic);
        magicText.SetText(currentSecondary.magic.ToString() + "/" + maxSecondary.magic.ToString());

        //energy
        energySlider.fillAmount = Slider(maxSecondary.energy, currentSecondary.energy);
        energyText.SetText(currentSecondary.energy.ToString() + "/" + maxSecondary.energy.ToString());
    }

    void LimitStatValues()
    {
        currentSecondary.health = Mathf.Clamp(currentSecondary.health, 0, maxSecondary.health);
        currentSecondary.magic = Mathf.Clamp(currentSecondary.magic, 0, maxSecondary.magic);
        currentSecondary.energy = Mathf.Clamp(currentSecondary.energy, 0, maxSecondary.energy);
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