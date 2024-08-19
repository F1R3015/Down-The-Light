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
    SelectAction, SelectAbility, SelectObject, SelectTargetEnemy, WaitingTurn, WaitingAttack
}

public class BattleBattleController : MonoBehaviour // CHANGE ABILITY MENU SYSTEM TO FUNCTION LIKE ACTION SYSTEM -> USE THE AOFA BUTTONS AND NOT UI 
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
    private Creature _creature; // Should be changed by the BattleSystem and not in the inspector

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

    [Header("Bars")]
    [SerializeField]
    private GameObject _hpBar;

    [SerializeField]
    private GameObject _manaBar;



    #endregion

    #region Propierties
    private GameObject[] _enemies;
    private GameObject[] _allies;
    [SerializeField] private int _selectedAbility;
    private Ability _chosenAbility;
    private int _selectTarget;
    public int SelectTarget
    {
        get { return _selectTarget; }
        set {
            if (_selectTarget != -1)
            {
                _enemies[_selectTarget].transform.GetChild(0).gameObject.SetActive(false);
                _enemies[_selectTarget].transform.GetChild(1).gameObject.SetActive(false);
                
            }
            _selectTarget = value;
            if (_selectTarget != -1)
            {
                _enemies[_selectTarget].transform.GetChild(0).gameObject.SetActive(true);
                _enemies[_selectTarget].transform.GetChild(1).gameObject.SetActive(true);

            }
        }
    }
    private BattleTurnState _turnState;
    private BattleTurnState _previousTurn;


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
        _abilityUI1.GetComponentInChildren<Text>().text = _creature._ability1.name; // Create function that checks if ability then name else blank ( could be other image better? ) 
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _enemies = SortEntitiesByXPosition(_enemies);
        _allies = GameObject.FindGameObjectsWithTag("Ally");
        _allies = SortEntitiesByXPosition(_allies);
        _chosenAbility = null;
        //Delete when turns system completed ( The Battle Manager will be the one doing it) 

        _turnState = BattleTurnState.SelectAction; // Should be Waiting when turn system completed and using EnterNewState()

        AOFAEnable();
        AcceptEnable();

    }

    // CHANGE AOFA AND ACTIONS TO BE LESS CONFUSING ??? ( NEW FUNCTIONS WITH CLEARER INTENTIONS )

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
                _previousTurn = _turnState;
                _rightAction.Enable();
                _leftAction.Enable();
                _leftAction.performed += Left;
                _rightAction.performed += Right;
                _turnState = BattleTurnState.SelectTargetEnemy; // Change to a different state due to the ability
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
                break;
            case BattleTurnState.SelectTargetEnemy:
                SelectTarget = -1;
                _rightAction.Disable();
                _leftAction.Disable();
                _leftAction.performed -= Left;
                _rightAction.performed -= Right;
                switch (_previousTurn)
                {
                    case BattleTurnState.SelectAbility:
                        _abilityMenu.SetActive(true);
                        _turnState = _previousTurn;
                        _previousTurn = BattleTurnState.SelectAction;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    private void Left(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectTargetEnemy:
                SelectTarget = _selectTarget == 0 ? _enemies.Length - 1 : _selectTarget - 1;
                break;
            default:
                break;
        }
    }

    private void Right(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case BattleTurnState.SelectTargetEnemy:
                SelectTarget = _selectTarget == _enemies.Length - 1  ? 0 : _selectTarget + 1;
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
    #endregion

    #region Enable and disable actions ( Change this later ) 
    public void EnableAOFA(InputAction.CallbackContext context)
    {
        _abilityMenu.SetActive(false);
        _goBackButton.SetActive(false);
        _cancelAction.Disable();
        _cancelAction.performed -= EnableAOFA;
        AOFAEnable();
        
    }

    public void DisableAOFA(InputAction.CallbackContext context)
    {
        AOFADisable();
    }
    #endregion

    //Switch ability description when ability selected or mouse enter
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

    public void AbilityChosen(int _ability)
    {
        
        switch (_ability)
        {
            case 1:
                if (!_creature._ability1.IsUnityNull())
                {
                    _abilityMenu.SetActive(false);
                    _chosenAbility = _creature._ability1;
                    TargetSelectSwitch(_chosenAbility._affected);
                }
                break;
            case 2:
                if (!_creature._ability2.IsUnityNull())
                {
                    _abilityMenu.SetActive(false);
                    _chosenAbility = _creature._ability2;
                    TargetSelectSwitch(_chosenAbility._affected);

                }
                break;
            case 3:
                if (!_creature._ability3.IsUnityNull())
                {
                    _abilityMenu.SetActive(false);
                    _chosenAbility = _creature._ability3;
                    TargetSelectSwitch(_chosenAbility._affected);

                }
                break;
            case 4:
                if (!_creature._ability4.IsUnityNull())
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


    // When an abilty or object is used, decides what is targeted based on the type
    private void TargetSelectSwitch(Type _type)
    {
        switch (_type)
        {
            case Type.Enemy:
                SelectTarget = 0;
                // Enable the select cursor and health bar
                break;

            case Type.AllEnemies:
                break;

            case Type.Ally:
                _selectTarget = 0;
                break;

            case Type.AllAllies:
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
    
}


