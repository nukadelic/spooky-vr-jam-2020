
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class SpaceButton : MonoBehaviour
{
    public bool autoRotate = true;

    public TextMeshPro label;
    public GameObject display;
    public GameObject glow;
    
    Material displayMat;

    Vector3 rot_animation;

    public Color idl;
    public Color active;

    string text = "Text Not Set";

    public event System.Action<SpaceButton> OnClick;

    public UnityEvent ClickEvent;

    public void SetText( string txt )
    {
        text = txt;
        label.text = txt;
    }

    private void OnEnable( )
    {
        invokeLock = false;
    }

    void Start( )
    {
        rot_animation = new Vector3
        (
            Random.Range( 1f, 10f ),
            Random.Range( 1f, 10f ),
            Random.Range( 1f, 10f )
        );

        displayMat = display.GetComponent<MeshRenderer>().material;
    }

    private void OnDrawGizmosSelected( )
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere( transform.position, Radius * transform.lossyScale.magnitude );
    }

    public float Radius = 1f;

    enum Hand { None = -1, Left = 0, Right = 1 }
    
    Hand activeHand = Hand.None;
    
    bool invokeLock = false;

    IEnumerator Invoke()
    {
        invokeLock = true;

        OnClick?.Invoke( this );
        ClickEvent.Invoke();

        yield return new WaitForSeconds( 1f );

        invokeLock = false;
    }

    void Update( )
    {
        if( activeHand != Hand.None )
        {
            bool left_down = activeHand == Hand.Left && XRInputs.instance.leftController_gripIsDown;
            bool right_down = activeHand == Hand.Right && XRInputs.instance.rightController_gripIsDown;

            if( ( left_down || right_down ) && ! invokeLock )
            {
                StartCoroutine( Invoke() ); 
                return;
            }
        }

        var R = Radius * transform.lossyScale.magnitude;

        var distL = HandScript.Left.transform.position - transform.position;
        var distR = HandScript.Right.transform.position - transform.position;

        if( activeHand == Hand.None )
        {
            if( distL.magnitude < R ) 
            {
                // left entered the area 
                activeHand = Hand.Left;
                glow.SetActive( true );
                label.text = "Grip to select";
            }
            if( distR.magnitude < R )
            {
                // right entererd the area 
                activeHand = Hand.Right;
                glow.SetActive( true );
                label.text = "Grip to select";
            }
        }
        else
        {
            var dist = ( activeHand == Hand.Left ? distL : distR ).magnitude;

            var t = ( 1f - ( dist / R ) ) * 0.6f + 0.4f;
            var c = Color.Lerp( idl, active, t );

            displayMat.SetColor("_EmissionColor", c );

            if( dist > R )
            {
                activeHand = Hand.None;
                glow.SetActive( false );
                label.text = text;
            }
        }
        
        if( autoRotate )
        {
            display.transform.Rotate( rot_animation * Time.deltaTime, Space.Self );        
        }
    }


}
