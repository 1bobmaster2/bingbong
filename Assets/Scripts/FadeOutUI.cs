using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutUI : MonoBehaviour
{
    [SerializeField] private float fadeOutSpeed; 
    [SerializeField] private Color fadeOutColor;
    private Text text;
    private bool shouldCheck = true;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 50);
    }
}
