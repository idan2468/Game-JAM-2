using System;
using System.Linq;
using UnityEngine;

public class Catcher : MonoBehaviour
{
    [SerializeField] private GameObject _thrower;
    private ThrowerController _throwerAPI;

    // Start is called before the first frame update
    void Start()
    {
        _throwerAPI = _thrower.GetComponent<ThrowerController>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Jew") && _throwerAPI.CanCatchJew)
        {
            var jewController = other.gameObject.GetComponent<JewController>();
            if(jewController.CurrentState == JewController.State.Free || jewController.CurrentState == JewController.State.CaughtByEnemy)
            {
                _throwerAPI.AddObjToThrow(other.gameObject);
                other.gameObject.GetComponent<JewController>().EnterGolemState();
            }
        }
    }
}