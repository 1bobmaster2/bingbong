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
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime * fadeOutSpeed);
            if (text.color.a == 0)
            {
                shouldCheck = false;
            }
        }
    }
}
