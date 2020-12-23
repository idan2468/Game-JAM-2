using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempJewControler : MonoBehaviour
{
    enum State
    {
        Free,
        CaughtByEnemy,
        CaughtByGolem,
        Thrown
    };
    private State _currentState = State.Free;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsFree()
    {
        return _currentState == State.Free;
    }

    public bool IsCaughtByEnemy()
    {
        return _currentState == State.CaughtByEnemy;
    }

    public bool IsCaughtByGolem()
    {
        return _currentState == State.CaughtByGolem;
    }

    public void SetState(string state)
    {
        
    }
}
