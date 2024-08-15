using UnityEngine;
using TMPro;

public class TabNavigation : MonoBehaviour
{
    public TMP_InputField[] inputFields;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            for (int i = 0; i < inputFields.Length; i++)
            {
                if (inputFields[i].isFocused)
                {
                    int nextIndex = (i + 1) % inputFields.Length;
                    inputFields[nextIndex].ActivateInputField();
                    break;
                }
            }
        }
    }
}