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
        //text.color = new Color32(255, 255, 255, 50); // i seriously don't know whats going on here (still waiting for response) (this line has been commented as it doesnt work properly)
    }

    void OnEnable()
    {
        Invoke("DisableObject", 3);
    }

    void DisableObject()
    {
        Destroy(gameObject); // please help me
        text.color = new Color32(255, 50, 255, 50);
    }
}
