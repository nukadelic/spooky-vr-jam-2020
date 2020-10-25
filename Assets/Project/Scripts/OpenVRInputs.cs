
// https://github.com/ValveSoftware/unity-xr-plugin/issues/16#issuecomment-692504445

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

//#if VRSTUDIOS_XRINPUT_OPENVR
using Valve.VR;
//#endif

namespace API.Input
{
	public sealed class OpenVRInputs : MonoBehaviour
    {
        public static OpenVRInputs singleton { get; private set; }

        public List<InputDevice> devices { get; private set; } = new List<InputDevice>();
        public List<InputDevice> controllers { get; private set; } = new List<InputDevice>();
        public InputDevice handLeft { get; private set; }
        public InputDevice handRight { get; private set; }

        //#if VRSTUDIOS_XRINPUT_OPENVR
        private const uint controllerStateLength = OpenVR.k_unMaxTrackedDeviceCount;
        //#else
        //private const int controllerStateLength = 4;
        //#endif
        private XRControllerState[] state_controllers = new XRControllerState[controllerStateLength];
        private uint state_controllerCount;
        private XRControllerState state_controllerLeft, state_controllerRight;

        private void Start()
        {
            // only one can exist in scene at a time
            if (singleton != null)
            {
                Destroy(gameObject);
                return;
		    }
            DontDestroyOnLoad(gameObject);
            singleton = this;

            devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);

            InputDevices.deviceConnected += InputDevices_deviceConnected;
		    InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
		    InputDevices.deviceConfigChanged += InputDevices_deviceConfigChanged;
        }

	    private void OnDestroy()
	    {
            InputDevices.deviceConnected -= InputDevices_deviceConnected;
            InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
            InputDevices.deviceConfigChanged -= InputDevices_deviceConfigChanged;
        }

	    private void Update()
        {
            state_controllerCount = 0;
            bool leftSet = false, rightSet = false;

            //#if VRSTUDIOS_XRINPUT_OPENVR
            var system = OpenVR.System;
            if (system == null || !system.IsInputAvailable()) return;

            for (uint i = 0; i != controllerStateLength; ++i)
            {
                if (!system.IsTrackedDeviceConnected(i)) continue;

                // update controller state
                if (system.GetTrackedDeviceClass(i) != Valve.VR.ETrackedDeviceClass.Controller) continue;
                var state = new VRControllerState_t();
                if (system.GetControllerState(i, ref state, (uint) System.Runtime.InteropServices.Marshal.SizeOf<VRControllerState_t>()))
                {
                    var controller = state_controllers[state_controllerCount];
                    controller.connected = true;

                    // update buttons states
                    controller.buttonTrigger.Update((state.ulButtonPressed & 8589934592) != 0);
                    controller.buttonGrip.Update((state.ulButtonPressed & 4) != 0);
                    controller.buttonMenu.Update((state.ulButtonPressed & 2) != 0);
                    controller.button1.Update((state.ulButtonPressed & 4294967296) != 0);

                    // update analog states
                    controller.trigger.Update(state.rAxis1.x);

                    // update joystick states
                    if (state.ulButtonTouched != 0) controller.joystick.Update(new Vector2(state.rAxis0.x, state.rAxis0.y));
                    else controller.joystick.Update(Vector2.zero);

                    // update controller side
                    var role = system.GetControllerRoleForTrackedDeviceIndex(i);
                    switch (role)
                    {
                        case Valve.VR.ETrackedControllerRole.LeftHand:
                            controller.side = XRControllerSide.Left;
                            state_controllerLeft = controller;
                            leftSet = true;
                            break;

                        case Valve.VR.ETrackedControllerRole.RightHand:
                            controller.side = XRControllerSide.Right;
                            state_controllerRight = controller;
                            rightSet = true;
                            break;

                        default: controller.side = XRControllerSide.Unknown; break;
                    }

                    state_controllers[state_controllerCount] = controller;
                    ++state_controllerCount;
                }
            }
            //#else
            //foreach (var c in controllers)
            //{
            //    if (!c.isValid || (c.characteristics & InputDeviceCharacteristics.Controller) == 0) continue;

            //    var controller = state_controllers[state_controllerCount];
            //    controller.connected = true;

            //    // update buttons states
            //    if (c.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton)) controller.buttonTrigger.Update(triggerButton);
            //    else controller.buttonTrigger.Update(false);

            //    if (c.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton)) controller.buttonGrip.Update(gripButton);
            //    else controller.buttonGrip.Update(false);

            //    if (c.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButton)) controller.buttonMenu.Update(menuButton);
            //    else controller.buttonMenu.Update(false);

            //    if (c.TryGetFeatureValue(CommonUsages.primaryButton, out bool button1)) controller.button1.Update(button1);
            //    else controller.button1.Update(false);

            //    if (c.TryGetFeatureValue(CommonUsages.secondaryButton, out bool button2)) controller.button2.Update(button2);
            //    else controller.button2.Update(false);

            //    // update analog states
            //    if (c.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) controller.trigger.Update(triggerValue);
            //    else controller.trigger.Update(0);

            //    // update joystick states
            //    if (c.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick)) controller.joystick.Update(joystick);
            //    else controller.joystick.Update(Vector2.zero);

            //    // update controller side
            //    if ((c.characteristics & InputDeviceCharacteristics.Left) != 0)
            //    {
            //        controller.side = XRControllerSide.Left;
            //        state_controllerLeft = controller;
            //        leftSet = true;
            //    }
            //    else if ((c.characteristics & InputDeviceCharacteristics.Right) != 0)
            //    {
            //        controller.side = XRControllerSide.Right;
            //        state_controllerRight = controller;
            //        rightSet = true;
            //    }
            //    else
            //    {
            //        controller.side = XRControllerSide.Unknown;
            //    }

            //    state_controllers[state_controllerCount] = controller;
            //    ++state_controllerCount;
            //}
            //#endif

            // null memory if no state
            if (!leftSet) state_controllerLeft = new XRControllerState();
            if (!rightSet) state_controllerRight = new XRControllerState();
        }

