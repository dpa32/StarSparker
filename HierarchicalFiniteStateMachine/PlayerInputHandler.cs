using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerHFSMHandler _playerHFSMHandler;
    private CameraController _cameraController;

    [SerializeField]
    private GameObject _cameraArm;

    private enum _actionMaps
    {
        InGame_Global,
        // 이하 SuperStateMap
        Move = 101,
        Combat,
        Interact
    }
    private enum _actions
    {
        Look,
        ResetView,
        Run,
        Skill,
        Pause,
        Move,
        Combat,
        Interact,
        Walk,
        Jump,
        Attack,
        Target,
        Dash,
        NormalInteract,
        Eat
    }

    private Dictionary<_actionMaps, InputActionMap> _actionMapsDic = new();
    private Dictionary<_actions, InputAction> _actionsDic = new();

    private void Awake()
    {
        _playerHFSMHandler = GetComponent<PlayerHFSMHandler>();
        _playerInput = GetComponent<PlayerInput>();
        _cameraController = _cameraArm.GetComponent<CameraController>();
        InitInputActions();
    }
    private void Start()
    {
    }
    private void Update()
    {
        _playerHFSMHandler.MoveMapEnabled = _actionMapsDic[_actionMaps.Move].enabled;
        _playerHFSMHandler.CombatMapEnabled = _actionMapsDic[_actionMaps.Combat].enabled;
        _playerHFSMHandler.InteractMapEnabled = _actionMapsDic[_actionMaps.Interact].enabled;
    }

    private void OnEnable()
    {
        _actionMapsDic[_actionMaps.InGame_Global].Enable();

        //Global Input
        // Camera 관련 Input
        _actionsDic[_actions.Look].performed += LookInputed;
        _actionsDic[_actions.Look].canceled += LookInputed;
        _actionsDic[_actions.ResetView].started += ResetViewInputed;
        _actionsDic[_actions.ResetView].canceled += ResetViewInputed;
        // bool값 설정 처리
        _actionsDic[_actions.Run].performed += RunInputed;
        _actionsDic[_actions.Run].canceled += RunInputed;
        _actionsDic[_actions.Skill].performed += SkillInputed;
        _actionsDic[_actions.Skill].canceled += SkillInputed;
        _actionsDic[_actions.Pause].performed += PauseInputed;
        _actionsDic[_actions.Pause].canceled += PauseInputed;
        //SuperState 설정 처리
        _actionsDic[_actions.Move].performed += MoveInputed;
        _actionsDic[_actions.Move].canceled += MoveInputed;
        _actionsDic[_actions.Combat].performed += CombatInputed;
        _actionsDic[_actions.Combat].canceled += CombatInputed;
        _actionsDic[_actions.Interact].performed += InteractInputed;
        _actionsDic[_actions.Interact].canceled += InteractInputed;

        // Move Input
        _actionsDic[_actions.Walk].performed += WalkInputed;
        _actionsDic[_actions.Walk].canceled += WalkInputed;
        _actionsDic[_actions.Jump].started += JumpInputed;
        _actionsDic[_actions.Jump].canceled += JumpInputed;

        //Combat Input
        _actionsDic[_actions.Attack].started += AttackInputed;
        _actionsDic[_actions.Attack].canceled += AttackInputed;
        _actionsDic[_actions.Target].performed += TargetInputed;
        _actionsDic[_actions.Target].canceled += TargetInputed;
        _actionsDic[_actions.Dash].started += DashInputed;
        _actionsDic[_actions.Dash].canceled += DashInputed;

        //Interact Input
        _actionsDic[_actions.NormalInteract].started += NormalInteractInputed;
        _actionsDic[_actions.NormalInteract].canceled += NormalInteractInputed;
        _actionsDic[_actions.Eat].started += EatInputed;
        _actionsDic[_actions.Eat].canceled += EatInputed;
    }

    private void OnDisable()
    {
        _actionMapsDic[_actionMaps.InGame_Global].Disable();
        DisableAllStateMaps();

        //Global Input
        // Camera 관련 Input
        _actionsDic[_actions.Look].performed -= LookInputed;
        _actionsDic[_actions.Look].canceled -= LookInputed;
        _actionsDic[_actions.ResetView].started -= ResetViewInputed;
        _actionsDic[_actions.ResetView].canceled -= ResetViewInputed;
        // bool값 설정 처리
        _actionsDic[_actions.Run].performed -= RunInputed;
        _actionsDic[_actions.Run].canceled -= RunInputed;
        _actionsDic[_actions.Skill].performed -= SkillInputed;
        _actionsDic[_actions.Skill].canceled -= SkillInputed;
        _actionsDic[_actions.Pause].performed -= PauseInputed;
        _actionsDic[_actions.Pause].canceled -= PauseInputed;
        //SuperState 설정 처리
        _actionsDic[_actions.Move].performed -= MoveInputed;
        _actionsDic[_actions.Move].canceled -= MoveInputed;
        _actionsDic[_actions.Combat].performed -= CombatInputed;
        _actionsDic[_actions.Combat].canceled -= CombatInputed;
        _actionsDic[_actions.Interact].performed -= InteractInputed;
        _actionsDic[_actions.Interact].canceled -= InteractInputed;

        // Move Input
        _actionsDic[_actions.Walk].performed -= WalkInputed;
        _actionsDic[_actions.Walk].canceled -= WalkInputed;
        _actionsDic[_actions.Jump].started -= JumpInputed;
        _actionsDic[_actions.Jump].canceled -= JumpInputed;

        //Combat Input
        _actionsDic[_actions.Attack].started -= AttackInputed;
        _actionsDic[_actions.Attack].canceled -= AttackInputed;
        _actionsDic[_actions.Target].performed -= TargetInputed;
        _actionsDic[_actions.Target].canceled -= TargetInputed;
        _actionsDic[_actions.Dash].started -= DashInputed;
        _actionsDic[_actions.Dash].canceled -= DashInputed;

        //Interact Input
        _actionsDic[_actions.NormalInteract].started -= NormalInteractInputed;
        _actionsDic[_actions.NormalInteract].canceled -= NormalInteractInputed;
        _actionsDic[_actions.Eat].started -= EatInputed;
        _actionsDic[_actions.Eat].canceled -= EatInputed;
    }
    public void EnableStateMap(String typeName)
    {
        DisableAllStateMaps();
        if(Enum.TryParse(typeName, out _actionMaps mapEnum))
        {
            _actionMapsDic[mapEnum].Enable();
        }
    }
    public InputActionMap ReturnActionMap(string actionMapName)
    {
        foreach(var map in _actionMapsDic)
        {
            if(map.Key.ToString() == actionMapName)
            {
                return map.Value;
            }
        }
        return null;
    }

    private void InitInputActions()
    {
        foreach (var map in _playerInput.actions.actionMaps)
        {
            if (Enum.TryParse(map.name, out _actionMaps mapEnum))
            {
                _actionMapsDic.Add(mapEnum, map);
            }
            foreach (var action in map.actions)
            {
                if (Enum.TryParse(action.name, out _actions actionEnum))
                {
                    _actionsDic.Add(actionEnum, action);
                }
            }
        }
    }
    private void DisableAllStateMaps()
    {
        foreach (var map in _actionMapsDic)
        {
            if ((int)map.Key > 100)
            {
                map.Value.Disable();
            }
        }
    }

    private void LookInputed(InputAction.CallbackContext context)
    {
        _cameraController.CameraMove = context.ReadValue<Vector2>();
    }
    private void ResetViewInputed(InputAction.CallbackContext context)
    {
        _cameraController.InitCamera = context.started;
    }

    // bool
    private void RunInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.RunTriggered = context.performed;
    }
    private void SkillInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.SkillTriggered = context.performed;
    }
    private void PauseInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.PauseTriggered = context.performed;
    }

    // SuperState 
    private void MoveInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.MoveInputed = context.performed;
        _playerHFSMHandler.SetInputContextPathOnSuperStateEnter(context.control.path, _actionMapsDic[_actionMaps.Move]);
    }
    private void CombatInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.CombatInputed = context.performed;
        _playerHFSMHandler.SetInputContextPathOnSuperStateEnter(context.control.path, _actionMapsDic[_actionMaps.Combat]);
    }
    private void InteractInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.InteractInputed = context.performed;
        _playerHFSMHandler.SetInputContextPathOnSuperStateEnter(context.control.path, _actionMapsDic[_actionMaps.Interact]);
    }

    // SubState
    private void WalkInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.MoveInput = context.ReadValue<Vector2>();
        _playerHFSMHandler.WalkInputed = context.performed;
    }
    private void JumpInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.JumpInputed = context.started;
    }
    private void TargetInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.TargetInputed = context.performed;
    }
    private void AttackInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.AttackInputed = context.started;
    }
    private void DashInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.DashInputed = context.started;
    }
    private void NormalInteractInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.NormalInteractInputed = context.started;
    }
    private void EatInputed(InputAction.CallbackContext context)
    {
        _playerHFSMHandler.EatInputed = context.started;
    }
}