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
    void Update()
    {
    }
    //#todo Check if the golem is empty handed before adding the obj (here or just in addObjToThrow
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Jew") && _throwerAPI.CanCatchJew)
        {
            _throwerAPI.AddObjToThrow(other.gameObject);
            other.gameObject.GetComponent<JewController>().EnterGolemState();
        }
    }
}