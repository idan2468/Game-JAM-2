using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempJewControler : MonoBehaviour
{
    public ObjectSpawner myObjectSpawner;

    private Vector3 _centerPoint;
    private float _zoneSize = 10f;

    public enum State
    {
        Free,
        CaughtByEnemy,
        CaughtByGolem,
        Thrown
    };
    private State _currentState = State.Free;

    public State CurrentState
    {
        get => _currentState;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentState == State.Free)
        {
            PlayerMove();
        }
    }

    private void PlayerMove()
    {
        // TODO: Add random movement within zone
    }

    // Sets values to default for object pool
    private void OnEnable()
    {
        // TODO: COMPLETE
        _centerPoint = transform.position;
    }

    public void EnterFreeState()
    {
        // TODO: COMPLETE 
        _currentState = State.Free;
    }

    public void EnterTaintState()
    {
        // TODO: COMPLETE 
        _currentState = State.CaughtByEnemy;
    }

    public void EnterGolemState()
    {
        // TODO: COMPLETE 
        _currentState = State.CaughtByGolem;
    }

    public void EnterThrownState()
    {
        // TODO: COMPLETE 
        _currentState = State.Thrown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Synagogue"))
        {
            // Score

            // Back to object pool
            myObjectSpawner.KillJew(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(_centerPoint, new Vector3(_zoneSize, 0f, _zoneSize));
    }
}
