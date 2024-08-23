using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum BattleTurnState
{
    SelectAction, SelectAbility, SelectObject, SelectTarget, Waiting
}
// NOTE: SHOULD ADD NUMBERS TO HEALTH AND MANA CANVAS
public class PlayerBattleController : MonoBehaviour // CHANGE ABILITY MENU SYSTEM TO FUNCTION LIKE ACTION SYSTEM -> USE THE AOFA BUTTONS AND NOT UI 
{
    #region Input Actions
    PlayerAction _inputAction;
    InputAction _attackAction;
    InputAction _objectAction;
    InputAction _fleeAction;
    InputAction _alchemyAction;
    InputAction _cancelAction;
    InputAction _acceptAction;
    InputAction _leftAction;
    InputAction _rightAction;
    #endregion

    #region Serialized Propierties

    [Header("Battle Buttons")]

    [SerializeField]
    private GameObject _battleButtons;

    [SerializeField]
    private GameObject _attackButton;

    [SerializeField]
    private GameObject _objectButton;

    [SerializeField]
    private GameObject _fleeButton;

    [SerializeField]
    private GameObject _alchemyButton;

    [Header("")]
    [SerializeField]
    private GameObject _abilityMenu;


    [Header("Current Creature")]
    [SerializeField]
    private CreatureStats _creature; // Should be changed by the BattleSystem and not in the inspector

    [Header("Abilities UI")]
    [SerializeField]
    private GameObject _abilityUI1;
    [SerializeField]
    private GameObject _abilityUI2;
    [SerializeField]
    private GameObject _abilityUI3;
    [SerializeField]
    private GameObject _abilityUI4;
    [SerializeField]
    private GameObject _abilityDescriptionUI;
    [SerializeField]
    private GameObject _goBackButton;




    #endregion

    #region Propierties
    private GameObject[] _enemies;
    private GameObject[] _allies;
    private int _selectedAbility;
    private Ability _chosenAbility;
    private Type _targetType;
    private int _selectTarget; // Indicates position in the array of the target / -1 if there is no target
    public int SelectTarget
    {
        get { return _selectTarget; }
        set {
            if (_selectTarget != -1)
            {
                switch (_targetType)
                {
                    case Type.Enemy:
                        _enemies[_selectTarget].transform.GetChild(0).gameObject.SetActive(false);
                        _enemies[_selectTarget].transform.GetChild(1).gameObject.SetActive(false);
                        break;
                    case Type.Ally:
                        _allies[_selectTarget].transform.GetChild(0).gameObject.SetActive(false); 
                        _allies[_selectTarget].transform.GetChild(1).gameObject.SetActive(false);// DELETE / ALLIES WILL HAVE A SPECIAL PLACE FOR DISPLAYING HEALTH AND MANA
                        break;
                }
                
            }
            _selectTarget = value;
            if (_selectTarget != -1)
            {
                switch (_targetType)
                {
                    case Type.Enemy:
                        _enemies[_selectTarget].transform.GetChild(0).gameObject.SetActive(true);
                        _enemies[_selectTarget].transform.GetChild(1).gameObject.SetActive(true);
                        break;
                    case Type.Ally:
                        _allies[_selectTarget].transform.GetChild(0).gameObject.SetActive(true);
                        _allies[_selectTarget].transform.GetChild(1).gameObject.SetActive(true);
                        break;
                }

            }
        }
    }
    private BattleTurnState _turnState;
    private BattleTurnState _previousTurn;
    private BattleSystem _battleSystem; 

    #endregion

    private void Awake()
    {
        _inputAction = new PlayerAction();
        _attackAction = _inputAction.Battle.Attack;
        _objectAction = _inputAction.Battle.Object;
        _fleeAction = _inputAction.Battle.Flee;
        _alchemyAction = _inputAction.Battle.Alchemy;
        _cancelAction = _inputAction.Battle.Cancel;
        _acceptAction = _inputAction.Battle.Accept;
        _leftAction = _inputAction.Battle.Left;
        _rightAction = _inputAction.Battle.Right;
        _abilityUI1.GetComponentInChildren<Text>().text = !_creature._ability1.IsUnityNull() ? _creature._ability1.name : ""; 
        _abilityUI2.GetComponentInChildren<Text>().text = !_creature._ability2.IsUnityNull() ? _creature._ability2.name : ""; 
        _abilityUI3.GetComponentInChildren<Text>().text = !_creature._ability3.IsUnityNull() ? _creature._ability3.name : ""; 
        _abilityUI4.GetComponentInChildren<Text>().text = !_creature._ability4.IsUnityNull() ? _creature._ability4.name : ""; 
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _enemies = SortEntitiesByXPosition(_enemies);
        _allies = GameObject.FindGameObjectsWithTag("Ally");
        _allies = SortEntitiesByXPosition(_allies);
        _chosenAbility = null;
        _battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
    }

    

