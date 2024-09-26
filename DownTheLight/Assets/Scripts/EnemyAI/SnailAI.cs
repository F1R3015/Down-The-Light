using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnailAI : EnemyAI
{
    protected override void CheckConditions()
    {
        GameObject _lowestHealth = FindLowestHealthEnemy();
        CreatureStats _lowestHealthStats = _lowestHealth.GetComponent<CreatureStats>();
        if (_lowestHealthStats._health <= _lowestHealthStats._maximumHealth * 0.2f)
        {
            _ability = _stats._ability1;
            _targets.Add(_lowestHealth);
            Debug.Log("FIREBALL");
            base._currentState = EnemyStateMachine.DoMove;
        }
        else if (_lowestHealthStats._defence >= _lowestHealthStats._baseDefence && _lowestHealthStats._defence > 0) 
        {
            _ability = _stats._ability2;
            _targets.Add(_lowestHealth.gameObject);
            Debug.Log("MUDBALL");
            base._currentState = EnemyStateMachine.DoMove;
        }
        else
        {
            _ability = _stats._ability1;
            _targets.Add(_allies[Random.Range(0,_allies.Length)]);
            Debug.Log("FIREBALL");
            base._currentState = EnemyStateMachine.DoMove;
        }
    }

    private GameObject FindLowestHealthEnemy()
    {
        GameObject _lowestEnemy = null;
        foreach(GameObject _enemy in base._allies)
        {
            _lowestEnemy = _lowestEnemy.IsUnityNull() || _enemy.GetComponent<CreatureStats>()._health < _lowestEnemy.GetComponent<CreatureStats>()._health ? _enemy : _lowestEnemy;
        }
        
        return _lowestEnemy;
    }
}
