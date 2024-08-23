using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { PLAYERTURN, ENEMYTURN, WIN, LOSE, START }

public class BattleSysten : MonoBehaviour
{
    //ALL THIS SHALL BE CHANGED
    [SerializeField] GameObject _player;
    //[SerializeField] GameObject _enemies;
    //[SerializeField] GameObject _playerPosition;
    //[SerializeField] GameObject _enemyPosition;

    private BattleState _state;
    // Start is called before the first frame update
    void Start()
    {
        _state = BattleState.START;
       // _player.transform.position = _playerPosition.transform.position; Position the allies and players in their respetive positions
        //_enemyPosition.transform.position = _enemyPosition.transform.position;
    }

    public BattleState GetState() { return _state; }
    public void PlayerTurnFinished() { _state = BattleState.PLAYERTURN; }

    public void PlayerLose() { _state = BattleState.LOSE; }

    public void EnemyLose() { _state = BattleState.WIN;  }

    public void EnemyTurnFinished() { _state = BattleState.ENEMYTURN; }

    
}
