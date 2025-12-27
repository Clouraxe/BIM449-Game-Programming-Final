using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionManager : MonoBehaviour
{
    public Button[] options;

    public void SelectOption(Button selected)
    {
        foreach (Button btn in options)
        {
            btn.image.color = Color.white;
            btn.GetComponentInChildren<TMP_Text>().color = Color.black;
        }

        selected.image.color = Color.black;
        selected.GetComponentInChildren<TMP_Text>().color = Color.white;
    }
}
