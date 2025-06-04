using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class PlayerRootState : AState
{
    private Dictionary<Type, PlayerRootState> _superStateDic = new();

    public PlayerRootState(PlayerHFSMHandler handler)
        : base(handler)
    {
        _playerHFSMHandler = handler;
        _defaultSubStateType = typeof(MoveState);
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        ChooseSubstate();
    }

    public override void Exit()
    {
        base.Exit();
    }
    protected virtual void ChooseSubstate()
    {
        if (_playerHFSMHandler.CombatInputed)
        {
            _ChangeSubState(typeof(CombatState));
        }
        else if (_playerHFSMHandler.InteractInputed)
        {
            _ChangeSubState(typeof(InteractState));
        }
        else // MoveInputed�� True�̰ų�, �Է��� ����(State == Idle) ��
        {
            _ChangeSubState(typeof(MoveState));
        }
    }

    private void _ChangeSubState(Type type)
    {
        ChangeSubState(type, _superStateDic, this);
    }

    // ���� �ڽ�(SuperStates)�� Ȱ���ϴ� �޼���
    protected virtual void SetSubStateOnEnter(PlayerRootState superState)
    {
        Type stateType = _playerHFSMHandler.GetSubStateOnEnter(superState.GetType());
        if (stateType == null)
        {
            stateType = superState._defaultSubStateType;
        }
        superState._ChangeSubState(stateType);
    }
}