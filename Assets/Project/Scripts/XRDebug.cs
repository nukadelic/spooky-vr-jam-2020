using System.Linq;
using UnityEngine;
using Valve.VR;

public class XRDebug : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textFloor;
    public TMPro.TextMeshProUGUI textStatus;
    public TMPro.TextMeshProUGUI textLeft;
    public TMPro.TextMeshProUGUI textRight;
    
    private void Start( )
    {
        textStatus.text = "";
        
        Application.logMessageReceived += GotLog;
    }

    void GotLog( string condition, string stackTrace, LogType type )
    {
        var color = "white";
        if( type == LogType.Warning ) color = "yellow";
        if( type == LogType.Error || type == LogType.Exception ) color = "red";
        textStatus.text += "<color=" + color +">" + condition + "</color>\n";
    }

    private void Update( )
    {
        textLeft.text = "";
        textRight.text = "";
        textFloor.text = "";

        LogOpenVR();
        LogUnityXR();
        LogUnifiedInputs();
        LogSteamVR();
    }
    
    #region Unified Inputs

    void LogUnifiedInputs()
    {
        textFloor.text = "";

        if(XRInputs.instance == null) 
        {
            textFloor.text = "Missing XRInputs scripts";
            return;
        }

        textFloor.text += "[L] primaryButton" + XRInputs.instance.leftController_primaryButton + "\t";
        textFloor.text += "[R] primaryButton" +  XRInputs.instance.rightController_primaryButton + "\n";
        
        textFloor.text += "[L] joystick" + XRInputs.instance.leftController_joystick + "\t";
        textFloor.text += "[R] joystick" +  XRInputs.instance.rightController_joystick + "\n";
        
        //textFloor.text += "[L] joystickClick" + XRInputs.instance.leftController_joystickClick + "\t";
        //textFloor.text += "[R] joystickClick" +  XRInputs.instance.rightController_joystickClick + "\n";
        
        //textFloor.text += "[L] joystickTouch" + XRInputs.instance.leftController_joystickTouch + "\t";
        //textFloor.text += "[R] joystickTouch" +  XRInputs.instance.rightController_joystickTouch + "\n";
        
        textFloor.text += "[L] gripIsDown" + XRInputs.instance.leftController_gripIsDown + "\t";
        textFloor.text += "[R] gripIsDown" +  XRInputs.instance.rightController_gripIsDown + "\n";

        textFloor.text += "[L] gripIsDown" + XRInputs.instance.leftController_gripIsDown + "\t";
        textFloor.text += "[R] gripIsDown" +  XRInputs.instance.rightController_gripIsDown + "\n";
        
        textFloor.text += "[L] gripValue" + XRInputs.instance.leftController_gripValue + "\t";
        textFloor.text += "[R] gripValue" +  XRInputs.instance.rightController_gripValue + "\n";
        
        textFloor.text += "[L] triggerIsDown" + XRInputs.instance.leftController_triggerIsDown + "\t";
        textFloor.text += "[R] triggerIsDown" +  XRInputs.instance.rightController_triggerIsDown + "\n";

        textFloor.text += "[L] triggerValue" + XRInputs.instance.leftController_triggerValue + "\t";
        textFloor.text += "[R] triggerValue" +  XRInputs.instance.rightController_triggerValue + "\n";
    }

    #endregion

    #region SteamVR

    bool svrActIzDead( SteamVR_Action item, out string item_status )
    {         
        item_status = "[isNull=" + ( item == null ? "T" : "F" );
        if( item!=null ) item_status += ", active=" + (item.activeBinding?"T":"F");
        item_status += "]";
        return ( item == null || ! item.activeBinding );
    }

    void LogSteamVR()
    {

        if( VRInputs.instance == null || ! VRInputs.instance.steamVR_captured )
        {
            textLeft.text += ("-- SteamVR iz Dead -- \n");
            textRight.text += ("-- SteamVR iz Dead -- \n");
            return;
        }
        
        textLeft.text += ("-- SteamVR -- \n");
        textRight.text += ("-- SteamVR -- \n");
        
        var map = VRInputs.instance.controllerMapping;

        textLeft.text += (  "VR Map - raw active = " + map.ReadRawSetActive( SteamVR_Input_Sources.LeftHand ) ) + "\n"; 
        textRight.text += ( "VR Map - raw active = " + map.ReadRawSetActive( SteamVR_Input_Sources.RightHand ) ) + "\n"; 

        foreach( var item in VRInputs.instance.listActionTrigger )
        {
            if( svrActIzDead( item, out string item_status ) ) {}// textLeft.text += (item_status);textRight.text += (item_status); continue; }
            textLeft.text += ( item_status,":T:", item.GetShortName() , ":\t", item.GetState( SteamVR_Input_Sources.LeftHand ) ) + "\n";
            textRight.text += ( item_status,":T:", item.GetShortName() , ":\t", item.GetState( SteamVR_Input_Sources.RightHand ) ) + "\n";
        }
        
        foreach( var item in VRInputs.instance.listActionSingle )
        {
            if( svrActIzDead( item, out string item_status ) ) {}// textLeft.text += (item_status);textRight.text += (item_status); continue; }
            textLeft.text += ( item_status,":S:", item.GetShortName() , ":\t", item.GetAxis( SteamVR_Input_Sources.LeftHand ).ToString("N3") ) + "\n";
            textRight.text += ( item_status,":S:", item.GetShortName() , ":\t", item.GetAxis( SteamVR_Input_Sources.RightHand ).ToString("N3") ) + "\n";
        }

        foreach( var item in VRInputs.instance.listActionVector2 )
        {
            if( svrActIzDead( item, out string item_status ) ) {}// textLeft.text += (item_status);textRight.text += (item_status); continue; }
            textLeft.text += ( item_status,":V:", item.GetShortName() , ":\t", item.GetAxis( SteamVR_Input_Sources.LeftHand ) ) + "\n";
            textRight.text += ( item_status,":V:", item.GetShortName() , ":\t", item.GetAxis( SteamVR_Input_Sources.RightHand ) ) + "\n";
        }

        textLeft.text += ("-- SteamVR :: END -- ") + "\n";
        textRight.text += ("-- SteamVR :: END -- ") + "\n";
    }

    #endregion

    #region OpenVR

    void LogOpenVR()
    {
        if( API.Input.OpenVRInputs.singleton == null ) 
        {
            textLeft.text += "-- OpenVR iz Dead  --\n";
            textRight.text += "-- OpenVR iz Dead  --\n";
            return;
        }
        
        textLeft.text += ("-- OpenVR -- \n");
        textRight.text += ("-- OpenVR -- \n");

        var left = API.Input.OpenVRInputs.ControllerState( API.Input.XRController.Left );
            
        if( left.connected )
        {
            textLeft.text += ("Button 1 : " + left.button1.down + "\n");
            textLeft.text += ("Button 2 : " + left.button2.down + "\n");
            textLeft.text += ("Button 3 : " + left.button3.down + "\n");
            textLeft.text += ("Button 4 : " + left.button4.down + "\n");
            textLeft.text += ("Button Grip : " + left.buttonGrip.down + "\n");
            textLeft.text += ("Button Menu : " + left.buttonMenu.down + "\n");
            textLeft.text += ("Button Trigger : " + left.buttonTrigger.down + "\n");
            textLeft.text += ("Trigger Value : " + left.trigger.value.ToString("N3") + "\n");
            textLeft.text += ("Joystick : "+ left.joystick.value + "\n");
        }
        else textLeft.text += ("Disconnected" + "\n");

        var right = API.Input.OpenVRInputs.ControllerState( API.Input.XRController.Right );
        
        if( right.connected )
        {
            textRight.text += ("Button 1 : " + right.button1.down + "\n");
            textRight.text += ("Button 2 : " + right.button2.down + "\n");
            textRight.text += ("Button 3 : " + right.button3.down + "\n");
            textRight.text += ("Button 4 : " + right.button4.down + "\n");
            textRight.text += ("Button Grip : " + right.buttonGrip.down + "\n");
            textRight.text += ("Button Menu : " + right.buttonMenu.down + "\n");
            textRight.text += ("Button Trigger : " + right.buttonTrigger.down + "\n");
            textRight.text += ("Trigger Value : " + right.trigger.value.ToString("N3") + "\n");
            textRight.text += ("Joystick : "+ right.joystick.value + "\n");
        }
        else textRight.text += ("Disconnected" + "\n");
        
        textLeft.text += "-- OpenVR::End --\n";
        textRight.text += "-- OpenVR::End --\n";
    }

    #endregion

    #region UnityXR
 
    void LogUnityXR()
    {
        if(XRInputs.instance == null) 
        {
            textLeft.text += ("-- UnityXR iz Dead  -- \n");
            textRight.text += ("-- UnityXR iz Dead  -- \n");
            return;
        }
        
        textLeft.text += ("-- UnityXR -- \n");
        textRight.text += ("-- UnityXR -- \n");

        textLeft.text += "primaryButton" + XRInputs.instance.xr_leftController_primaryButton + "\n";
        textRight.text+= "primaryButton" +  XRInputs.instance.xr_rightController_primaryButton + "\n";
        
        textLeft.text += "joystick" + XRInputs.instance.xr_leftController_joystick + "\n";
        textRight.text+= "joystick" +  XRInputs.instance.xr_rightController_joystick + "\n";
        
        textLeft.text += "joystickClick" + XRInputs.instance.xr_leftController_joystickClick + "\n";
        textRight.text+= "joystickClick" +  XRInputs.instance.xr_rightController_joystickClick + "\n";
        
        textLeft.text += "joystickTouch" + XRInputs.instance.xr_leftController_joystickTouch + "\n";
        textRight.text+= "joystickTouch" +  XRInputs.instance.xr_rightController_joystickTouch + "\n";
        
        textLeft.text += "gripIsDown" + XRInputs.instance.xr_leftController_gripIsDown + "\n";
        textRight.text+= "gripIsDown" +  XRInputs.instance.xr_rightController_gripIsDown + "\n";

        textLeft.text += "gripIsDown" + XRInputs.instance.xr_leftController_gripIsDown + "\n";
        textRight.text+= "gripIsDown" +  XRInputs.instance.xr_rightController_gripIsDown + "\n";
        
        textLeft.text += "gripValue" + XRInputs.instance.xr_leftController_gripValue + "\n";
        textRight.text+= "gripValue" +  XRInputs.instance.xr_rightController_gripValue + "\n";
        
        textLeft.text += "triggerIsDown" + XRInputs.instance.xr_leftController_triggerIsDown + "\n";
        textRight.text+= "triggerIsDown" +  XRInputs.instance.xr_rightController_triggerIsDown + "\n";

        textLeft.text += "triggerValue" + XRInputs.instance.xr_leftController_triggerValue + "\n";
        textRight.text+= "triggerValue" +  XRInputs.instance.xr_rightController_triggerValue + "\n";
        
        textLeft.text += "-- UnityXR::End --\n";
        textRight.text += "-- UnityXR::End --\n";
    }

    #endregion
}