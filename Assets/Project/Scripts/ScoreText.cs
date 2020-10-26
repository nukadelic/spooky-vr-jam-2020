using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public TextMeshPro txt;

    void Start()
    {
        txt = GetComponent<TextMeshPro>();
    }
    
    void Update()
    {
        txt.text = PunpkinController.Instance.candyPoints.ToString();
    }
}
