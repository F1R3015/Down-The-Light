using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum PlayerTurnState
{
    SelectAction, SelectAbility, SelectObject, SelectTarget, WaitingTurn, WaitingAttack
}

public class PlayerBattleController : MonoBehaviour // CHANGE ABILITY MENU SYSTEM TO FUNCTION LIKE ACTION SYSTEM -> USE THE AOFA BUTTONS AND NOT UI 
{
    #region Input Actions
    PlayerAction _inputAction;
    InputAction _attackAction;
    InputAction _objectAction;
    InputAction _fleeAction;
    InputAction _alchemyAction;
    InputAction _goBackAction;
    #endregion

    #region Serialized Propierties

    [Header("Battle Buttons")]
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

    [SerializeField]
    private GameObject _selectCursor;



    #endregion

    #region Propierties
    private GameObject[] _enemies;
    private GameObject[] _allies;
    private Ability _chosenAbility;
    private GameObject _selectTarget;
    public GameObject SelectTarget
    {
        get { return _selectTarget; }
        set {
            _selectTarget = value;
        }
    }
    private PlayerTurnState _turnState;

    #endregion

    private void Awake()
    {
        _inputAction = new PlayerAction();
        _attackAction = _inputAction.Player.Attack;
        _objectAction = _inputAction.Player.Object;
        _fleeAction = _inputAction.Player.Flee;
        _alchemyAction = _inputAction.Player.Alchemy;
        _goBackAction = _inputAction.Player.GoBack;
        _abilityUI1.GetComponentInChildren<Text>().text = _creature._ability1.name; // Create function that checks if ability then name else blank ( could be other image better? ) 
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _enemies = SortEntitiesByPosition(_enemies);
        _allies = GameObject.FindGameObjectsWithTag("Ally");
        _allies = SortEntitiesByPosition(_allies);
        _chosenAbility = null;
        //Delete when turns system completed ( The Battle Manager will be the one doing it) 

        _turnState = PlayerTurnState.SelectAction; // Should be Waiting when turn system completed
        AOFAEnable();

    }

    // CHANGE AOFA AND ACTIONS TO BE LESS CONFUSING ??? ( NEW FUNCTIONS WITH CLEARER INTENTIONS )

    #region AOFA functions

    private void Attack(InputAction.CallbackContext context)
    {

        switch (_turnState)
        {
            case PlayerTurnState.SelectAction:
                AOFADisable();
                _abilityMenu.SetActive(true);
                _goBackButton.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_abilityUI1); // Remember to add this when exiting attack menu
                _goBackAction.Enable();
                _goBackAction.performed += EnableAOFA;
                AbilitySelected(1);
                break;
            default:
                break;
        }
        
    } // A

    private void Object(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case PlayerTurnState.SelectAction:
                AOFADisable();
                break;
            default:
                break;
        }
    } // X

    private void Flee(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case PlayerTurnState.SelectAction:
                AOFADisable();
                break;
            default:
                break;
        }
    } // B

    private void Alchemy(InputAction.CallbackContext context)
    {
        switch (_turnState)
        {
            case PlayerTurnState.SelectAction:
                AOFADisable();
                break;
            default:
                break;
        }
    } // Y

    #endregion

    #region Enable and disable AOFA
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
    #endregion

    #region Enable and disable actions
    public void EnableAOFA(InputAction.CallbackContext context)
    {
        _abilityMenu.SetActive(false);
        _goBackButton.SetActive(false);
        _goBackAction.Disable();
        _goBackAction.performed -= EnableAOFA;
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
                break;
            case 2:
                 _description = _creature._ability2.IsUnityNull() ? "" : _creature._ability2._description;
                _abilityDescriptionUI.GetComponentInChildren<Text>().text = _description;
                break;
            case 3:
                 _description = _creature._ability3.IsUnityNull() ? "" : _creature._ability3._description;
                _abilityDescriptionUI.GetComponentInChildren<Text>().text = _description;
                break;
            case 4:
                 _description = _creature._ability4.IsUnityNull() ? "" : _creature._ability4._description;
                _abilityDescriptionUI.GetComponentInChildren<Text>().text = _description;
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
        }
    }


    // When an abilty or object is used, decides what is targeted based on the type
    private void TargetSelectSwitch(Type _type)
    {
        switch (_type)
        {
            case Type.Enemy:
                SelectTarget = _enemies[0];
                _selectCursor.SetActive(true); // make function for this ??? 
                _selectCursor.transform.position = SelectTarget.transform.position + Vector3.up * 2; // CHANGE SO IT KNOWS SPRITE HEIGHT AND GO A LITTLE UP OF IT
                break;

            case Type.AllEnemies:
                break;

            case Type.Ally:
                _selectTarget = _allies[0];
                break;

            case Type.AllAllies:
                break;
        }
    }

    // Method for sorting an array of gameobjects by their x position
    private GameObject[] SortEntitiesByPosition(GameObject[] _entitiesArray)
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
        _lowerPositionArray = SortEntitiesByPosition(_lowerPosition.ToArray());
        GameObject[] _higherPositionArray = _higherPosition.ToArray();
        _higherPositionArray = SortEntitiesByPosition(_higherPosition.ToArray());


        var _returnArray =  new GameObject[_lowerPositionArray.Length + _higherPositionArray.Length + 1];
        _lowerPositionArray.CopyTo(_returnArray, 0);
        _returnArray[_lowerPositionArray.Length] = _pivot;
        _higherPositionArray.CopyTo(_returnArray, _lowerPositionArray.Length + 1);
        return _returnArray;
    }
    
}


