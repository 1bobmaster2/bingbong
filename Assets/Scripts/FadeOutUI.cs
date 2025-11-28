using TMPro;
using UnityEngine;

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
            text.CrossFadeAlpha(0, fadeOutSpeed, true);
            shouldCheck = false;
        }
    }
}
