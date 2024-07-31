using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBattleController : MonoBehaviour
{
    #region Input Actions
    PlayerAction _inputAction;
    InputAction _attackAction;
    InputAction _objectAction;
    InputAction _fleeAction;
    InputAction _goBackAction;
    #endregion

    #region Serialized Propierties
    [SerializeField]
    private GameObject _attackButton;

    [SerializeField] 
    private GameObject _objectButton;

    [SerializeField]
    private GameObject _fleeButton;

    [SerializeField]
    private GameObject _abilityMenu;

    [SerializeField]
    private Creature _creature;

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


    #endregion

    private void Awake()
    {
        _inputAction = new PlayerAction();
        _attackAction = _inputAction.Player.Attack;
        _objectAction = _inputAction.Player.Object;
        _fleeAction = _inputAction.Player.Flee;
        _goBackAction = _inputAction.Player.GoBack;
        _abilityUI1.GetComponentInChildren<Text>().text = _creature._ability1.name;

        //Delete when turns system completed

        AOFEnable();

    }

    #region AOF functions

    private void Attack(InputAction.CallbackContext context)
    {
        
        AOFDisable();
        _abilityMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_abilityUI1); // Remember to add this when exiting attack menu
        _goBackAction.Enable();
        _goBackAction.performed += EnableAOF;
        
    }

    private void Object(InputAction.CallbackContext context)
    {
        AOFDisable();
    }

    private void Flee(InputAction.CallbackContext context)
    {
        AOFDisable();
    }
    #endregion

    #region Enable and disable AOF
    private void AOFEnable()
    {
        _attackButton.SetActive(true);
        _objectButton.SetActive(true);
        _fleeButton.SetActive(true);
        _attackAction.Enable();
        _attackAction.performed += Attack;

        _objectAction.Enable();
        _objectAction.performed += Object;

        _fleeAction.Enable();
        _fleeAction.performed += Flee;
    }

    private void AOFDisable()
    {
        // AOF UI DISABLE
        _attackButton.SetActive(false);
        _objectButton.SetActive(false);
        _fleeButton.SetActive(false);

        // AOF ACTIONS DISABLE
        _attackAction.Disable();
        _attackAction.performed -= Attack;

        _objectAction.Disable();
        _objectAction.performed -= Object;

        _fleeAction.Disable();
        _fleeAction.performed -= Flee;
    }
    #endregion

    #region Enable and disable actions
    public void EnableAOF(InputAction.CallbackContext context)
    {
        _abilityMenu.SetActive(false);
        _goBackAction.Disable();
        _goBackAction.performed -= EnableAOF;
        AOFEnable();
        
    }

    public void DisableAOF(InputAction.CallbackContext context)
    {
        AOFDisable();
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

    
}
