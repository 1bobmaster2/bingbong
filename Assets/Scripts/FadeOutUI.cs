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
        if (gameObject.activeSelf && shouldCheck)
        {
            text.CrossFadeAlpha(0, fadeOutSpeed, true); // this too doesnt work
            shouldCheck = false;
        }
    }
}
