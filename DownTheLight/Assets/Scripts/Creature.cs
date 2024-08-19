using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Creature")]
public class Creature : ScriptableObject
{
    public int _health ;

    public int _maximumHealth ;

    public int _mana ;

    public int _maxMana ;

    public Element _primaryElement ;

    public Element _secondaryElement ;

    public int _strength ;

    public int _baseStrength ;

    public int _defence ;

    public int _baseDefence ;

    public int _speed ;

    public int _baseSpeed ;

    public int _accuracy ;

    public int _baseAccuracy ;
    
    public Ability _ability1 ;
    
    public Ability _ability2 ;
    
    public Ability _ability3 ;
    
    public Ability _ability4 ;

}
