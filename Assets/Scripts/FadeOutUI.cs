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
}