    #region Action Button Functions

    private void Attack(InputAction.CallbackContext context)
    {
        
        switch (_turnState)
        {
            case BattleTurnState.SelectAction: // CHANGE WITH CANCEL AND ACCEPT
                _abilityMenu.SetActive(true);
                _battleButtons.SetActive(false);
                _goBackButton.SetActive(true);
                _cancelAction.Enable();
                _cancelAction.performed += Cancel;
                _previousTurn = _turnState;
                _turnState = BattleTurnState.SelectAbility;

                break;
            case BattleTurnState.SelectAbility:
                AbilitySelected(1);
                break;
            default:
                break;
        }
        
    } // A (XBOX) SQUARE (PLAY) B (NINTENDO)

    private void Object(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectAction:
                AOFADisable();
                break;
            case BattleTurnState.SelectAbility:
                AbilitySelected(2);
                break;
            default:
                break;
        }
    } // X (XBOX) CUBE (PLAY) Y (NINTENDO)

    private void Flee(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectAction:
                AOFADisable();
                break;
            case BattleTurnState.SelectAbility:
                AbilitySelected(3);
                break;
            default:
                break;
        }
    } // B (XBOX) CIRCLE (PLAY) A (NINTENDO)

    private void Alchemy(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectAction:
                AOFADisable();
                break;
            case BattleTurnState.SelectAbility:
                AbilitySelected(4);
                break;
            default:
                break;
        }
    } // Y (XBOX) TRIANGLE (PLAY) X (NINTENDO)

    private void Accept(InputAction.CallbackContext context) // RB (XBOX) R1 (PLAY) R (NINTENDO)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectAbility:
                AbilityChosen(_selectedAbility);
                break;
            case BattleTurnState.SelectTarget:
                _goBackButton.SetActive(false);
                switch (_targetType)
                {
                    case Type.Enemy:
                        _enemies[_selectTarget].transform.GetChild(0).gameObject.SetActive(false);
                        DoAction(_enemies[_selectTarget]); // Note:  Call  coroutine better for animation + waiting 
                        _enemies[_selectTarget].transform.GetChild(1).gameObject.SetActive(false);
                        break;
                    case Type.Ally:
                        _allies[_selectTarget].transform.GetChild(0).gameObject.SetActive(false);
                        DoAction(_allies[_selectTarget]); // Note:  Call  coroutine better for animation + waiting 
                        _enemies[_selectTarget].transform.GetChild(1).gameObject.SetActive(false);
                        break;
                    case Type.AllEnemies:
                        foreach(GameObject _enemy in _enemies)
                        {
                            _enemy.transform.GetChild(0).gameObject.SetActive(false);
                            DoAction(_enemy); // Note:  Call  coroutine better for animation + waiting 
                        }
                        foreach(GameObject _enemy in _enemies)
                        {
                            _enemy.transform.GetChild(1).gameObject.SetActive(false);
                        }
                        break;
                    case Type.AllAllies:
                        foreach(GameObject _ally in _allies)
                        {
                            _ally.transform.GetChild(0).gameObject.SetActive(false);
                            DoAction(_ally); // Note:  Call  coroutine better for animation + waiting 
                        }
                        foreach(GameObject _enemy in _enemies)
                        {
                            _enemy.transform.GetChild(1).gameObject.SetActive(false);
                        }
                        break;
                }
                // Set null all not necesary things -->
                _turnState = BattleTurnState.Waiting;
                _previousTurn = BattleTurnState.Waiting;
                _selectedAbility = -1;
                _chosenAbility = null;
                _targetType = Type.None;
                SelectTarget = -1;
                _abilityDescriptionUI.GetComponentInChildren<Text>().text = ""; // Resets the description to null
                // <--
                EndTurn();
                break;

            default:
                break;
        }

    }

    private void Cancel(InputAction.CallbackContext context) // LB (XBOX) L1 (PLAY) L (NINTENDO)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectAbility:
                _abilityMenu.SetActive(false);
                _battleButtons.SetActive(true);
                _goBackButton.SetActive(false);
                _cancelAction.Disable();
                _cancelAction.performed -= Cancel;
                _turnState = BattleTurnState.SelectAction;
                _previousTurn = _turnState;
                _selectedAbility = -1;
                break;
            case BattleTurnState.SelectTarget:
                switch (_targetType) // Part 1 : Disable enemies or allies
                {
                    case Type.Enemy:
                    case Type.Ally:
                        SelectTarget = -1;
                        _rightAction.Disable();
                        _leftAction.Disable();
                        _leftAction.performed -= Left;
                        _rightAction.performed -= Right;
                        break;

                    case Type.AllEnemies:
                        foreach(GameObject _currentGO in _enemies)
                        {
                            _currentGO.transform.GetChild(0).gameObject.SetActive(false);
                            _currentGO.transform.GetChild(1).gameObject.SetActive(false);
                        }
                        break;
                    case Type.AllAllies:
                        foreach (GameObject _currentGO in _enemies)
                        {
                            _currentGO.transform.GetChild(0).gameObject.SetActive(false);
                            _currentGO.transform.GetChild(1).gameObject.SetActive(false); // DELETE WHEN ALLIES HAVE SPECIAL PLACE IN UI
                        }
                        break;
                    default:
                        Debug.Log("ERROR: TYPE ERROR WHEN CANCELING TARGET");
                        break;
                }



                
                switch (_previousTurn)
                {
                    case BattleTurnState.SelectAbility:
                        _abilityMenu.SetActive(true);
                        _turnState = _previousTurn;
                        _previousTurn = BattleTurnState.SelectAction;
                        _chosenAbility = null;
                        break;
                    default:
                        break;
                } // Part 2: Go back
                break;
            default:
                break;
        }
    }

    private void Left(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectTarget:
                switch (_chosenAbility._affected)
                {
                    case Type.Enemy:
                        SelectTarget = _selectTarget == 0 ? _enemies.Length - 1 : _selectTarget - 1;
                        break;
                    case Type.Ally:
                        SelectTarget = _selectTarget == 0 ? _allies.Length - 1 : _selectTarget - 1;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    private void Right(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectTarget:
                switch (_chosenAbility._affected)
                {
                    case Type.Enemy:
                        SelectTarget = _selectTarget == _enemies.Length - 1 ? 0 : _selectTarget + 1;
                        break;
                    case Type.Ally:
                        SelectTarget = _selectTarget == _allies.Length - 1 ? 0 : _selectTarget + 1;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    #endregion

    #region Enable and disable AOFA (Maybe separate for each button?)
    private void AOFAEnable()
    {

        //AOFA UI ENABLE
        _attackButton.SetActive(true);
        _objectButton.SetActive(true);
        _fleeButton.SetActive(true);
        _alchemyButton.SetActive(true);

        //AOFA ACTION ENABLE
        _attackAction.Enable();
        _attackAction.performed += Attack;

        _objectAction.Enable();
        _objectAction.performed += Object;

        _fleeAction.Enable();
        _fleeAction.performed += Flee;

        _alchemyAction.Enable();
        _alchemyAction.performed += Alchemy;
    }

    private void AOFADisable()
    {
        // AOFA UI DISABLE
        _attackButton.SetActive(false);
        _objectButton.SetActive(false);
        _fleeButton.SetActive(false);
        _alchemyButton.SetActive(false);

        // AOFA ACTIONS DISABLE
        _attackAction.Disable();
        _attackAction.performed -= Attack;

        _objectAction.Disable();
        _objectAction.performed -= Object;

        _fleeAction.Disable();
        _fleeAction.performed -= Flee;

        _alchemyAction.Disable();
        _alchemyAction.performed -= Alchemy;
        

    }

    private void AcceptEnable()
    {
        _acceptAction.Enable();
        _acceptAction.performed += Accept;
    }

    private void AcceptDisable()
    {
        _acceptAction.Disable();
        _acceptAction.performed -= Accept;
    }

    private void CancelDisable()
    {
        _cancelAction.Disable();
        _cancelAction.performed -= Cancel;
    }
    private void RightDisable()
    {
        _rightAction.Disable();
        _rightAction.performed -= Right;
    }
    private void LeftDisable()
    {
        _leftAction.Disable();
        _leftAction.performed -= Left;
    }

    private void EndTurn()
    {
        AOFADisable();
        AcceptDisable();
        CancelDisable();
        RightDisable();
        LeftDisable();
        _battleSystem.PlayerTurnFinished();
    }

    public void StartTurn()
    {
        _turnState = BattleTurnState.SelectAction; // Should be Waiting when turn system completed and using EnterNewState()
        _battleButtons.SetActive(true);
        AOFAEnable();
        AcceptEnable();
    }
    #endregion

    

    #region Ability Selection
    public void AbilitySelected(int _ability)
    {
        string _description;
        switch (_ability)
        {
            case 1:
                 _description = _creature._ability1.IsUnityNull() ? "" : _creature._ability1._description;
                _abilityDescriptionUI.GetComponentInChildren<Text>().text = _description;
                _selectedAbility = 1;
                break;
            case 2:
                 _description = _creature._ability2.IsUnityNull() ? "" : _creature._ability2._description;
                _abilityDescriptionUI.GetComponentInChildren<Text>().text = _description;
                _selectedAbility = 2;
                break;
            case 3:
                 _description = _creature._ability3.IsUnityNull() ? "" : _creature._ability3._description;
                _abilityDescriptionUI.GetComponentInChildren<Text>().text = _description;
                _selectedAbility = 3;
                break;
            case 4:
                 _description = _creature._ability4.IsUnityNull() ? "" : _creature._ability4._description;
                _abilityDescriptionUI.GetComponentInChildren<Text>().text = _description;
                _selectedAbility = 4;
                break;
        }
    }

    public void AbilityChosen(int _ability) // Note: If ability null or not enought mana make sound ( Also change ability appearence if no mana ) 
    {
        
        switch (_ability)
        {
            case 1:
                if (!_creature._ability1.IsUnityNull() && _creature._mana >= _creature._ability1._manaCost)
                {
                    _abilityMenu.SetActive(false);
                    _chosenAbility = _creature._ability1;
                    TargetSelectSwitch(_chosenAbility._affected);
                }
                break;
            case 2:
                if (!_creature._ability2.IsUnityNull() && _creature._mana >= _creature._ability1._manaCost)
                {
                    _abilityMenu.SetActive(false);
                    _chosenAbility = _creature._ability2;
                    TargetSelectSwitch(_chosenAbility._affected);

                }
                break;
            case 3:
                if (!_creature._ability3.IsUnityNull() && _creature._mana >= _creature._ability1._manaCost)
                {
                    _abilityMenu.SetActive(false);
                    _chosenAbility = _creature._ability3;
                    TargetSelectSwitch(_chosenAbility._affected);

                }
                break;
            case 4:
                if (!_creature._ability4.IsUnityNull() && _creature._mana >= _creature._ability1._manaCost)
                {
                    _abilityMenu.SetActive(false);
                    _chosenAbility = _creature._ability4;
                    TargetSelectSwitch(_chosenAbility._affected);

                }
                break;
            default:
                Debug.Log("NO ABILITY CHOSED");
                break;
        }
    }

    #endregion
    // When an abilty or object is used, decides what is targeted based on the type
    private void TargetSelectSwitch(Type _type)
    {
        switch (_type)
        {
            case Type.Enemy:
                _targetType = Type.Enemy;
                SelectTarget = 0;
                _previousTurn = _turnState;
                _rightAction.Enable();
                _leftAction.Enable();
                _leftAction.performed += Left;
                _rightAction.performed += Right;
                _turnState = BattleTurnState.SelectTarget; // Change to a different state due to the ability
                break;

            case Type.AllEnemies:
                _targetType = Type.AllEnemies;
                _previousTurn = _turnState;
                _turnState = BattleTurnState.SelectTarget;
                foreach (GameObject _currentGO in _enemies)
                {
                    _currentGO.transform.GetChild(0).gameObject.SetActive(true);
                    _currentGO.transform.GetChild(1).gameObject.SetActive(true);
                }
                break;

            case Type.Ally:
                _targetType = Type.Ally;
                SelectTarget = 0;
                _previousTurn = _turnState;
                _rightAction.Enable();
                _leftAction.Enable();
                _leftAction.performed += Left;
                _rightAction.performed += Right;
                _turnState = BattleTurnState.SelectTarget; // Change to a different state due to the ability
                break;

            case Type.AllAllies:
                _targetType = Type.AllAllies;
                _previousTurn = _turnState;
                _turnState = BattleTurnState.SelectTarget;
                foreach (GameObject _currentGO in _allies)
                {
                    _currentGO.transform.GetChild(0).gameObject.SetActive(true);
                    _currentGO.transform.GetChild(1).gameObject.SetActive(true); // DELETE WHEN ALLIES HAVE SPECIAL PLACE IN UI
                }
                break;
        }
    }

    // Method for sorting an array of gameobjects by their x position
    private GameObject[] SortEntitiesByXPosition(GameObject[] _entitiesArray)
    {
        if(_entitiesArray.Length <= 1)
        {
            return _entitiesArray;
        }
        GameObject _pivot = _entitiesArray[0];
        List<GameObject> _lowerPosition = new List<GameObject>();
        List<GameObject> _higherPosition = new List<GameObject>();
        for (int i = 1; i < _entitiesArray.Length; i++)
        {
            if(_pivot.transform.position.x < _entitiesArray[i].transform.position.x)
            {
                _higherPosition.Add(_entitiesArray[i]);
            }

            if(_pivot.transform.position.x > _entitiesArray[i].transform.position.x)
            {
                _lowerPosition.Add(_entitiesArray[i]);
            }
        }

        GameObject[] _lowerPositionArray = _lowerPosition.ToArray();
        _lowerPositionArray = SortEntitiesByXPosition(_lowerPosition.ToArray());
        GameObject[] _higherPositionArray = _higherPosition.ToArray();
        _higherPositionArray = SortEntitiesByXPosition(_higherPosition.ToArray());


        var _returnArray =  new GameObject[_lowerPositionArray.Length + _higherPositionArray.Length + 1];
        _lowerPositionArray.CopyTo(_returnArray, 0);
        _returnArray[_lowerPositionArray.Length] = _pivot;
        _higherPositionArray.CopyTo(_returnArray, _lowerPositionArray.Length + 1);
        return _returnArray;
    }
    
    private void DoAction(GameObject _target)  
    {
        CreatureStats _targetCreature = _target.GetComponent<CreatureStats>();
        Ability _ability = _chosenAbility; // Note: Should use an if to checks if its an ability or an object 
        // Note: Should be ordered
        if(_ability._manaCost != 0)
        {
            _creature.ChangeMana(-_ability._manaCost);
        }
        if(_ability._damagePoint != 0)
        {
            _targetCreature.ChangeHealth(-_ability._damagePoint); // If defeated remove from enemies[]
            if(_targetCreature._health <= 0)
            {
                _target.SetActive(false); // Note:  Should do it the gameObject itself
                _enemies = _enemies.Where(val => val != _target).ToArray(); // Alternatly, if enemies could die without player attack then they should disable and be ignored
            }
            Debug.Log($"{_targetCreature._health}/{_targetCreature._maximumHealth}");
        }
        if(_ability._healPoint != 0)
        {
            _targetCreature.ChangeHealth(_ability._healPoint);
        }
        if(_ability._accuracyPoint != 0)
        {
            _targetCreature.ChangeAccuracy(_ability._accuracyPoint);
        }
        if(_ability._defencePoint != 0)
        {
            _targetCreature.ChangeDefence( _ability._defencePoint);
        }
        if (_ability._selfDamagePoint != 0)
        {
            _creature.ChangeHealth(_ability._selfDamagePoint);
        }
        if(_ability._manaPoint != 0)
        {
            _targetCreature.ChangeMana(_ability._manaPoint);
        }
        if(_ability._speedPoint != 0)
        {
            _targetCreature.ChangeSpeed(_ability._speedPoint);
        }
        if(_ability._strengthPoint != 0)
        {
            _targetCreature.ChangeStrength( _ability._strengthPoint);
        }
    } // Note: Should also apply the player stats and  status effects ( last one not really necessary? ) 
    //Note : Two Differents Objects Types : Combat and Non-Combat

}


