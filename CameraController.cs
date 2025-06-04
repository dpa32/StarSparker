using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Target; 
    public GameObject CameraArm;
    public Camera Camera;

    public Vector2 CameraMove;
    public bool InitCamera;

    public float ArmLength = 2f;
    public float SpeedRot = 5f;
    //public float SpeedZoom = 300f;

    private float _xRot;
    private float _yRot;
    // private float _armLengthMin = 1f; 
    // private float _armLengthMax = 3.5f;

    private Quaternion _originRot;
    private float _originArmLength;

    void Start()
    {
        _originRot = CameraArm.transform.localRotation;
        _originArmLength = ArmLength;

        Init();
    }

    void Update()
    {
        Rotate();
        CameraUpdate();
        Move();

        if (InitCamera)
        {
            Init();
        }
    }

    private void Init()
    {
        CameraArm.transform.localPosition = Target.transform.position + new Vector3(0, 1, 0);
        CameraArm.transform.localRotation = _originRot;
        ArmLength = _originArmLength;
        _xRot = 25f;
        _yRot = -2f;
    }

    private void CameraUpdate()
    {

        var ray = new Ray(CameraArm.transform.position, -CameraArm.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, ArmLength))
        {
            Camera.transform.position = hit.point;
        }
        else
        {
            Camera.transform.position = CameraArm.transform.position + (-CameraArm.transform.forward * ArmLength);
        }
    }

    private void Rotate()
    {
        _xRot += -CameraMove.y * SpeedRot * Time.deltaTime;
        _yRot += CameraMove.x * SpeedRot * Time.deltaTime;

        _xRot = Mathf.Clamp(_xRot, -85f, 85f);

        CameraArm.transform.localRotation = Quaternion.Euler(_xRot, _yRot, 0f);
    }

    private void Move()
    {
        CameraArm.transform.position = Target.GetComponent<Rigidbody>().position + new Vector3(0, 1, 0);
    }

    /*
    private void Zoom()
    {
        var zoom = Input.GetAxis("Mouse ScrollWheel") * SpeedZoom * -1f;

        ArmLength = Mathf.Lerp(ArmLength, ArmLength + zoom, Time.deltaTime);
        ArmLength = Mathf.Clamp(ArmLength, _armLengthMin, _armLengthMax);
    }

    
    public void SetTarget(GameObject target)
    {
        Target = target;
        if (target != null)
        {
            CameraArm.transform.SetParent(Target.transform);
            CameraArm.transform.localPosition = Vector3.zero;
            CameraArm.transform.localScale = Vector3.one;
            CameraArm.transform.localRotation = Quaternion.identity;
        }
    }
    */
}