using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyStateMachine
{
    Waiting,SelectMove,DoMove
}

public class EnemyAI : MonoBehaviour
{

    protected CreatureStats _stats;
    [SerializeField] protected EnemyStateMachine _currentState;
    protected BattleSystem _battleSystem;
    protected GameObject[] _enemies; // The allies of the enemy
    protected GameObject[] _allies; // Player
    protected List<GameObject> _targets;
    protected Ability _ability;
    protected WaitingCreatureScript _waitingScript;
    // Start is called before the first frame update
    void Start()
    {
        _stats = GetComponent<CreatureStats>();
        _currentState = EnemyStateMachine.Waiting;
        _battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        _enemies = _battleSystem._enemies;
        _allies = _battleSystem._allies;
        _targets = new List<GameObject>();
        _waitingScript = GetComponent<WaitingCreatureScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTurn()
    {
        StartCoroutine(Turn());
    }

    IEnumerator Turn()
    {
        _currentState = EnemyStateMachine.SelectMove;
        CheckConditions();
        yield return new WaitUntil(() => _currentState == EnemyStateMachine.DoMove);
        foreach(GameObject _target in _targets)
        {
            DoAction(_target);
        }
        _targets.Clear();
        _ability = null;
        _currentState = EnemyStateMachine.Waiting;
        _waitingScript.StartWaiting();
        _battleSystem.EnemyTurnFinished();
        
    }

    protected virtual void CheckConditions() // Decide ability and move
    {
       
    }

    protected void DoAction(GameObject _target)
    {
        CreatureStats _targetCreature = _target.GetComponent<CreatureStats>();
        // Removed mana cost for enemies
        if (_ability._damagePoint != 0)
        {
            _targetCreature.ChangeHealth(-_ability._damagePoint); // If defeated remove from enemies[]
            if (_targetCreature._health <= 0)
            {
                _target.SetActive(false); // Note:  Should do it the gameObject itself
                _battleSystem._turns.Remove(_target);
                _allies = _allies.Where(val => val != _target).ToArray();
                _battleSystem._allies = _allies;
            }
            else
            {
                _target.transform.GetChild(1).gameObject.SetActive(false);
            }
            Debug.Log($"{_targetCreature._health}/{_targetCreature._maximumHealth}");
        }
        if (_ability._healPoint != 0)
        {
            _targetCreature.ChangeHealth(_ability._healPoint);
        }
        if (_ability._accuracyPoint != 0)
        {
            _targetCreature.ChangeAccuracy(_ability._accuracyPoint);
        }
        if (_ability._defencePoint != 0)
        {
            _targetCreature.ChangeDefence(_ability._defencePoint);
        }
        if (_ability._selfDamagePoint != 0)
        {
            _stats.ChangeHealth(_ability._selfDamagePoint);
        }
        if (_ability._manaPoint != 0)
        {
            _targetCreature.ChangeMana(_ability._manaPoint);
        }
        if (_ability._speedPoint != 0)
        {
            _targetCreature.ChangeSpeed(_ability._speedPoint);
        }
        if (_ability._strengthPoint != 0)
        {
            _targetCreature.ChangeStrength(_ability._strengthPoint);
        }
    }
}
