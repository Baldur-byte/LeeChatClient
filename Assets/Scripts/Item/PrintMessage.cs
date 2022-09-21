using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintMessage : MonoBehaviour
{
    [SerializeField]
    private Image IconImage;
    [SerializeField]
    private Text MessageText;

    public void SetMessage(Player player, string message)
    {
        MessageText.text = message;
    }
}
