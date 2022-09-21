using Manager;
using Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text name;
    [SerializeField]
    private Text lastMessage;

    private Button ChatButton;

    private string uuid;

    [SerializeField]
    private GameObject ChatPrefab;

    private ChatMainItem chatMainItem;

    public void Init(PlayerInfo info)
    {
        //this.icon = info.IconUrl;
        this.name.text = info.Name;
        //this.lastMessage.text = lastMessage;
        this.uuid = info.Uuid;
    }

    private void Start()
    {
        ChatButton.onClick.AddListener(ChatBtnOnClick);
    }

    private void ChatBtnOnClick()
    {
        ChatManager.Instance.SwithToFriend(uuid);
    }

    public bool CheckId(string uuid)
    {
        return this.uuid.Equals(uuid);
    }
}
