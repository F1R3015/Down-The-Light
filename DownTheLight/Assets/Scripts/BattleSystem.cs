using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleState { DECIDETURN, PLAYERTURN, ENEMYTURN, WIN, LOSE, START }

public class BattleSystem : MonoBehaviour
{
    //ALL THIS SHALL BE CHANGED
    [SerializeField] public GameObject[] _allies;
    [SerializeField] public GameObject[] _enemies;
    [SerializeField] public List<GameObject> _turns;
    //[SerializeField] GameObject _playerPosition;
    //[SerializeField] GameObject _enemyPosition;

    [SerializeField] private BattleState _state;
    // Start is called before the first frame update

    private void Awake()
    {
        _allies = GameObject.FindGameObjectsWithTag("Ally");
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        _turns = SortEntitiesBySpeed(_allies.Concat(_enemies).ToArray()).ToList();
    }

    void Start()
    {
        _state = BattleState.START;
        StartCoroutine(PlayerTurn()); // Note: Should be the first in _turns
       // _player.transform.position = _playerPosition.transform.position; Position the allies and players in their respetive positions
        //_enemyPosition.transform.position = _enemyPosition.transform.position;
    }

    public BattleState GetState() { return _state; }
    public void PlayerTurnFinished() 
    {   
        if(_enemies.Length == 0)
        {
            _state = BattleState.WIN;
            Debug.Log("WINWINWIN");// Note: Rewards + Go back to dungeon
        }
        else 
        {
            _turns.Remove(_turns.First());
            _state = BattleState.DECIDETURN;
            StartCoroutine(DecidesTurn());
        }

    } 

    public void PlayerLose() { _state = BattleState.LOSE; }

    public void EnemyLose() { _state = BattleState.WIN;  }

    public void EnemyTurnFinished() { _turns.Remove(_turns.First()); _state = BattleState.DECIDETURN; StartCoroutine(DecidesTurn()); }

    IEnumerator PlayerTurn()
    {
        yield return new WaitForSeconds(3);
        _state = BattleState.PLAYERTURN;
        GetComponent<PlayerBattleController>().StartTurn(_turns.First().GetComponent<CreatureStats>());
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(3);
        _state = BattleState.ENEMYTURN;
        Debug.Log("ENEMY ATTAKCS WAAA"); // Note: It should call the enemy AI
        EnemyTurnFinished();// Note: Delete when enemy AI is done
    }

    IEnumerator DecidesTurn()
    {
        yield return new WaitUntil(() => _turns.Count > 0);
        if (_allies.Contains(_turns.First())) // It should do an animation for player or enemy
        {
            StartCoroutine(PlayerTurn());
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }
    // Called by a creature when they are ready
    public void TurnReady(GameObject _creature)
    {
        _turns.Add(_creature);
    }

    private GameObject[] SortEntitiesBySpeed(GameObject[] _entitiesArray)
    {
        if (_entitiesArray.Length <= 1)
        {
            return _entitiesArray;
        }
        GameObject _pivot = _entitiesArray[0];
        List<GameObject> _lowerPosition = new List<GameObject>();
        List<GameObject> _higherPosition = new List<GameObject>();
        for (int i = 1; i < _entitiesArray.Length; i++)
        {
            if (_pivot.GetComponent<CreatureStats>()._speed <= _entitiesArray[i].GetComponent<CreatureStats>()._speed)
            {
                _lowerPosition.Add(_entitiesArray[i]);
            }

            if (_pivot.GetComponent<CreatureStats>()._speed > _entitiesArray[i].GetComponent<CreatureStats>()._speed)
            {
                _higherPosition.Add(_entitiesArray[i]);
            }
        }

        GameObject[] _lowerPositionArray = _lowerPosition.ToArray();
        _lowerPositionArray = SortEntitiesBySpeed(_lowerPosition.ToArray());
        GameObject[] _higherPositionArray = _higherPosition.ToArray();
        _higherPositionArray = SortEntitiesBySpeed(_higherPosition.ToArray());


        var _returnArray = new GameObject[_lowerPositionArray.Length + _higherPositionArray.Length + 1];
        _lowerPositionArray.CopyTo(_returnArray, 0);
        _returnArray[_lowerPositionArray.Length] = _pivot;
        _higherPositionArray.CopyTo(_returnArray, _lowerPositionArray.Length + 1);
        return _returnArray;
    }
}
