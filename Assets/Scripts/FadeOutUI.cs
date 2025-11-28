using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutUI : MonoBehaviour
{
    [SerializeField] private float fadeOutSpeed; 
    private TextMeshProUGUI text;
    private bool shouldCheck = true;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
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
