using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation.Editor;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider))]
public class BlinkyBehaviour : MonoBehaviour
{
    private int _maxSpeed = 10;

    NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _homePos;

    private const float _speedIncrement = 0.0208333f;       // (10% - 5% / 240) = 5/240. Or, (maximum allowed speed - starting speed / total pellets)
    // Need a reference to the Total Remaining Pellets
    // Increase total movement speed by 5% when 240 pellets have been collected = 
    // Increment per GameObject (10% - 5% / 240) = 5/240 


    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();  
    }

    void Update()
    {
        _agent.destination = _player.position;

    }

    // TODO - Add to event 
    // TODO - If a Reset level takes place, reset enemy speed
    // Increments agents speed everytime a pellet is collected
    void IncrementAgentSpeed()
    {
        if (_agent.speed < _maxSpeed)
            _agent.speed += _speedIncrement;
        else
        {
            _agent.speed = _maxSpeed;
            return;
        }
    }


    // TODO - Player position doesn't need to be updated 60 times per second, limit this time.
    // Start Coroutine when game starts
    //IEnumerator UpdatePlayerPos()
    //{

    //}
}
