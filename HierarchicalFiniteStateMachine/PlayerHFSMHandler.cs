using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;
using System;
using System.Linq;
using UnityEditorInternal;
using Unity.VisualScripting;

public class PlayerHFSMHandler : MonoBehaviour
{
    // 현 State 추적
    public Type CurrentPlayerSuperState;
    public Type CurrentPlayerSubState;

    public Player PlayerController;
    public Animator Animator;
    public Vector2 MoveInput;

    public bool MoveInputed;
    public bool CombatInputed;
    public bool InteractInputed;

    public bool MoveMapEnabled;
    public bool CombatMapEnabled;
    public bool InteractMapEnabled;

    public bool WalkInputed;
    public bool JumpInputed;

    public bool TargetInputed;
    public bool AttackInputed;
    public bool DashInputed;

    public bool NormalInteractInputed;
    public bool EatInputed;

    // State를 결정하는 조건이 되는 bool값 (e.g. RunTriggered에 따라 WalkState 또는 RunState로 결정)
    public bool RunTriggered;
    public bool SkillTriggered;
    public bool PauseTriggered;

    public bool IsJumping;

    private PlayerInputHandler _playerInputHandler;
    private PlayerRootState _rootState;
    private string _inputBindingpath;

    private Dictionary<Type, Dictionary<string, Type>> _bindingToSubStateDic_CachedBySuperState = new();

    void Start()
    {
        PlayerController = GetComponent<Player>();
        Animator = GetComponent<Animator>();

        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _rootState = new PlayerRootState(this);

        MoveInput = Vector2.zero;
        MoveInputed = false;
        CombatInputed = false;
        InteractInputed = false;
        IsJumping = false;

        CacheStateDic();
    }

    void FixedUpdate()
    {
        _rootState.Update();
    }
    public void EnableStateMap(Type stateType)
    {
        var typeName = stateType.Name.Replace("State", "");
        _playerInputHandler.EnableStateMap(typeName);
    }
    public InputActionMap ReturnActionMap <T>()
    {
        var actionMapName = typeof(T).Name.Replace("State", "");
        return _playerInputHandler.ReturnActionMap(actionMapName);
    }
    public void SetInputContextPathOnSuperStateEnter(string contextPath, InputActionMap map)
    {
        if (!map.enabled)
        {
            _inputBindingpath = contextPath;
        }
    }
    public Type GetSubStateOnEnter(Type superState)
    {
        if (_inputBindingpath == null) 
        {
            return null;
        }
        return _bindingToSubStateDic_CachedBySuperState[superState].GetValueOrDefault(_inputBindingpath);
    }

    // 캐시 생성
    private void CacheBindingToActionDic<T>(Dictionary<string, string> bindingToActionDic)
    {
        InputActionMap actionMap = ReturnActionMap<T>();
        if (bindingToActionDic.Count > 0) return;

        foreach (var action in actionMap.actions)
        {
            foreach (var binding in action.bindings)
            {
                if (!binding.isComposite)
                {
                    bindingToActionDic.TryAdd(binding.path, action.name);
                }
            }
        }
    }
    private void CacheActionToStateDic<T>(Dictionary<string, Type> actionToStateDic)
    {
        if (actionToStateDic.Count > 0) return;
        var stateTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract);
        foreach (var stateType in stateTypes)
        {
            var attributes = stateType.GetCustomAttributes<ActionNameAttribute>().FirstOrDefault();
            if (attributes != null)
            {
                actionToStateDic.Add(attributes.ActionName, stateType);
            }
        }
    }

    private void CacheStateDic()
    {
        var baseType = typeof(PlayerRootState);
        var SuperStates = Assembly.GetAssembly(baseType).GetTypes().Where(t => t.BaseType == baseType && !t.IsAbstract);
        
        MethodInfo bindingToAction_method = typeof(PlayerHFSMHandler)
            .GetMethod(nameof(CacheBindingToActionDic), BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo actionToState_method = typeof(PlayerHFSMHandler)
            .GetMethod(nameof(CacheActionToStateDic), BindingFlags.Instance | BindingFlags.NonPublic);

        // 용도가 다른 중복 Binding 처리를 위해 SuperState별로 관리 (ex: LeftClick-Attack, LeftClick-Interact)
        foreach (var superState in SuperStates) 
        {
            // InputBinding->ActionName 
            Dictionary<string, string> bindingToActionDic = new();
            MethodInfo bindingToAction_genericMethod = bindingToAction_method.MakeGenericMethod(superState);
            bindingToAction_genericMethod.Invoke(this, new object[] { bindingToActionDic });

            // ActionName->StateClass 
            Dictionary<string, Type> actionToStateDic = new();
            MethodInfo actionToState_genericMethod = actionToState_method.MakeGenericMethod(superState);
            actionToState_genericMethod.Invoke(this, new object[] { actionToStateDic });

            _bindingToSubStateDic_CachedBySuperState[superState] = bindingToActionDic
                .ToDictionary(binding => binding.Key, 
                binding => actionToStateDic.ContainsKey(binding.Value) ? actionToStateDic[binding.Value] : null);
        }
    }
}
