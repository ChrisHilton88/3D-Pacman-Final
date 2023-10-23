using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlinky : EnemyBase
{
    [SerializeField] private Transform[] _blinkyScatterPositions;


    void Awake()
    {
        _scatterPositions = _blinkyScatterPositions;
    }

    protected override void Start()
    {
        // This doesn't need to be here if Blinky doesn't have any additional initialisation that needs to be done. Can be removed at the end if this is the case.
        base.Start();

    }

    protected override void CheckState()        
    {
        switch (_currentState)
        {
            case EnemyState.Scatter:
                if (_agent.hasPath)
                {
                    Debug.DrawLine(transform.position, _scatterPositions[CurrentPosition].position, Color.red);

                    if (_agent.remainingDistance < 1.5f)
                    {
                        CalculateNextDestination();
                        // The base class method cannot be set to private otherwise we won't see this. 
                        // At the same time, I don't want this class to be able to make any changes to the base method, affecting other classes that derive the base class.
                    }
                }
                break;

            case EnemyState.Chase:
                _agent.destination = _pacmanTargetPos.position;       // Needs to be continually updating for Player position
                Debug.DrawLine(transform.position, _pacmanTargetPos.position, Color.red);       // The line is correct when changing states
                break;

            case EnemyState.Frightened:
                if (_agent.remainingDistance < 1.5f)
                {
                    GenerateRandomFrightenedPosition();
                }

                Debug.DrawLine(transform.position, _agent.destination, Color.red);
                break;

            default:
                _agent.isStopped = true;
                break;
        }
    }
}
