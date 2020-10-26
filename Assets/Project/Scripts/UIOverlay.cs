using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour
{
    public static bool IsOpen = false;

    public SpaceButton btnRestart;
    public SpaceButton bntSnap;

    bool primaryButtonCooldown = false;

    IEnumerator OnPrimaryButton()
    {
        primaryButtonCooldown = true;
        
            Debug.Log("Primary Button" + !gameObject.activeSelf );

        gameObject.SetActive( !gameObject.activeSelf );

        yield return new WaitForSeconds( 1f );

        primaryButtonCooldown = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        XRInputs.instance.OnPrimaryButton += () =>
        {   
            if( ! primaryButtonCooldown ) 
                
                GlobalCoroutine.instance.StartCoroutine( OnPrimaryButton() );
        };

        transform.localPosition = new Vector3( 0, 1, 0.5f );

        btnRestart.SetText("Restart");

        btnRestart.OnClick += x =>
        {
            //var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            //UnityEngine.SceneManagement.SceneManager.LoadScene( s.buildIndex );

            gameObject.SetActive( false );

            PunpkinController.Instance.ResetVariables();
        };
        
        bntSnap.SetText(PunpkinController.Instance.snapTurning ? "Enable Snap Turning" : "Enable Smooth Turning");
        
        bntSnap.OnClick += x =>
        {
            PunpkinController.Instance.snapTurning = ! PunpkinController.Instance.snapTurning ;
            
            bntSnap.SetText(PunpkinController.Instance.snapTurning ? "Enable Snap Turning" : "Enable Smooth Turning");
        };
        
        gameObject.SetActive( false );
    }
}
