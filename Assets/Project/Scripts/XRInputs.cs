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

    public static XRInputs instance;
    
    #region Static Hook

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
        
        // read Left XR

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
        
        // read Right XR

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
        
        if( VRInputs.instance && VRInputs.instance.steamVR_captured )
        {
            // read Left SteamVR

            leftController_tracking = true;

            leftController_joystick = VRInputs.instance.leftAxis;

            leftController_gripValue = VRInputs.instance.leftGrip;
            leftController_gripIsDown = leftController_gripValue > 0.5f;

            leftController_triggerValue = VRInputs.instance.leftTrigger;
            leftController_triggerIsDown = leftController_triggerValue > 0.5f;
            
            leftController_primaryButton = VRInputs.instance.leftPrimaryButtonDown;
            leftController_secondaryButton = VRInputs.instance.leftSecondaryButtonDown;
            
            // read Right SteamVR
            
            rightController_tracking = true;

            rightController_joystick = VRInputs.instance.rightAxis;

            rightController_gripValue = VRInputs.instance.rightGrip;
            rightController_gripIsDown = rightController_gripValue > 0.5f;

            rightController_triggerValue = VRInputs.instance.rightTrigger;
            rightController_triggerIsDown = rightController_triggerValue > 0.5f;
            
            rightController_primaryButton = VRInputs.instance.rightPrimaryButtonDown;
            rightController_secondaryButton = VRInputs.instance.rightSecondaryButtonDown;
        }
    }

    #endregion
    
    public void ForceRefresh()
    {
        Fetch_HMD();
        Fetch_LeftController();
        Fetch_RightController();
    }
    
    #region Common values

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
    
    [SerializeField] List<InputDevice> deviceList_HMD = new List<InputDevice>();
    
    void Fetch_HMD()
    {   
        if( ! hasHMD )

            InputDevices.GetDevicesWithCharacteristics( DeviceType.HeadMounted, deviceList_HMD );
        
        if( ! hasHMD ) Debug.LogWarning("Failed to fetch HMD");
    }

    void Scan_HMD()
    {
        if( ! hasHMD ) 
        { 
            Fetch_HMD();
            return;
        }
        
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
    public float        leftController_gripValue;
    public bool         leftController_triggerIsDown;
    public float        leftController_triggerValue;
    
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
    
    [SerializeField] List<InputDevice> deviceList_LeftController = new List<InputDevice>();
    
    void Fetch_LeftController()
    {   
        if( ! hasLeftController )

            InputDevices.GetDevicesWithCharacteristics( DeviceType.Left, deviceList_LeftController );
        
        if( ! hasLeftController ) Debug.LogWarning("Failed to fetch LeftController");

        else OnFetchedLeft?.Invoke();
    }

    void Scan_LeftController()
    {
        if( ! hasLeftController ) 
        { 
            Fetch_LeftController();
            return;
        }
        
        activeLeftController.TryGetFeatureValue( CommonUsages.isTracked, out xr_leftController_tracking );

        if( xr_leftController_tracking )
        {
            activeLeftController.TryGetFeatureValue( CommonUsages.devicePosition,       out xr_leftController_position );
            activeLeftController.TryGetFeatureValue( CommonUsages.deviceRotation,       out xr_leftController_rotation );
            
            activeLeftController.TryGetFeatureValue( CommonUsages.secondaryButton,      out xr_leftController_secondaryButton );
            activeLeftController.TryGetFeatureValue( CommonUsages.primaryButton,        out xr_leftController_primaryButton );

            activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxis,        out xr_leftController_joystick );      
            activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxisClick,   out xr_leftController_joystickClick ); 
            activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxisTouch,   out xr_leftController_joystickTouch );
        
            activeLeftController.TryGetFeatureValue( CommonUsages.gripButton,           out xr_leftController_gripIsDown);
            activeLeftController.TryGetFeatureValue( CommonUsages.grip,                 out xr_leftController_gripValue);

            activeLeftController.TryGetFeatureValue( CommonUsages.triggerButton,        out xr_leftController_triggerIsDown);
            activeLeftController.TryGetFeatureValue( CommonUsages.trigger,              out xr_leftController_triggerValue);
            
            activeLeftController.TryGetFeatureValue( CommonUsages.menuButton,           out menu_buttonDown);
            
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
    public float        rightController_gripValue;
    public bool         rightController_triggerIsDown;
    public float        rightController_triggerValue;
    
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
    
    [SerializeField] List<InputDevice> deviceList_RightController = new List<InputDevice>();
    
    void Fetch_RightController()
    {   
        if( ! hasRightController )

            InputDevices.GetDevicesWithCharacteristics( DeviceType.Right, deviceList_RightController );
        
        if( ! hasRightController ) Debug.LogWarning("Failed to fetch RightController");

        else OnFetchedRight?.Invoke();
    }

    void Scan_RightController()
    {
        if( ! hasRightController ) 
        {
            Fetch_RightController();
            return;
        }

        activeRightController.TryGetFeatureValue( CommonUsages.isTracked, out xr_rightController_tracking );

        if( xr_rightController_tracking )
        {
            activeRightController.TryGetFeatureValue( CommonUsages.devicePosition,       out xr_rightController_position );
            activeRightController.TryGetFeatureValue( CommonUsages.deviceRotation,       out xr_rightController_rotation );
        
            activeRightController.TryGetFeatureValue( CommonUsages.secondaryButton,      out xr_rightController_secondaryButton );
            activeRightController.TryGetFeatureValue( CommonUsages.primaryButton,        out xr_rightController_primaryButton );

            activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxis,        out xr_rightController_joystick );      
            activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxisClick,   out xr_rightController_joystickClick ); 
            activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxisTouch,   out xr_rightController_joystickTouch );
        
            activeRightController.TryGetFeatureValue( CommonUsages.gripButton,           out xr_rightController_gripIsDown);
            activeRightController.TryGetFeatureValue( CommonUsages.grip,                 out xr_rightController_gripValue);

            activeRightController.TryGetFeatureValue( CommonUsages.triggerButton,        out xr_rightController_triggerIsDown);
            activeRightController.TryGetFeatureValue( CommonUsages.trigger,              out xr_rightController_triggerValue);
            
            activeRightController.TryGetFeatureValue( CommonUsages.menuButton,           out xr_menu_buttonDown);
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
