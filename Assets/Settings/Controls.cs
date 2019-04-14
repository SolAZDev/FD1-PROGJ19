// GENERATED AUTOMATICALLY FROM 'Assets/Settings/Controls.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class Controls : InputActionAssetReference
{
    public Controls()
    {
    }
    public Controls(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Default
        m_Default = asset.GetActionMap("Default");
        m_Default_Movement = m_Default.GetAction("Movement");
        m_Default_Jump = m_Default.GetAction("Jump");
        m_Default_SwitchLeft = m_Default.GetAction("Switch Left");
        m_Default_SwitchRight = m_Default.GetAction("Switch Right");
        m_Default_Attack = m_Default.GetAction("Attack");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Default = null;
        m_Default_Movement = null;
        m_Default_Jump = null;
        m_Default_SwitchLeft = null;
        m_Default_SwitchRight = null;
        m_Default_Attack = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Default
    private InputActionMap m_Default;
    private InputAction m_Default_Movement;
    private InputAction m_Default_Jump;
    private InputAction m_Default_SwitchLeft;
    private InputAction m_Default_SwitchRight;
    private InputAction m_Default_Attack;
    public struct DefaultActions
    {
        private Controls m_Wrapper;
        public DefaultActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement { get { return m_Wrapper.m_Default_Movement; } }
        public InputAction @Jump { get { return m_Wrapper.m_Default_Jump; } }
        public InputAction @SwitchLeft { get { return m_Wrapper.m_Default_SwitchLeft; } }
        public InputAction @SwitchRight { get { return m_Wrapper.m_Default_SwitchRight; } }
        public InputAction @Attack { get { return m_Wrapper.m_Default_Attack; } }
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
    }
    public DefaultActions @Default
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new DefaultActions(this);
        }
    }
    private int m_PCKeyboardSchemeIndex = -1;
    public InputControlScheme PCKeyboardScheme
    {
        get

        {
            if (m_PCKeyboardSchemeIndex == -1) m_PCKeyboardSchemeIndex = asset.GetControlSchemeIndex("PC Keyboard");
            return asset.controlSchemes[m_PCKeyboardSchemeIndex];
        }
    }
}
