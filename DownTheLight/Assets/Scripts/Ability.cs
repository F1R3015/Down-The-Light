using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Ability")]

public class Ability : ScriptableObject
{



    public Type _elementalType;

    public string _name;
    
    public string _description;

    public int _manaCost;

    public int _damagePoint;

    public int _selfDamagePoint;

    public int _healPoint;

    public int _strengthPoint;

    public int _speedPoint;

    public int _defencePoint;

    public int _accuracyPoint;

    public int _manaPoint;

}
