using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldAnimalValues : MonoBehaviour
{
    [Header("Animal Attunement Levels")]
    [SerializeField] float chickenLevel;
    [SerializeField] float dogLevel;
    [SerializeField] float penguinLevel;
    [SerializeField] float catLevel;
    [SerializeField] float deerLevel;
    [SerializeField] float horseLevel;
    [SerializeField] float tigerLevel;

    [Header("Animal Startle Levels")]
    [SerializeField] float chickenStartle;
    [SerializeField] float dogStartle;
    [SerializeField] float penguinStartle;
    [SerializeField] float catStartle;
    [SerializeField] float deerStartle;
    [SerializeField] float horseStartle;
    [SerializeField] float tigerStartle;

    AnimalAttuneLevels aal;

    private void Update()
    {
        chickenLevel = aal.animalLevel;
        dogLevel = aal.animalLevel;
        penguinLevel = aal.animalLevel;
        catLevel = aal.animalLevel;
        deerLevel = aal.animalLevel;
        horseLevel = aal.animalLevel;
        tigerLevel = aal.animalLevel;
    }




}