	    private void InputDevices_deviceConnected(InputDevice device)
	    {
            Debug.Log("XR Device connected: " + device.name);
            devices.Add(device);
            UpdateDevice(device, false);
        }

        private void InputDevices_deviceDisconnected(InputDevice device)
        {
            Debug.Log("XR Device disconnected: " + device.name);
            devices.Remove(device);
            UpdateDevice(device, true);
        }

        private void InputDevices_deviceConfigChanged(InputDevice device)
        {
            Debug.Log("XR Device config changed: " + device.name);
            var index = devices.FindIndex(x => x.name == device.name);
            devices[index] = device;
            UpdateDevice(device, false);
        }

        private void UpdateDevice(InputDevice device, bool removingDevice)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                if (controllers.Exists(x => x.name == device.name))
                {
				    if (!removingDevice)
				    {
					    var index = controllers.FindIndex(x => x.name == device.name);
					    controllers[index] = device;
				    }
                    else
                    {
                        controllers.Remove(device);
                    }
                }
                else
                {
                    if (!removingDevice) controllers.Add(device);
                }

                if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left))
                {
                    if (!removingDevice) handLeft = device;
                    else handLeft = new InputDevice();
			    }
                else if (device.characteristics.HasFlag(InputDeviceCharacteristics.Right))
                {
                    if (!removingDevice) handRight = device;
                    else handRight = new InputDevice();
			    }
            }
	    }

        public static XRControllerState ControllerState(XRController controller)
        {
            switch (controller)
            {
                case XRController.First: return singleton.state_controllers[0];
                case XRController.Left: return singleton.state_controllerLeft;
                case XRController.Right: return singleton.state_controllerRight;
                case XRController.Merged:
                {
                    var state = new XRControllerState();
                    for (uint i = 0; i != singleton.state_controllerCount; ++i)
                    {
                        var controllerState = singleton.state_controllers[i];
                        if (controllerState.connected) state.connected = true;

                        controllerState.buttonTrigger.Merge(ref state.buttonTrigger);
                        controllerState.buttonGrip.Merge(ref state.buttonGrip);
                        controllerState.buttonMenu.Merge(ref state.buttonMenu);

                        controllerState.button1.Merge(ref state.button1);
                        controllerState.button2.Merge(ref state.button2);
                        controllerState.button3.Merge(ref state.button3);
                        controllerState.button4.Merge(ref state.button4);

                        controllerState.trigger.Merge(ref state.trigger);
                        controllerState.joystick.Merge(ref state.joystick);
					}
                    return state;
                }
            }
            throw new NotImplementedException("XR Controller type not implemented" + controller.ToString());
	    }
    }

    public enum XRController
    {
        /// <summary>
        /// First controller connected
        /// </summary>
        First,

        /// <summary>
        /// All controller states merged
        /// </summary>
        Merged,

        /// <summary>
        /// Left controller states only
        /// </summary>
        Left,

        /// <summary>
        /// Right controller states only
        /// </summary>
        Right
    }

    public enum XRControllerSide
    {
        Unknown,
        Left,
        Right
	}

    public struct XRControllerState
    {
        public bool connected;
        public XRControllerSide side;
        public XRControllerButton buttonTrigger, buttonGrip, buttonMenu;
        public XRControllerButton button1, button2 ,button3, button4;
        public XRControllerAnalog trigger;
        public XRControllerJoystick joystick;
    }

    public struct XRControllerButton
    {
        public bool on, down, up;

        internal void Update(bool on)
        {
            down = false;
            up = false;
            if (this.on != on)
            {
                if (on) down = true;
                else if (!on) up = true;
            }
            this.on = on;
	    }

        internal void Merge(ref XRControllerButton button)
        {
            if (on) button.on = true;
            if (down) button.down = true;
            if (up) button.up = true;
		}
    }

    public struct XRControllerAnalog
    {
        public float value;
        public const float tolerance = 0.1f;

        internal void Update(float value)
        {
            if (value < tolerance) value = 0.0f;
            this.value = value;
	    }

        internal void Merge(ref XRControllerAnalog analog)
        {
            if (value >= tolerance) analog.value = value;
        }
    }

    public struct XRControllerJoystick
    {
        public Vector2 value;
        public const float tolerance = 0.1f;

        internal void Update(Vector2 value)
        {
            if (value.magnitude < tolerance) value = Vector2.zero;
            this.value = value;
        }

        internal void Merge(ref XRControllerJoystick joystick)
        {
            if (value.magnitude >= tolerance) joystick.value = value;
        }
    }
}

/*
public class TestInput : MonoBehaviour
{
    void Update()
    {
        var state = XRInput.ControllerState(XRController.Merged);

        // buttons
        PrintButton(state.buttonTrigger, "ButtonTrigger");
        PrintButton(state.buttonGrip, "ButtonGrid");
        PrintButton(state.buttonMenu, "ButtonMenu");
        PrintButton(state.button1, "Button1");
        PrintButton(state.button2, "Button2");
        PrintButton(state.button3, "Button3");
        PrintButton(state.button4, "Button4");

        // triggers
        PrintAnalog(state.trigger, "Trigger");

        // triggers
        PrintJoystick(state.joystick, "Joystick");
    }

    void PrintButton(XRControllerButton button, string name)
    {
        if (button.down) Debug.Log(name + " down");
        if (button.up) Debug.Log(name + " up");
    }

    void PrintAnalog(XRControllerAnalog analog, string name)
    {
       if (analog.value >= .1f) Debug.Log(name + " " + analog.value.ToString());
	}

    void PrintJoystick(XRControllerJoystick joystick, string name)
    {
        if (joystick.value.magnitude >= .1f) Debug.Log($"{name} {joystick.value.x}x{joystick.value.y}");
    }
}
*/