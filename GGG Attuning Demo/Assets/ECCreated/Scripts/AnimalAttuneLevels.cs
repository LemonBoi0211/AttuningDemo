using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimalAttuneLevels : MonoBehaviour
{
    public Image levelBar;
    public Text levelNumber;
    HoldAnimalValues hAV;

    //Player xp and levels
    float playerAttuneLevel;
    float playerCurrentXP;
    float playerTotalXP;

    float xpToNextLevel;
    float xpToFirstLevel;

    //Animal levels and values
    float startleValue;
    public float animalLevel;


    private void Update()
    {
        HandlePlayerLevels();
        HandleAnimalLevels();
    }

    void HandlePlayerLevels()
    {
        playerAttuneLevel = 0;
        playerCurrentXP = 0;
        playerTotalXP = 0;
        xpToFirstLevel = 100;

        //1 - 100, 2 - 150, 3 - 200, etc

        float i = Mathf.InverseLerp(playerTotalXP, xpToNextLevel, playerCurrentXP);
        levelBar.fillAmount = i;

        if(playerCurrentXP == xpToNextLevel)
        {
            playerAttuneLevel++;
            levelBar.fillAmount = 0;
            levelNumber.text = playerAttuneLevel.ToString();
            xpToNextLevel = xpToFirstLevel + 50;
        }
    }

    void HandleAnimalLevels()
    {
        if(playerAttuneLevel <= 2)
        {
            animalLevel = 1;
        }
        else if(playerAttuneLevel >= 3)
        {
            animalLevel = playerAttuneLevel + Random.Range(playerAttuneLevel - 2, playerAttuneLevel + 2);
        }


        
    }
}
