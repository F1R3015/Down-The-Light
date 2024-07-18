using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region player inputs
    private PlayerAction _inputActions;
    private InputAction _moveAction;
    #endregion

    #region player components
    private Rigidbody _rb;

    [SerializeField] private float _speedMovement;
    #endregion

    private void Awake()
    {
        _inputActions = new PlayerAction();
        _moveAction = _inputActions.Player.Move;
        _rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 _moveVector = new Vector3(_moveAction.ReadValue<Vector2>().x,0, _moveAction.ReadValue<Vector2>().y);
        Vector3 _playerPosition = _rb.position;
        _playerPosition += _moveVector*_speedMovement*Time.deltaTime;
        _rb.MovePosition(_playerPosition);
    }


    #region Enable and Disable Player Actions
    // Enable Player Actions
    private void OnEnable()
    {
        _moveAction.Enable();
    }

    // Disable Player Actions

    private void OnDisable()
    {
        _moveAction.Disable();
    }

    #endregion

}
