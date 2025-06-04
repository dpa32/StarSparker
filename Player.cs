using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    public Dictionary<string, int> Inventory = new Dictionary<string, int>();

    public int Progress;
    public int SaveMask;

    private CheckpointManager _checkpointManager;
    private Vector3 _dir;
    private Rigidbody _rb;

    [SerializeField]
    private GameObject _cameraArm;

    void Start()
    {
        SaveMask = 1 << LayerMask.NameToLayer("SavePoint");
        _rb = GetComponent<Rigidbody>();
        _checkpointManager = GetComponent<CheckpointManager>();
    }

    public void Movement(float speedMove, Vector2 moveInput)
    {
        _dir = (_cameraArm.transform.right * moveInput.x + _cameraArm.transform.forward * moveInput.y).normalized;
        _dir.y = 0;
        _rb.position += _dir * (speedMove * Time.deltaTime);

        Quaternion viewRot = Quaternion.LookRotation(_dir, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, viewRot, Time.deltaTime * 20);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == SaveMask)
        {
            _checkpointManager.SaveCheckpoint(this);
        }
    }

    public void SaveMemento(PlayerMemento memento)
    {
        transform.position = memento.Position;
        Inventory = new Dictionary<string, int>(memento.Items);
        Progress = memento.Progress;
    }
    public PlayerMemento CreateMemento()
    {
        return new PlayerMemento(transform.position, Inventory, Progress);
    }
}
