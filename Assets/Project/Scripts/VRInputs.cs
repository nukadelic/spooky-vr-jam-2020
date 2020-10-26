using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Linq;

public class VRInputs : MonoBehaviour
{
    public SteamVR_ActionSet controllerMapping;

    public static VRInputs instance;

    private void Start( )
    {
        if( instance != null )
        {
            Destroy( gameObject );

            return;
        }

        instance = this;

        StartCoroutine( Hook( ) );
    }
    
    public SteamVR_Action_Vector2 joystick = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("joystick_position");
    public SteamVR_Action_Single trigger = SteamVR_Input.GetAction<SteamVR_Action_Single>("trigger_pull");
    public SteamVR_Action_Single grip = SteamVR_Input.GetAction<SteamVR_Action_Single>("grip_pull");
    public SteamVR_Action_Boolean primary_button = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("primary_button");
    public SteamVR_Action_Boolean secondary_button = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("secondary_button");
    
    public bool leftPrimaryButtonDown => primary_button.GetState( SteamVR_Input_Sources.LeftHand );
    public bool rightPrimaryButtonDown => primary_button.GetState( SteamVR_Input_Sources.RightHand );

    public bool leftSecondaryButtonDown => secondary_button.GetState( SteamVR_Input_Sources.LeftHand );
    public bool rightSecondaryButtonDown => secondary_button.GetState( SteamVR_Input_Sources.RightHand );
    
    public float leftGrip => grip.GetAxis(SteamVR_Input_Sources.LeftHand);
    public float rightGrip => grip.GetAxis(SteamVR_Input_Sources.RightHand);
    
    public float leftTrigger => trigger.GetAxis(SteamVR_Input_Sources.LeftHand);
    public float rightTrigger => trigger.GetAxis(SteamVR_Input_Sources.RightHand);

    public Vector2 leftAxis => joystick.GetAxis(SteamVR_Input_Sources.LeftHand);
    public Vector2 rightAxis => joystick.GetAxis(SteamVR_Input_Sources.RightHand);

    [HideInInspector] public SteamVR_Action_Boolean[] listActionTrigger;
    [HideInInspector] public SteamVR_Action_Single[] listActionSingle;
    [HideInInspector] public SteamVR_Action_Vector2[] listActionVector2;

    public bool steamVR_captured = false;

    IEnumerator Hook()
    {
        if( ! steamVR_captured)
        {
            //SteamVR.Initialize( );
            //UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
            //UnityEngine.XR.XRSettings.LoadDeviceByName("OpenVR");

            while 
            ( 
                SteamVR.initializedState == SteamVR.InitializedStates.None || 
                SteamVR.initializedState == SteamVR.InitializedStates.Initializing
            )
                    yield return null;

            if(SteamVR.instance != null)
            {
                Debug.Log("Steam VR captured instance");
                steamVR_captured = true;
                SteamVR_CompleteCapture();
            }
            else
            {
                Debug.LogWarning("Failed to capture VR , retry ... ");
                yield return new WaitForSeconds( 2f );
                StartCoroutine( Hook() );
            }

        }

        yield return null;
    }
        
    void SteamVR_CompleteCapture()
    {
        listActionTrigger = SteamVR_Input.actionsBoolean;
        listActionSingle = SteamVR_Input.actionsSingle;
        listActionVector2 = SteamVR_Input.actionsVector2;

        controllerMapping.Activate( SteamVR_Input_Sources.Any );
        
        Debug.Log("Action Set Activated : " + controllerMapping.GetShortName() );

        Debug.Log( "SteamVR_Input.actions=" +
            string.Join(" , ", SteamVR_Input.actions.Select( x=> x.GetShortName() + "[" + (x.active?"T":"F") + "]:" + x.GetType() ) ) );    
        
    }
}
