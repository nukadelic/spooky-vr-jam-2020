using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using DeviceType = UnityEngine.XR.InputDeviceCharacteristics;
using Valve.VR;
using System.Linq;
using System.Collections;

public class XRInputs : MonoBehaviour
{
    // -------------------------------------------------------
    
    #region Static Hook

    public static XRInputs instance;
    
    static void Hook_Static( XRInputs _instance )
    {
        if( instance != null ) 
        {   // prevent duplicates ( not critical ) 
            Destroy( _instance.gameObject );
            return;
        }

        instance = _instance;
    }

    #endregion

    // -------------------------------------------------------

    #region MonoBehaviour methods
        

    void OnEnable( )
    {
        Hook_Static( this );

        ForceRefresh();

        Application.onBeforeRender += Update;
    }

    private void OnDisable( )
    {
        Application.onBeforeRender -= Update;
    }

    void Update()
    {    
        Scan_HMD();
        Scan_LeftController();
        Scan_RightController();

        UpdateUnifiedInputs();
    }

    void UpdateUnifiedInputs()
    {
        bool steamVR_isValid = VRInputs.instance && VRInputs.instance.steamVR_captured;
        
        #region Left

        if( xr_leftController_readInput )
        {
            // read [ Left ] XR
            
            leftController_tracking = xr_leftController_tracking;
        
            //leftController_joystickClick = xr_leftController_joystickClick; // Obsolete
            //leftController_joystickTouch = xr_leftController_joystickTouch; // Obsolete

            leftController_primaryButton = xr_leftController_primaryButton;
            leftController_secondaryButton = xr_leftController_secondaryButton;
        
            leftController_position = xr_leftController_position;
            leftController_rotation = xr_leftController_rotation;
        
            leftController_joystick = xr_leftController_joystick;
        
            leftController_triggerIsDown = xr_leftController_triggerIsDown;
            leftController_triggerValue = xr_leftController_triggerValue;

            leftController_gripValue = xr_leftController_gripValue;
            leftController_gripIsDown = xr_leftController_gripIsDown;
        }
        else if( steamVR_isValid )
        {
            // read [ Left ] SteamVR

            leftController_tracking = true;

            leftController_joystick = VRInputs.instance.leftAxis;

            leftController_gripValue = VRInputs.instance.leftGrip;
            leftController_gripIsDown = leftController_gripValue > 0.5f;

            leftController_triggerValue = VRInputs.instance.leftTrigger;
            leftController_triggerIsDown = leftController_triggerValue > 0.5f;
            
            leftController_primaryButton = VRInputs.instance.leftPrimaryButtonDown;
            leftController_secondaryButton = VRInputs.instance.leftSecondaryButtonDown;
        }

        #endregion

        #region Right

        if( xr_rightController_readInput )
        {
            // read [ Right ] XR

            rightController_tracking = xr_rightController_tracking;
        
            //rightController_joystickClick = xr_rightController_joystickClick; // Obsolete
            //rightController_joystickTouch = xr_rightController_joystickTouch; // Obsolete

            rightController_primaryButton = xr_rightController_primaryButton;
            rightController_secondaryButton = xr_rightController_secondaryButton;
        
            rightController_position = xr_rightController_position;
            rightController_rotation = xr_rightController_rotation;
        
            rightController_joystick = xr_rightController_joystick;
        
            rightController_triggerIsDown = xr_rightController_triggerIsDown;
            rightController_triggerValue = xr_rightController_triggerValue;

            rightController_gripValue = xr_rightController_gripValue;
            rightController_gripIsDown = xr_rightController_gripIsDown;
        }
        else if( steamVR_isValid )
        {
            // read [ Right ] SteamVR
            
            rightController_tracking = true;

            rightController_joystick = VRInputs.instance.rightAxis;

            rightController_gripValue = VRInputs.instance.rightGrip;
            rightController_gripIsDown = rightController_gripValue > 0.5f;

            rightController_triggerValue = VRInputs.instance.rightTrigger;
            rightController_triggerIsDown = rightController_triggerValue > 0.5f;
            
            rightController_primaryButton = VRInputs.instance.rightPrimaryButtonDown;
            rightController_secondaryButton = VRInputs.instance.rightSecondaryButtonDown;
        }

        #endregion

        #region Fire Events

        if( leftController_primaryButton || rightController_primaryButton ) OnPrimaryButton?.Invoke();

        #endregion
    }

