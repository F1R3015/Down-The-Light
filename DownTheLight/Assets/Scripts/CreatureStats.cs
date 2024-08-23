using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreatureStats : MonoBehaviour
{
    // Note: MUST add Math.Min(_maxVal,_newVal) and Max to ALL changes
    #region Variables 
    public int _health;

    public int _maximumHealth;

    public int _mana;

    public int _maxMana;

    public Element _primaryElement;

    public Element _secondaryElement;

    public int _strength;

    public int _baseStrength;

    public int _defence;

    public int _baseDefence;

    public int _speed; // Affects dodges and the time needed for recovering the turn

    public int _baseSpeed;

    public int _accuracy;

    public int _baseAccuracy;

    public Ability _ability1;

    public Ability _ability2;

    public Ability _ability3;

    public Ability _ability4;
    #endregion 

    public GameObject _healthBar;
    public GameObject _manaBar;
    [SerializeField]
    private Creature _creatureBlueprint;
    private BattleSystem _battleSystem;
    #region Functions
    public void ChangeHealth(int _healthAlter)
    {
        _health += _healthAlter;
        _healthBar.GetComponent<Image>().fillAmount =  (float) ((float) _health / (float) _maximumHealth); // Use a coroutine for a smooth update
    }

    public void ChangeMaxHealth(int _maxHealthAlter)
    {
        _maximumHealth += _maxHealthAlter;
    }

    public void ChangeMana(int _manaAlter)
    {
        _mana += _manaAlter;
        _manaBar.GetComponent<Image>().fillAmount =(float) _mana / (float) _maxMana;
    }

    public void ChangeMaxMana(int _maxManaAlter)
    {
        _maxMana += _maxManaAlter;
    }

    public void ChangePrimaryElement(Element _element)
    {
        _primaryElement = _element;
    }

    public void ChangeSecondaryElement(Element _element)
    {
        _secondaryElement = _element;
    }

    public void ChangeStrength(int _strenthAlter)
    {
        _strength += _strenthAlter;
    }

    public void ChangeBaseStrength(int _baseStrenthAlter)
    {
        _baseStrength += _baseStrenthAlter;
    }

    public void ChangeDefence(int _defenceAlter)
    {
        _defence += _defenceAlter;
    }

    public void ChangeBaseDefence(int _baseDefenceAlter)
    {
        _baseDefence += _baseDefenceAlter;
    }

    public void ChangeSpeed(int _speedAlter)
    {
        _speed += _speedAlter;
    }

    public void ChangeBaseSpeed(int _baseSpeedAlter)
    {
        _baseSpeed += _baseSpeedAlter;
    }

    public void ChangeAccuracy(int _accuracyAlter)
    {
        _accuracy += _accuracyAlter;
    }

    public void ChangeBaseAccuracy(int _baseAccuracyAlter)
    {
        _baseAccuracy += _baseAccuracyAlter;
    }

    public void ChangeAbility1(Ability _newAbility)
    {
        _ability1 = _newAbility;
    }
    public void ChangeAbility2(Ability _newAbility)
    {
        _ability2 = _newAbility;
    }
    public void ChangeAbility3(Ability _newAbility)
    {
        _ability3 = _newAbility;
    }
    public void ChangeAbility4(Ability _newAbility)
    {
        _ability4 = _newAbility;
    }

    public void ResetStats()
    {
        _strength = _baseStrength;
        _defence = _baseDefence;
        _speed = _baseSpeed;
        _accuracy = _baseAccuracy;
    } // Called whenever a fight is done

    public void InitializeStats()
    {
        _health = _creatureBlueprint._maximumHealth;
        _maximumHealth = _creatureBlueprint._maximumHealth;
        _mana = _creatureBlueprint._maxMana;
        _maxMana = _creatureBlueprint._maxMana;
        _strength = _creatureBlueprint._baseStrength;
        _baseStrength = _creatureBlueprint._baseStrength;
        _defence = _creatureBlueprint._baseDefence;
        _baseDefence = _creatureBlueprint._baseDefence;
        _speed = _creatureBlueprint._baseSpeed;
        _baseSpeed = _creatureBlueprint._baseSpeed;
        _accuracy= _creatureBlueprint._baseAccuracy;
        _baseAccuracy = _creatureBlueprint._baseAccuracy;
        _primaryElement = _creatureBlueprint._primaryElement;
        _secondaryElement = _creatureBlueprint._secondaryElement;
        _ability1 = _creatureBlueprint._ability1;
        _ability2 = _creatureBlueprint._ability2;
        _ability3 = _creatureBlueprint._ability3;
        _ability4 = _creatureBlueprint._ability4;
    } // Should only be called when creating creature first
    #endregion

    private void Awake  ()
    {
        InitializeStats();
        _battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
    }

    public IEnumerator WaitForNextTurn()
    {
        yield return new WaitForSeconds(10-_speed); // Wait Time should be less based in the speed / Note: Temporal is 10-_speed, should be changed in future
        _battleSystem.TurnReady(this.gameObject);
    }



}
