using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    [SerializeField] private float rotationSpeed = 2f;

    // [SerializeField] private float rayCastDistFactor = 2f;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject throwingObj;
    private string HorizontalAxis = "Horizontal";
    private string VerticalAxis = "Vertical";
    private int throwId = Animator.StringToHash("Throw");
    private int speedId = Animator.StringToHash("Speed");
    private CharacterController _playerController;
    private Animator _myAnimator;
    [Header("Animation")] [SerializeField] private bool isUsingAnimator = false;

    [SerializeField] private ThrowerController _throwerController;

    // Start is called before the first frame update
    void Start()
    {
        _throwerController = GetComponentInChildren<ThrowerController>();
        _playerController = GetComponent<CharacterController>();
        if (isUsingAnimator)
        {
            _myAnimator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var vertical = Input.GetAxis(VerticalAxis);
        var horizontal = Input.GetAxis(HorizontalAxis);
        var dir = PlayerMove(new Vector3(horizontal, 0, vertical));
        var velocity = dir * speed;
        if (isUsingAnimator)
        {
            _myAnimator.SetFloat(speedId, velocity.magnitude);
        }

        _playerController.Move(velocity * Time.deltaTime);
    }

    private Vector3 PlayerMove(Vector3 dir)
    {
        if (dir.magnitude <= .1) return Vector3.zero;
        var forwardDir = cam.eulerAngles.y;
        var forwardAccordingToCamera = Quaternion.Euler(0f, forwardDir, 0f) * dir;
        var rotation = Quaternion.LookRotation(forwardAccordingToCamera);
        var step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, step);
        return forwardAccordingToCamera;
    }


    public void ThrowJew()
    {
        _throwerController.Throw();
    }
}