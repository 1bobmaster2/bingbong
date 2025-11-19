using UnityEngine;
using UnityEngine.UI;

public class PasteHandler : MonoBehaviour
{
    [SerializeField] private InputField inputField; // this is the input field of the ui object the script is attached to

    void FixedUpdate()
    {
        if (inputField.isFocused && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.V))
        {
            inputField.text = GUIUtility.systemCopyBuffer;
        }
    }
}
