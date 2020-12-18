using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float rayCastDistFactor = 2f;
    [SerializeField] private Transform cam;
    private string HorizontalAxis = "Horizontal";
    private string VerticalAxis = "Vertical";
    private string FrontHand = "FrontHand";
    private string BackHand = "BackHand";
    private string LeftHand = "LeftHand";
    private string RightHand = "RightHand";
    private CharacterController _playerController;

    [SerializeField] private float dist;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var vertical = Input.GetAxis(VerticalAxis);
        var horizontal = Input.GetAxis(HorizontalAxis);
        var dir = PlayerMove(new Vector3(horizontal, 0, vertical));
        var velocity = dir * speed;
        // RotationHandler(dir);
        _playerController.Move(velocity * Time.deltaTime);
    }

    #region rotation with mouse (not working good yet)

    // private void RotationHandler(Vector3 dir)
    // {
    //     RaycastHit hit;
    //     var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     if (Physics.Raycast(ray, out hit))
    //     {
    //         var newDir = new Vector3(hit.point.x,dir.y,dir.z);
    //         Debug.DrawLine(transform.position,newDir);
    //         dist = Vector3.Distance(transform.position, newDir);
    //         if(dist > rayCastDistFactor)
    //         {
    //             transform.LookAt(new Vector3(hit.point.x, dir.y, hit.point.z));
    //         }
    //     }
    //     
    // }

    #endregion

    private Vector3 PlayerMove(Vector3 dir)
    {
        if (dir.magnitude <= .1) return Vector3.zero;
        var forwardDir = cam.gameObject.transform.eulerAngles.y;
        var forwardAccordingToCamera = Quaternion.Euler(0f, forwardDir, 0f) * dir;
        var rotation = Quaternion.LookRotation(forwardAccordingToCamera);
        var step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation,rotation,step);
        return forwardAccordingToCamera;
    }
}