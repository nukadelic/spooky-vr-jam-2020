using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerClimb : MonoBehaviour
{
    // shared value across all sewers as the height values used in blender when designed 
    public static float SewerHeight = 20f;

    private void OnValidate( )
    {
        platformsLayerMask = 1 << BitwiseUtil.GetBiggestBit( platformsLayerMask );
        sewerLayerMask = 1 << BitwiseUtil.GetBiggestBit( sewerLayerMask );
    }

    [Header("Generator Params")]
    public LayerMask platformsLayerMask;
    public LayerMask sewerLayerMask;
    public int randomSeed = 2345;
    public Material sewerMaterial;
    public float sewerScale = 1f;
    public GameObject[] sewerSpawnPrefabs;

    public GameObject[] platforms;
    
    [Range(0.5f,10f)]
    public float platformScaleMin = 2f;
    [Range(0.5f,10f)]
    public float platformScaleMax = 4f;

    [Header("Track Active Parts")]
    //public SewerPart below;
    public SewerPart current;
    //public SewerPart above;

    void Start()
    {
        Random.InitState( randomSeed );

        for( var i = 0; i < 3; ++i )
        {
            SpawnPart( i );
        }
    }

    public SewerPart SpawnPart( int offset )
    {
        int idx = Random.Range( 0, sewerSpawnPrefabs.Length );
        var prefab = sewerSpawnPrefabs[ idx ];


        var go = Instantiate( prefab );
        
        go.layer = BitwiseUtil.GetBiggestBit( sewerLayerMask );

        go.transform.parent = transform;
        go.transform.position = Vector3.up * sewerScale * SewerHeight * offset;
        go.transform.rotation = Quaternion.identity;

        var scale = Vector3.one * sewerScale;
        if( Random.value > 0.5f )
            scale.y *= -1; 
        
        go.transform.localScale = scale;

        // Give collider
        go.AddComponent<MeshCollider>().sharedMesh = go.GetComponent<MeshFilter>().sharedMesh;
       


        go.GetComponent<MeshRenderer>( ).material = sewerMaterial;

        var part = go.AddComponent<SewerPart>();

        if( current == null ) current = part;

        float platform_count = Random.Range( 10, 20 );

        // index +1 and -1 :: do not spawn at the edges 

        for(var i = 1; i < platform_count - 1; ++i ) 
        {
            var platform_elevation = ( (float) i / platform_count );
            var platform_offset_y = SewerHeight * platform_elevation - ( SewerHeight / 2f );
            var platform_local_sacle = Random.Range( platformScaleMin, platformScaleMax );
            var platform_rotation =  Random.value * 360f;
            int platform_index = Random.Range( 0, platforms.Length );

            for( var j = 0; j < 2; j ++ )
            {
                SpawnPlatform( go.transform, 
                    platform_index, 
                    platform_offset_y, 
                    platform_rotation, 
                    platform_local_sacle 
                );

                platform_rotation += 180f;
            }
        }

        return part;
    }

    public GameObject SpawnPlatform( Transform parent , int i, float y, float rot, float scale )
    {   
        var item = platforms[ i ];
        
        var go = Instantiate( item );
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.up * y;
        go.layer = BitwiseUtil.GetBiggestBit( platformsLayerMask ); // << 1

        go.transform.localScale = Vector3.one * scale;
        
        var rotQ = Quaternion.Euler( 0, rot, 0 );
        
        var rayO = go.transform.position;
        var rayD = rotQ * parent.forward;

        if( ! Physics.Raycast( rayO, rayD, out RaycastHit hit, 1000f )) //, layerMask.value ) )
        {
            Debug.LogError("Failed to raycast");
            return null;
        }

        //var angle = Vector3.Angle( hitinfo.normal, Vector3.forward );

        go.transform.rotation = rotQ;
        go.transform.position = hit.point + hit.normal * scale * 0.5f;
        
        //Physics.Raycast(  )

        return item;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
