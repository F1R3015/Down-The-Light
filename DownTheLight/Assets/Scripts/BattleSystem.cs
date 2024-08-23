using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { DECIDETURN, PLAYERTURN, ENEMYTURN, WIN, LOSE, START }

public class BattleSystem : MonoBehaviour
{
    //ALL THIS SHALL BE CHANGED
    [SerializeField] GameObject _player;
    //[SerializeField] GameObject _enemies;
    //[SerializeField] GameObject _playerPosition;
    //[SerializeField] GameObject _enemyPosition;

    [SerializeField] private BattleState _state;
    // Start is called before the first frame update
    void Start()
    {
        _state = BattleState.START;
        StartCoroutine(PlayerTurn());
       // _player.transform.position = _playerPosition.transform.position; Position the allies and players in their respetive positions
        //_enemyPosition.transform.position = _enemyPosition.transform.position;
    }

    public BattleState GetState() { return _state; }
    public void PlayerTurnFinished() { _state = BattleState.DECIDETURN; StartCoroutine(PlayerTurn()); }

    public void PlayerLose() { _state = BattleState.LOSE; }

    public void EnemyLose() { _state = BattleState.WIN;  }

    public void EnemyTurnFinished() { _state = BattleState.DECIDETURN; }

    IEnumerator PlayerTurn()
    {
        yield return new WaitForSeconds(3);
        _state = BattleState.PLAYERTURN;
        _player.GetComponent<PlayerBattleController>().StartTurn();
    }
}
