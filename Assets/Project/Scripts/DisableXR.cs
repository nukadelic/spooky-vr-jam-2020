using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DisableXR : MonoBehaviour
{
    [System.Serializable]
    public struct DataToSurviveEditorCompilation
    {
        // Define variables here 
        public bool disableXR;
    }

    static DataToSurviveEditorCompilation Data;

    public string helpBox = "Disable XR For this scene only";

    [SerializeField]
    public DataToSurviveEditorCompilation SetScriptParams = new DataToSurviveEditorCompilation 
    { 
        // Set varaible initial value here
        disableXR = false
    };

    // Update data from editor to static
    private void OnValidate( ) => Data = SetScriptParams;

    // Recover changes
    private void OnDestroy( )
    {
        if( ! Application.isPlaying || Recover.state < 1 ) return;
        
#if UNITY_EDITOR
        if( EditorApplication.isCompiling ) return;
        try { UnityEngine.XR.Management.XRGeneralSettings.Instance.InitManagerOnStart = Recover.init_xr_onstart; }
        catch( System.Exception ex ) { }
#endif

    }

    struct RecoverDataState 
    { 
        public int state;
        public bool init_xr_onstart;
    }

    static RecoverDataState Recover;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] 
    static void RuntimeInitSubsystemRegistration() 
    {
        if( ! Application.isPlaying ) return;

        var _data = DisableXR.FindObjectOfType<DisableXR>()?.SetScriptParams ?? Data;

        bool init_xr_onstart = UnityEngine.XR.Management.XRGeneralSettings.Instance.InitManagerOnStart;

        Recover = new RecoverDataState 
        { 
            state = 1,
            init_xr_onstart = init_xr_onstart
        };

        Recover.init_xr_onstart = false;

        if( _data.disableXR && init_xr_onstart )
        {
            Debug.Log( "XR SubsystemRegistration :: preventing init manager on start" );

            Recover.init_xr_onstart = true;
            
#if UNITY_EDITOR
            UnityEngine.XR.Management.XRGeneralSettings.Instance.InitManagerOnStart = false;
#endif
            var complete = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.isInitializationComplete;

            if ( complete )
            {
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();
                    
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }

        }

        //Debug.Log( "XR InitManagerOnStart: " + UnityEngine.XR.Management.XRGeneralSettings.Instance.InitManagerOnStart );

        //var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name ?? "";
        //Debug.Log( "XR detected scene: " + scene );

        //Debug.Log("XR Auto load.1: " + UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.automaticLoading );
        //Debug.Log("XR Auto load.2: " + UnityEngine.XR.Management.XRGeneralSettings.Instance.AssignedSettings.automaticLoading );
        //Debug.Log("XR Auto run.1: " + UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.automaticRunning );
        //Debug.Log("XR Auto run.2: " + UnityEngine.XR.Management.XRGeneralSettings.Instance.AssignedSettings.automaticRunning );

        //UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.automaticLoading = false;
        //UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.automaticRunning = false;

        //Debug.Log( "XR Active loader: " + UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.activeLoader );
        //UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.activeLoader?.Deinitialize();

        

        //var complete = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.isInitializationComplete;
        //Debug.Log("XR Init Complete : " + complete );

        //if ( complete )
        //{
        //    Debug.Log("XR Stopping sub systems...");
        //    UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();
            
        //    //UnityEngine.XR.Management.XRLoaderHelper;

        //    //( Camera.main ?? Camera.current ).ResetAspect();
            
        //    Debug.Log("XR Deinitializing loader...");
        //    UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();

        //    Debug.Log("XR Disabled plugins");
        //}

        //else
        //{
        //    Debug.Log("XR Failed to prevent loading plugins");
        //}
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(DisableXR))]
    class QuickEditor : Editor
    {
        public override void OnInspectorGUI( )
        {
            GUILayout.Space( 5 );

            var script = ( DisableXR ) target;

            EditorGUILayout.HelpBox( script.helpBox , MessageType.None);

            GUILayout.Space( 5 );

            var b = script.SetScriptParams.disableXR;

            b = EditorGUILayout.ToggleLeft( "Disable XR", b );

            script.SetScriptParams.disableXR = b;
            
            GUILayout.Space( 5 );
        }
    }
    #endif
}

