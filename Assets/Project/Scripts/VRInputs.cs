using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Linq;

public class VRInputs : MonoBehaviour
{
    #region Debug Text

    [Header("Debug Text")]
    public TMPro.TextMeshProUGUI statusText;
    public TMPro.TextMeshProUGUI leftText;
    public TMPro.TextMeshProUGUI rightText;
    public void LogStatus( params object[] args ) {if( statusText == null ) return;statusText.text += string.Join(" ", args) + "\n";}
    public void LogLeft( params object[] args ) {if( leftText == null ) return;leftText.text += string.Join(" ", args) + "\n";}
    public void LogRight( params object[] args ) {if( rightText == null ) return;rightText.text += string.Join(" ", args) + "\n";}
    public void ClearLeftLog() {if( leftText == null ) return;leftText.text = "";}
    public void ClearRightLog() {if( rightText == null ) return;rightText.text = "";}

    void DebugStart()
    {
        if( statusText == null ) return;

        statusText.text = "";

        LogStatus("SteamVR_Input.actions: ");

        foreach( var x in  SteamVR_Input.actions )
        {
            LogStatus( x.GetType(), x.GetShortName(), "active="+x.active );
        }
    }

    void DebugUpdate()
    {
        controllerMapping?.Activate( SteamVR_Input_Sources.LeftHand | SteamVR_Input_Sources.RightHand, 0, true );

        if( ! steamVR_captured ) return;

        foreach( var item in listActionTrigger )
        {
            if( item == null || ! item.activeBinding ) continue;
            LogLeft( "T", item.GetShortName() , ":\t", item.GetState( SteamVR_Input_Sources.LeftHand ) );
            LogRight( "T", item.GetShortName() , ":\t", item.GetState( SteamVR_Input_Sources.RightHand ) );
        }
        
        foreach( var item in listActionSingle )
        {
            if( item == null || ! item.activeBinding ) continue;
            LogLeft( "S", item.GetShortName() , ":\t", item.GetAxis( SteamVR_Input_Sources.LeftHand ).ToString("N3") );
            LogRight( "S", item.GetShortName() , ":\t", item.GetAxis( SteamVR_Input_Sources.RightHand ).ToString("N3") );
        }

        foreach( var item in listActionVector2 )
        {
            if( item == null || ! item.activeBinding ) continue;
            LogLeft( "V", item.GetShortName() , ":\t", item.GetAxis( SteamVR_Input_Sources.LeftHand ) );
            LogRight( "V", item.GetShortName() , ":\t", item.GetAxis( SteamVR_Input_Sources.RightHand ) );
        }
    }
    
    void DebugXRInputAPI()
    {
        if( API.Input.XRInput.singleton == null ) return;

        var left = API.Input.XRInput.ControllerState( API.Input.XRController.Left );
            
        if( left.connected )
        {
            LogLeft("Button 1 : " , left.button1.down );
            LogLeft("Button 2 : " , left.button2.down );
            LogLeft("Button 3 : " , left.button3.down );
            LogLeft("Button 4 : " , left.button4.down );
            LogLeft("Button Grip : " , left.buttonGrip.down );
            LogLeft("Button Menu : " , left.buttonMenu.down );
            LogLeft("Button Trigger : " , left.buttonTrigger.down );
            LogLeft("Trigger Value : " , left.trigger.value.ToString("N3") );
            LogLeft("Joystick : ", left.joystick.value );
        }
        else LogLeft("Disconnected");

        var right = API.Input.XRInput.ControllerState( API.Input.XRController.Right );


        if( right.connected )
        {
            LogRight("Button 1 : " , right.button1.down );
            LogRight("Button 2 : " , right.button2.down );
            LogRight("Button 3 : " , right.button3.down );
            LogRight("Button 4 : " , right.button4.down );
            LogRight("Button Grip : " , right.buttonGrip.down );
            LogRight("Button Menu : " , right.buttonMenu.down );
            LogRight("Button Trigger : " , right.buttonTrigger.down );
            LogRight("Trigger Value : " , right.trigger.value.ToString("N3") );
            LogRight("Joystick : ", right.joystick.value );
        }
        else LogRight("Disconnected");
    }

    void DebugXRInputs()
    {
        LogLeft("[xr]", "primaryButton", XRInputs.instance.leftController_primaryButton );
        LogRight("[xr]", "primaryButton",  XRInputs.instance.rightController_primaryButton );
        
        LogLeft("[xr]", "joystick", XRInputs.instance.leftController_joystick );
        LogRight("[xr]", "joystick",  XRInputs.instance.rightController_joystick );
        
        LogLeft("[xr]", "joystickClick", XRInputs.instance.leftController_joystickClick );
        LogRight("[xr]", "joystickClick",  XRInputs.instance.rightController_joystickClick );
        
        LogLeft("[xr]", "joystickTouch", XRInputs.instance.leftController_joystickTouch );
        LogRight("[xr]", "joystickTouch",  XRInputs.instance.rightController_joystickTouch );
        
        LogLeft("[xr]", "gripIsDown", XRInputs.instance.leftController_gripIsDown );
        LogRight("[xr]", "gripIsDown",  XRInputs.instance.rightController_gripIsDown );

        LogLeft("[xr]", "gripIsDown", XRInputs.instance.leftController_gripIsDown );
        LogRight("[xr]", "gripIsDown",  XRInputs.instance.rightController_gripIsDown );
        
        LogLeft("[xr]", "gripValue", XRInputs.instance.leftController_gripValue );
        LogRight("[xr]", "gripValue",  XRInputs.instance.rightController_gripValue );
        
        LogLeft("[xr]", "triggerIsDown", XRInputs.instance.leftController_triggerIsDown );
        LogRight("[xr]", "triggerIsDown",  XRInputs.instance.rightController_triggerIsDown );

        LogLeft("[xr]", "triggerValue", XRInputs.instance.leftController_triggerValue );
        LogRight("[xr]", "triggerValue",  XRInputs.instance.rightController_triggerValue );
    }


    #endregion
    
    public SteamVR_ActionSet controllerMapping;

    private void Start( )
    {
        DebugStart();

        StartCoroutine( Hook( ) );
    }

    public SteamVR_Action_Boolean[] listActionTrigger;
    public SteamVR_Action_Single[] listActionSingle;
    public SteamVR_Action_Vector2[] listActionVector2;

    bool steamVR_captured = false;

    IEnumerator Hook()
    {
        if( ! steamVR_captured)
        {
            SteamVR.Initialize( );
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
                LogStatus("Steam VR captured instance");

                SteamVR_ReadyToCapture();
                
                steamVR_captured = true;
            }
            else
            {
                LogStatus("Failed to capture VR , retry ... ");
                
                yield return new WaitForSeconds( 1f );

                StartCoroutine( Hook() );
            }

        }

        yield return null;
    }

    void SteamVR_ReadyToCapture()
    {
        // Do we need to activate map ? - steamVR exmaples don't do that  

    }

    private void Update( )
    {
        ClearLeftLog(); 
        ClearRightLog();
        
        DebugXRInputAPI();
        DebugUpdate();
        DebugXRInputs();
    }


}
