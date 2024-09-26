using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Creature")]
public class Creature : ScriptableObject
{ 
    #region Variables

    public int _maximumHealth;


    public int _maxMana;

    public Element _primaryElement;

    public Element _secondaryElement;


    public int _baseStrength;


    public int _baseDefence;


    public int _baseSpeed;


    public int _baseAccuracy;

    public Ability _ability1;

    public Ability _ability2;

    public Ability _ability3;

    public Ability _ability4;


    #endregion
}
    
