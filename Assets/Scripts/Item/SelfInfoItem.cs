using UnityEngine;
using UnityEngine.UI;

public class SelfInfoItem : MonoBehaviour
{
    [SerializeField]
    private Image Icon;
    [SerializeField]
    private Text Name;
    [SerializeField]
    private Text Uuid;

    private string Description;

    private Player Player;

    public void Init()
    {
        Player = GameController.Instance.Player;

        Refresh();
    }

    private void Refresh()
    {
        Name.text = Player.Name();
        Uuid.text = Player.Uuid();
    }
}
