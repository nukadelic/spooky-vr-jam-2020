using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using DeviceType = UnityEngine.XR.InputDeviceCharacteristics;
using Valve.VR;
using System.Linq;
using System.Collections;

public class XRInputs : MonoBehaviour
{
    [Header("Debug (temp)")]

    public TMPro.TextMeshProUGUI debugText;

    public bool isDebugging => debugText != null;

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

        SteamVR_ScanInputs();

        if( isDebugging )
        {
            debugText.text = "";
            
            debugText.text += "\n" + "(L) primaryButton: "   + leftController_primaryButton.ToString()
                + "\t\t (R) primaryButton"   + rightController_primaryButton.ToString();
            debugText.text += "\n" + "(L) joystick: "        + leftController_joystick.ToString()
                + "\t\t (R) joystick"        + rightController_joystick.ToString();
            debugText.text += "\n" + "(L) joystickClick: "   + leftController_joystickClick.ToString()
                + "\t\t (R) joystickClick"   + rightController_joystickClick.ToString();
            debugText.text += "\n" + "(L) joystickTouch: "   + leftController_joystickTouch.ToString()
                + "\t\t (R) joystickTouch"   + rightController_joystickTouch.ToString();
            debugText.text += "\n" + "(L) gripIsDown: "      + leftController_gripIsDown.ToString()
                + "\t\t (R) gripIsDown"      + rightController_gripIsDown.ToString();
            debugText.text += "\n" + "(L) gripValue: "       + leftController_gripValue.ToString("N2")
                + "\t\t (R) gripValue"       + rightController_gripValue.ToString("N2");
            debugText.text += "\n" + "(L) triggerIsDown: "   + leftController_triggerIsDown.ToString()
                + "\t\t (R) triggerIsDown"   + rightController_triggerIsDown.ToString();
            debugText.text += "\n" + "(L) triggerValue: "    + leftController_triggerValue.ToString("N2")
                + "\t\t (R) triggerValue"    + rightController_triggerValue.ToString("N2");

        }
    }

    #endregion
    
    public void ForceRefresh()
    {
        if( ! steamVR_captured ) 
            StartCoroutine( SteamVR_Capture() );

        Fetch_HMD();
        Fetch_LeftController();
        Fetch_RightController();
    }

    // -------------------------------------------------------

    #region SteamVR Setup

    [Header("SteamVR")]

    public SteamVR_ActionSet controllerMapping;
    
    static SteamVR_Input_Sources leftController = SteamVR_Input_Sources.LeftHand;
    static SteamVR_Input_Sources rightController = SteamVR_Input_Sources.RightHand;

    public SteamVR_Action_Vector2 inputJoystick = SteamVR_Input.GetVector2Action("joystick_position");
    public SteamVR_Action_Single inputGrip = SteamVR_Input.GetSingleAction("grip_pull");
    public SteamVR_Action_Single inputTrigger = SteamVR_Input.GetSingleAction("trigger_pull");
    public SteamVR_Action_Boolean inputPrimary = SteamVR_Input.GetBooleanAction("primary_button");   

    IEnumerator SteamVR_Capture()
    {
        //SteamVR.Initialize();

        if( ! steamVR_captured)
        {
            while 
            ( 
                SteamVR.initializedState == SteamVR.InitializedStates.None || 
                SteamVR.initializedState == SteamVR.InitializedStates.Initializing
            )
                    yield return null;

            if(SteamVR.instance != null)
            {
                Debug.Log("Steam VR captured instance");

                SteamVR_ReadyToCapture();
                
                steamVR_captured = true;
            }
            else
            {
                Debug.LogError("Failed to capture VR , retry ... ");

                SteamVR.Initialize( true );

                StartCoroutine( SteamVR_Capture() );

                // throw new System.Exception("Missing SteamVR Instance");
            }

        }

        yield return null;
    }

    bool steamVR_captured = false;

    void SteamVR_ReadyToCapture( )
    {
        for( var i = 0; i < transform.childCount; ++i )

            transform.GetChild( i ).gameObject.SetActive( true );

        //OnFetchedLeft -= SteamVR_ReadyToCapture;
        //OnFetchedRight -= SteamVR_ReadyToCapture;

        controllerMapping.Activate( leftController | rightController, 0, true );

        //steamVR_captured = controllerMapping.IsActive();

        //if( ! controllerMapping.IsActive() ) Debug.LogError("Failed to activate selected controller mapping, check your target");

        if( isDebugging )
        {
            Debug.Log( "XRInputs :: SteamVR_Input.actions: " + 
                string.Join( ", " , SteamVR_Input.actions.Select( x => x.GetShortName() ) ) );
        }     
    }

    void SteamVR_ScanInputs()
    {
        if( ! steamVR_captured ) return;

        if( inputJoystick != null && inputJoystick.activeBinding )
        {
            leftController_joystick = inputJoystick.GetAxis( leftController );
            rightController_joystick = inputJoystick.GetAxis( rightController );
        }
        
        if( inputGrip != null && inputGrip.activeBinding )
        {
            leftController_gripValue = inputGrip.GetAxis( leftController );
            rightController_gripValue = inputGrip.GetAxis( rightController );

            leftController_gripIsDown = leftController_gripValue > 0.5f;
            rightController_gripIsDown = rightController_gripValue > 0.5f;
        }

        if( inputTrigger != null && inputTrigger.activeBinding )
        {
            leftController_triggerValue = inputTrigger.GetAxis( leftController );
            rightController_triggerValue = inputTrigger.GetAxis( rightController );
            
            leftController_triggerIsDown = leftController_triggerValue > 0.5f;
            rightController_triggerIsDown = rightController_triggerValue > 0.5f;
        }

        if( inputPrimary != null && inputPrimary.activeBinding )
        {
            leftController_primaryButton = inputPrimary.GetState( leftController );
            rightController_primaryButton = inputPrimary.GetState( rightController );
        }
        
        //leftController_joystick = SteamVR_Actions.VRInputs.joystick_position[ leftController ].axis;

        //Debug.Log( SteamVR_Actions.VRInputs.joystick_position[ leftController ].axis );
        
        //Debug.Log( "GrabGrip.state " + SteamVR_Actions._default.GrabGrip.state );
        //Debug.Log( "GrabPinch.state " + SteamVR_Actions._default.GrabPinch.state );
    }

    #endregion

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
    public bool         leftController_primaryButton;
    public Vector2      leftController_joystick;
    public bool         leftController_joystickClick;
    public bool         leftController_joystickTouch;
    public bool         leftController_gripIsDown;
    public float        leftController_gripValue;
    public bool         leftController_triggerIsDown;
    public float        leftController_triggerValue;
    
    public event System.Action OnFetchedLeft;
    
    public bool hasLeftController => ! ( deviceList_LeftController == null || deviceList_LeftController.Count < 1 );
    
    public InputDevice activeLeftController => ! hasLeftController ? default : deviceList_LeftController[ 0 ];
    
    [SerializeField] List<InputDevice> deviceList_LeftController = new List<InputDevice>();
    
    void Fetch_LeftController()
    {   
        if( ! hasLeftController )

            InputDevices.GetDevicesWithCharacteristics( DeviceType.Left, deviceList_LeftController );
        
        if( ! hasLeftController )
            
            Debug.LogWarning("Failed to fetch LeftController, check `hasLeftController` after fetching hardware");

        else OnFetchedLeft?.Invoke();
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
            
            activeLeftController.TryGetFeatureValue( CommonUsages.primaryButton,        out leftController_primaryButton );

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
    public bool         rightController_primaryButton;
    public Vector2      rightController_joystick;
    public bool         rightController_joystickClick;
    public bool         rightController_joystickTouch;
    public bool         rightController_gripIsDown;
    public float        rightController_gripValue;
    public bool         rightController_triggerIsDown;
    public float        rightController_triggerValue;

    public event System.Action OnFetchedRight;
    
    public bool hasRightController => ! ( deviceList_RightController == null || deviceList_RightController.Count < 1 );
    
    public InputDevice activeRightController => ! hasRightController ? default : deviceList_RightController[ 0 ];
    
    [SerializeField] List<InputDevice> deviceList_RightController = new List<InputDevice>();
    
    void Fetch_RightController()
    {   
        if( ! hasRightController )

            InputDevices.GetDevicesWithCharacteristics( DeviceType.Right, deviceList_RightController );
        
        if( ! hasRightController )
            
            Debug.LogWarning("Failed to fetch RightController, check `hasRightController` after fetching hardware");

        else OnFetchedRight?.Invoke();
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
        
            activeRightController.TryGetFeatureValue( CommonUsages.primaryButton,        out rightController_primaryButton );

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