    #endregion

    #region Config-settings and public methods 

    [Header("Configuration")] 
    [Range(0.1f,5f)] public float FetchRetryCooldown = 1f;

    public void ForceRefresh()
    {
        if( ! fetching_hmd ) 
            StartCoroutine( Fetch_HMD() );
        
        if( ! fetching_leftController ) 
            StartCoroutine( Fetch_LeftController() );

        if( ! fetching_rightController )
            StartCoroutine( Fetch_RightController() );
    }

    #endregion

    #region Common values

    public event System.Action OnPrimaryButton;

    [Header("Common Vars")]

    public bool         menu_buttonDown;
    
    [HideInInspector] public bool         xr_menu_buttonDown;

    #endregion

    #region Device HMD and its values

    [Header("HMD Vars")]

    public bool         hmd_tracking;
    public Vector3      hmd_position;
    public Quaternion   hmd_rotation;
    
    public bool hasHMD => ! ( deviceList_HMD == null || deviceList_HMD.Count < 1 );
    
    public InputDevice activeHMD => ! hasHMD ? default : deviceList_HMD[ 0 ];
    
    List<InputDevice> deviceList_HMD;
    
    bool fetching_hmd = false;

    IEnumerator Fetch_HMD()
    {   
        fetching_hmd = true;

        while( ! hasHMD )
        {
            if( deviceList_HMD == null ) deviceList_HMD = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics( DeviceType.HeadMounted, deviceList_HMD );

            if( ! hasHMD ) Debug.LogWarning("Failed to fetch HMD, retying...");

            yield return new WaitForSeconds( FetchRetryCooldown );
        }
        
        yield return null;
        
        fetching_hmd = false;
    }


    void Scan_HMD()
    {
        if( ! hasHMD ) return;
        
        activeHMD.TryGetFeatureValue( CommonUsages.isTracked, out hmd_tracking );
        
        if( hmd_tracking )
        {
            activeHMD.TryGetFeatureValue( CommonUsages.devicePosition,  out hmd_position );
            activeHMD.TryGetFeatureValue( CommonUsages.deviceRotation,  out hmd_rotation );
        }
    }

    #endregion

    #region Left Controller and its values

    [System.Obsolete, HideInInspector] public bool         leftController_joystickClick;
    [System.Obsolete, HideInInspector] public bool         leftController_joystickTouch;
    
    [Header("Left Controller Vars")]
    
    public bool         leftController_tracking;
    public Vector3      leftController_position;
    public Quaternion   leftController_rotation;
    public bool         leftController_primaryButton;
    public bool         leftController_secondaryButton;
    public Vector2      leftController_joystick;
    public bool         leftController_gripIsDown;
    public bool         leftController_triggerIsDown;

    [Range(0f,1f)]      public float leftController_triggerValue;
    [Range(0f,1f)]      public float leftController_gripValue;
    
    bool         xr_leftController_readInput = false;
    
    [HideInInspector] public bool         xr_leftController_tracking;
    [HideInInspector] public Vector3      xr_leftController_position;
    [HideInInspector] public Quaternion   xr_leftController_rotation;
    [HideInInspector] public bool         xr_leftController_primaryButton;
    [HideInInspector] public bool         xr_leftController_secondaryButton;
    [HideInInspector] public Vector2      xr_leftController_joystick;
    [HideInInspector] public bool         xr_leftController_joystickClick;
    [HideInInspector] public bool         xr_leftController_joystickTouch;
    [HideInInspector] public bool         xr_leftController_gripIsDown;
    [HideInInspector] public float        xr_leftController_gripValue;
    [HideInInspector] public bool         xr_leftController_triggerIsDown;
    [HideInInspector] public float        xr_leftController_triggerValue;
    
