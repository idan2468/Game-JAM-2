using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using Singletons;

public class TempJewControler : MonoBehaviour
{
    public ObjectSpawner myObjectSpawner;

    private float _boundsExtent = 10f;
    [SerializeField] private Bounds _movementBounds;

    private Vector3 _currentTargetPosition;
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 2f;

    private Animator _jewAnimator;
    [SerializeField] private bool _isUsingAnimator = false;
    public enum State
    {
        Free = 0,
        CaughtByEnemy = 1,
        CaughtByGolem = 2,
        Thrown = 3
    };
    [SerializeField] private State _currentState = State.Free;
    [SerializeField] private GameObject _chasingEnemy; // Currently chased by this enemy

    public State CurrentState
    {
        get => _currentState;
    }


    // Start is called before the first frame update
    void Start()
    {
        InitializeJew();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentState == State.Free)
        {
            PlayerMove();
        }
        //Debug.Log(_currentState);
    }
    
    private void PlayerMove()
    {
        var angleToTarget = Vector3.Angle(transform.forward, (_currentTargetPosition - transform.position).normalized);
        if (angleToTarget > 0)
        {
            var normDirection = (_currentTargetPosition - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(normDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
        }

        transform.position = Vector3.MoveTowards(transform.position, _currentTargetPosition, _movementSpeed * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, _currentTargetPosition) < 0.05f)
        {
            _currentTargetPosition = GameManager.Instance.RandomPointInBounds(_movementBounds);
        }
    }

    private void SetBounds(Vector3 center, Vector3 extents)
    {
        _movementBounds.center = center;
        _movementBounds.extents = extents;
        // TODO: CLAMP BOUNDS TO FLOOR BOUNDS
    }

    // Sets values to default for object pool
    private void OnEnable()
    {
        // TODO: COMPLETE
        InitializeJew();
    }

    private void InitializeJew()
    {
        _jewAnimator = GetComponent<Animator>();
        SetBounds(transform.position, new Vector3(_boundsExtent, _movementBounds.extents.y, _boundsExtent));
        _currentTargetPosition = GameManager.Instance.RandomPointInBounds(_movementBounds);
        EnterFreeState();
    }

    public void SetChase(GameObject enemy)
    {
        _chasingEnemy = enemy;
    }

    public GameObject GetChase()
    {
        return _chasingEnemy;
    }

    public void EnterFreeState()
    {
        // TODO: COMPLETE 
        _currentState = State.Free;
        SetChase(null);
        if (_isUsingAnimator) _jewAnimator.SetInteger("State", (int)State.Free);
    }

    public void EnterTaintState(GameObject enemy)
    {
        // TODO: COMPLETE 
        _currentState = State.CaughtByEnemy;
        SetChase(null);

        // Turn Jew to face enemy
        var normDirection = (enemy.transform.position - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(normDirection);
        transform.localRotation = lookRotation;

        if (_isUsingAnimator) _jewAnimator.SetInteger("State", (int) State.CaughtByEnemy);
    }

    public void EnterGolemState()
    {
        // TODO: COMPLETE 
        _currentState = State.CaughtByGolem;
        if (_isUsingAnimator) _jewAnimator.SetInteger("State", (int) State.CaughtByGolem);
    }

    public void EnterThrownState()
    {
        // TODO: COMPLETE 
        _currentState = State.Thrown;
        SetChase(null);
        if (_isUsingAnimator) _jewAnimator.SetInteger("State", (int) State.Thrown);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Synagogue"))
        {
            // TODO: SCORE

            // Back to object pool
            GameManager.Instance.KillJew(gameObject);
        }

        // Hit floor after missed throw
        if (other.tag.Equals("Floor") && _currentState == State.Thrown)
        {
            EnterFreeState();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(_movementBounds.center, _movementBounds.size);
    }
}
