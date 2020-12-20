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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent != _thrower)
        {
            other.gameObject.GetComponent<Collider>().enabled =false;
            _throwerAPI.AddObjToThrow(other.gameObject);    
        }
    }
}