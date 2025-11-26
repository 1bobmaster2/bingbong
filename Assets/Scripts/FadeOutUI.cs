using TMPro;
using UnityEngine;

public class FadeOutUI : MonoBehaviour
{
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
            shouldCheck = false;
            //start fading out
        }
    }
}
