using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingCreatureScript : MonoBehaviour
{

    private float _waitingTime;
    private bool _pauseWaiting;
    private WaitingSystem _waitingSystem; // Is it really necessary? 
    private BattleSystem _battleSystem;
    // Start is called before the first frame update
    void Start()
    {
        ChangeWaitingTime();
       // _waitingSystem = GameObject.FindGameObjectWithTag("WaitingSystem").GetComponent<WaitingSystem>();
        _battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        Pause(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_pauseWaiting)
        {
            _waitingTime -= Time.deltaTime;
            if(_waitingTime <= 0f)
            {
                // Remove from waiting line ( SetActive(false) ) 
                _battleSystem.TurnReady(this.gameObject);
                Pause(true);
            }
        }
        
    }

    public void ChangeWaitingTime()
    {
        _waitingTime = -GetComponent<CreatureStats>()._speed/2 + 10;
    }

    public void UpdateWaitingTime(float _addTime)
    {
        _waitingTime += _addTime; // Waiting Line should automatically update
    }

    public void Pause(bool _value) // Link to event in battle system that pause everything when combat finish
    {
        _pauseWaiting = _value;
        // Waiting Line Pause
    }

   
    public float GetWaitingTime()
    {
        return _waitingTime;
    }

    public void StartWaiting()
    {
        ChangeWaitingTime();
        Pause(false);
        // Would call waiting system to add character in waiting line ( SetActive(true) and position and move ) 
    }

    
}
