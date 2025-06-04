using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AState
{
    protected PlayerHFSMHandler _playerHFSMHandler;
    protected AState _currentSubState;
    protected AState _defaultSubState;
    protected Type _defaultSubStateType;

    public AState(PlayerHFSMHandler handler) 
    {
        _playerHFSMHandler = handler;
    }

    public virtual void Enter()
    {
        if (_defaultSubState != null && _currentSubState == null)
        {
            _currentSubState = _defaultSubState;
        }
        _currentSubState?.Enter();
    }

    public virtual void Update()
    {
        _currentSubState?.Update();
    }

    public virtual void Exit() 
    {
        _currentSubState?.Exit();
    }
   
    protected virtual void ChangeSubState<T>(Type subStateType, Dictionary<Type, T> subStateDic, T superState) where T : AState
    {
        // Dictionary ��ȸ
        if (!subStateDic.TryGetValue(subStateType, out var newSubState)) // ��ϵ� State ����
        {
            newSubState = (T)Activator.CreateInstance(subStateType, new object[] { _playerHFSMHandler });
            if (subStateType == superState._defaultSubStateType)
            {
                superState._defaultSubState = newSubState;
            }
            subStateDic.Add(subStateType, newSubState);
        }
        if (superState._currentSubState == newSubState) // State ��ȭ ����
        {
            return;
        }

        // SuperState�� ActionMap Ȱ��ȭ
        _playerHFSMHandler.EnableStateMap(typeof(T));

        superState._currentSubState?.Exit();
        superState._currentSubState = newSubState;
        superState._currentSubState.Enter();


        // �� state �����
        _playerHFSMHandler.CurrentPlayerSuperState = typeof(T);
        _playerHFSMHandler.CurrentPlayerSubState = newSubState.GetType();

        Debug.Log($"{_playerHFSMHandler.CurrentPlayerSubState.Name} : {_playerHFSMHandler.CurrentPlayerSuperState.Name}");
    }
}