    public event System.Action OnFetchedLeft;
    
    public bool hasLeftController => ! ( deviceList_LeftController == null || deviceList_LeftController.Count < 1 );
    
    public InputDevice activeLeftController => ! hasLeftController ? default : deviceList_LeftController[ 0 ];
    
    [SerializeField] List<InputDevice> deviceList_LeftController;
    
    bool fetching_leftController = false;

    IEnumerator Fetch_LeftController()
    {   
        fetching_leftController = true;

        while( ! hasLeftController )
        {
            if( deviceList_LeftController == null ) deviceList_LeftController = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics( DeviceType.Left, deviceList_LeftController );

            if( ! hasLeftController ) Debug.LogWarning("Failed to fetch LeftController, retying...");

            yield return new WaitForSeconds( FetchRetryCooldown );
        }

        OnFetchedLeft?.Invoke();
        
        yield return null;
        
        fetching_leftController = false;
    }

    void Scan_LeftController()
    {
        if( ! hasLeftController ) return;
        
        activeLeftController.TryGetFeatureValue( CommonUsages.isTracked, out xr_leftController_tracking );

        if( xr_leftController_tracking )
        {
            activeLeftController.TryGetFeatureValue( CommonUsages.devicePosition,       out xr_leftController_position );
            activeLeftController.TryGetFeatureValue( CommonUsages.deviceRotation,       out xr_leftController_rotation );
            
            bool b = false;

            b |= activeLeftController.TryGetFeatureValue( CommonUsages.secondaryButton,      out xr_leftController_secondaryButton );
            b |= activeLeftController.TryGetFeatureValue( CommonUsages.primaryButton,        out xr_leftController_primaryButton );

            b |= activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxis,        out xr_leftController_joystick );      
            b |= activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxisClick,   out xr_leftController_joystickClick ); 
            b |= activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxisTouch,   out xr_leftController_joystickTouch );
        
            b |= activeLeftController.TryGetFeatureValue( CommonUsages.gripButton,           out xr_leftController_gripIsDown);
            b |= activeLeftController.TryGetFeatureValue( CommonUsages.grip,                 out xr_leftController_gripValue);

            b |= activeLeftController.TryGetFeatureValue( CommonUsages.triggerButton,        out xr_leftController_triggerIsDown);
            b |= activeLeftController.TryGetFeatureValue( CommonUsages.trigger,              out xr_leftController_triggerValue);
            
            b |= activeLeftController.TryGetFeatureValue( CommonUsages.menuButton,           out menu_buttonDown);
            
            xr_leftController_readInput = b;
        }
    }

    #endregion
    
    #region Right Controller and its values

    [System.Obsolete, HideInInspector] public bool         rightController_joystickClick;
    [System.Obsolete, HideInInspector] public bool         rightController_joystickTouch;
    
    [Header("Right Controller Vars")]
    
    public bool         rightController_tracking;
    public Vector3      rightController_position;
    public Quaternion   rightController_rotation;
    public bool         rightController_primaryButton;
    public bool         rightController_secondaryButton;
    public Vector2      rightController_joystick;
    public bool         rightController_gripIsDown;
    public bool         rightController_triggerIsDown;

    [Range(0f,1f)]      public float rightController_triggerValue;
    [Range(0f,1f)]      public float rightController_gripValue;
    
    bool         xr_rightController_readInput = false;
    
    [HideInInspector] public bool         xr_rightController_tracking;
    [HideInInspector] public Vector3      xr_rightController_position;
    [HideInInspector] public Quaternion   xr_rightController_rotation;
    [HideInInspector] public bool         xr_rightController_primaryButton;
    [HideInInspector] public bool         xr_rightController_secondaryButton;
    [HideInInspector] public Vector2      xr_rightController_joystick;
    [HideInInspector] public bool         xr_rightController_joystickClick;
    [HideInInspector] public bool         xr_rightController_joystickTouch;
    [HideInInspector] public bool         xr_rightController_gripIsDown;
    [HideInInspector] public float        xr_rightController_gripValue;
    [HideInInspector] public bool         xr_rightController_triggerIsDown;
    [HideInInspector] public float        xr_rightController_triggerValue;

    public event System.Action OnFetchedRight;
    
    public bool hasRightController => ! ( deviceList_RightController == null || deviceList_RightController.Count < 1 );
    
    public InputDevice activeRightController => ! hasRightController ? default : deviceList_RightController[ 0 ];
    
    List<InputDevice> deviceList_RightController;
    
    bool fetching_rightController = false;

    IEnumerator Fetch_RightController()
    {   
        fetching_rightController = true;

        while( ! hasRightController )
        {
            if( deviceList_RightController ==null ) deviceList_RightController = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics( DeviceType.Right, deviceList_RightController );

            if( ! hasRightController ) Debug.LogWarning("Failed to fetch RightController, retying...");

            yield return new WaitForSeconds( FetchRetryCooldown );
        }

        OnFetchedRight?.Invoke();
        
        yield return null;
        
        fetching_rightController = false;
    }

    void Scan_RightController()
    {
        if( ! hasRightController ) return;

        activeRightController.TryGetFeatureValue( CommonUsages.isTracked, out xr_rightController_tracking );

        if( xr_rightController_tracking )
        {
            activeRightController.TryGetFeatureValue( CommonUsages.devicePosition,       out xr_rightController_position );
            activeRightController.TryGetFeatureValue( CommonUsages.deviceRotation,       out xr_rightController_rotation );
        
            bool b = false;
            
            b |= activeRightController.TryGetFeatureValue( CommonUsages.secondaryButton,      out xr_rightController_secondaryButton );
            b |= activeRightController.TryGetFeatureValue( CommonUsages.primaryButton,        out xr_rightController_primaryButton );

            b |= activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxis,        out xr_rightController_joystick );      
            b |= activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxisClick,   out xr_rightController_joystickClick ); 
            b |= activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxisTouch,   out xr_rightController_joystickTouch );
        
            b |= activeRightController.TryGetFeatureValue( CommonUsages.gripButton,           out xr_rightController_gripIsDown);
            b |= activeRightController.TryGetFeatureValue( CommonUsages.grip,                 out xr_rightController_gripValue);

            b |= activeRightController.TryGetFeatureValue( CommonUsages.triggerButton,        out xr_rightController_triggerIsDown);
            b |= activeRightController.TryGetFeatureValue( CommonUsages.trigger,              out xr_rightController_triggerValue);
            
            b |= activeRightController.TryGetFeatureValue( CommonUsages.menuButton,           out xr_menu_buttonDown);

            xr_rightController_readInput = b;
        }
    }


    #endregion
    
    #region Trigger Emitters
    
    public struct TriggerEmitter 
    {
        public static float Min = 0.1f;
        public static float Max = 0.9f;

        public float value;

        public bool IsPressed => value > Max;
        public bool IsReleased => value < Min;

        public event System.Action OnRelease;
        public event System.Action OnPressed;

        public void Update( float newValue )
        {
            if( value > Min && newValue < Min ) OnRelease?.Invoke();
            if( value < Max && newValue > Max ) OnPressed?.Invoke();
        }
    }

    public TriggerEmitter leftController_grip;
    public TriggerEmitter leftController_trigger;
    public TriggerEmitter rightController_grip;
    public TriggerEmitter rightController_trigger;

    void Fetch_Emitters()
    {
        leftController_grip = new TriggerEmitter { value = 0 };
        leftController_trigger = new TriggerEmitter { value = 0 };
        
        rightController_grip = new TriggerEmitter { value = 0 };
        rightController_trigger = new TriggerEmitter { value = 0 };
    }

    void Update_Emitters()
    {
        leftController_grip.Update( leftController_gripValue );
        leftController_trigger.Update( leftController_triggerValue );

        rightController_grip.Update( rightController_gripValue );
        rightController_trigger.Update( rightController_triggerValue );
    }

    #endregion

}
