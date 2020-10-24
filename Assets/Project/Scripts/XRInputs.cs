using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using DeviceType = UnityEngine.XR.InputDeviceCharacteristics;

public class XRInputs : MonoBehaviour
{
    // -------------------------------------------------------

    public static XRInputs instance;
    
    #region Static Hook

    static void HookStatic( XRInputs _instance )
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

    private void Awake( )
    {
        HookStatic( this );
    }

    void Update()
    {

        Scan_LeftController();
        Scan_RightController();
    }

    #endregion
    
    public void FetchHardware()
    {
        Fetch_HMD();
        Fetch_LeftController();
        Fetch_RightController();
    }
    protected virtual void OnEnable()
    {
        FetchHardware();
        Application.onBeforeRender += OnBeforeRender;
    }

    protected virtual void OnDisable()
    {

        Application.onBeforeRender -= OnBeforeRender;
    }


    protected virtual void OnBeforeRender()
    {
        Scan_HMD();
    }
    // -------------------------------------------------------

    #region Common values

    [Header("Common Vars")]

    public bool         menu_buttonDown;

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
        
        if( ! hasHMD )
            
            Debug.LogWarning("Failed to fetch HMD, check `hasHMD` after fetching hardware");
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

    [Header("Left Controller Vars")]

    public bool         leftController_tracking;
    public Vector3      leftController_position;
    public Quaternion   leftController_rotation;
    public Vector2      leftController_joystick;
    public bool         leftController_joystickClick;
    public bool         leftController_joystickTouch;
    public bool         leftController_gripIsDown;
    public float        leftController_gripValue;
    public bool         leftController_triggerIsDown;
    public float        leftController_triggerValue;

    
    public bool hasLeftController => ! ( deviceList_LeftController == null || deviceList_LeftController.Count < 1 );
    
    public InputDevice activeLeftController => ! hasLeftController ? default : deviceList_LeftController[ 0 ];
    
    [SerializeField] List<InputDevice> deviceList_LeftController = new List<InputDevice>();
    
    void Fetch_LeftController()
    {   
        if( ! hasLeftController )

            InputDevices.GetDevicesWithCharacteristics( DeviceType.Left, deviceList_LeftController );
        
        if( ! hasLeftController )
            
            Debug.LogWarning("Failed to fetch LeftController, check `hasLeftController` after fetching hardware");
    }

    void Scan_LeftController()
    {
        if( ! hasLeftController ) 
        { 
            Fetch_LeftController();
            return;
        }
        
        activeLeftController.TryGetFeatureValue( CommonUsages.isTracked, out leftController_tracking );

        if( leftController_tracking )
        {
            activeLeftController.TryGetFeatureValue( CommonUsages.devicePosition,       out leftController_position );
            activeLeftController.TryGetFeatureValue( CommonUsages.deviceRotation,       out leftController_rotation );
        
            activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxis,        out leftController_joystick );      
            activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxisClick,   out leftController_joystickClick ); 
            activeLeftController.TryGetFeatureValue( CommonUsages.primary2DAxisTouch,   out leftController_joystickTouch );
        
            activeLeftController.TryGetFeatureValue( CommonUsages.gripButton,           out leftController_gripIsDown);
            activeLeftController.TryGetFeatureValue( CommonUsages.grip,                 out leftController_gripValue);

            activeLeftController.TryGetFeatureValue( CommonUsages.triggerButton,        out leftController_triggerIsDown);
            activeLeftController.TryGetFeatureValue( CommonUsages.trigger,              out leftController_triggerValue);
            
            activeLeftController.TryGetFeatureValue( CommonUsages.menuButton,           out menu_buttonDown);
        }
    }

    #endregion
    
    #region Right Controller and its values

    [Header("Right Controller Vars")]

    public bool         rightController_tracking;
    public Vector3      rightController_position;
    public Quaternion   rightController_rotation;
    public Vector2      rightController_joystick;
    public bool         rightController_joystickClick;
    public bool         rightController_joystickTouch;
    public bool         rightController_gripIsDown;
    public float        rightController_gripValue;
    public bool         rightController_triggerIsDown;
    public float        rightController_triggerValue;

    
    public bool hasRightController => ! ( deviceList_RightController == null || deviceList_RightController.Count < 1 );
    
    public InputDevice activeRightController => ! hasRightController ? default : deviceList_RightController[ 0 ];
    
    [SerializeField] List<InputDevice> deviceList_RightController = new List<InputDevice>();
    
    void Fetch_RightController()
    {   
        if( ! hasRightController )

            InputDevices.GetDevicesWithCharacteristics( DeviceType.Right, deviceList_RightController );
        
        if( ! hasRightController )
            
            Debug.LogWarning("Failed to fetch RightController, check `hasRightController` after fetching hardware");
    }

    void Scan_RightController()
    {
        if( ! hasRightController ) 
        {
            Fetch_RightController();
            return;
        }

        activeRightController.TryGetFeatureValue( CommonUsages.isTracked, out rightController_tracking );

        if( rightController_tracking )
        {
            activeRightController.TryGetFeatureValue( CommonUsages.devicePosition,       out rightController_position );
            activeRightController.TryGetFeatureValue( CommonUsages.deviceRotation,       out rightController_rotation );
        
            activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxis,        out rightController_joystick );      
            activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxisClick,   out rightController_joystickClick ); 
            activeRightController.TryGetFeatureValue( CommonUsages.primary2DAxisTouch,   out rightController_joystickTouch );
        
            activeRightController.TryGetFeatureValue( CommonUsages.gripButton,           out rightController_gripIsDown);
            activeRightController.TryGetFeatureValue( CommonUsages.grip,                 out rightController_gripValue);

            activeRightController.TryGetFeatureValue( CommonUsages.triggerButton,        out rightController_triggerIsDown);
            activeRightController.TryGetFeatureValue( CommonUsages.trigger,              out rightController_triggerValue);
            
            activeRightController.TryGetFeatureValue( CommonUsages.menuButton,           out menu_buttonDown);
        }
    }

    #endregion
}
