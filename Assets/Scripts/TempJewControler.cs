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
        // TODO: REMOVE DOTWEEN

        var angleToTarget = Vector3.Angle(transform.forward, _currentTargetPosition - transform.position);
        if (angleToTarget > 0)
        {
            transform.DORotate(_currentTargetPosition, angleToTarget / _rotationSpeed);
        }
        
        transform.DOMove(_currentTargetPosition, Vector3.Distance(_currentTargetPosition, transform.position) / _movementSpeed);
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
        SetBounds(transform.position, new Vector3(_boundsExtent, _movementBounds.extents.y, _boundsExtent));
        _currentTargetPosition =GameManager.Instance.RandomPointInBounds(_movementBounds);
    }

    public void EnterFreeState()
    {
        // TODO: COMPLETE 
        _currentState = State.Free;
        _jewAnimator.SetInteger("State", (int) State.Free);
    }

    public void EnterTaintState()
    {
        // TODO: COMPLETE 
        _currentState = State.CaughtByEnemy;
        _jewAnimator.SetInteger("State", (int) State.CaughtByEnemy);
    }

    public void EnterGolemState()
    {
        // TODO: COMPLETE 
        _currentState = State.CaughtByGolem;
        _jewAnimator.SetInteger("State", (int) State.CaughtByGolem);
    }

    public void EnterThrownState()
    {
        // TODO: COMPLETE 
        _currentState = State.Thrown;
        _jewAnimator.SetInteger("State", (int) State.Thrown);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Synagogue"))
        {
            // TODO: SCORE

            // Back to object pool
<<<<<<< Updated upstream
            GameManager.Instance.KillJew(gameObject);
=======
            ObjectSpawner.Instance.KillJew(gameObject);
            // TODO: CHECK BACK WHEN MANAGER READY
>>>>>>> Stashed changes
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
