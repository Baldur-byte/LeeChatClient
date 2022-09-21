using Google.Protobuf;
using Manager;
using Protobuf;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ChatMainItem : MonoBehaviour
{
    [SerializeField]
    private GameObject FromMePrefab;
    [SerializeField]
    private GameObject ToMePrefab;
    [SerializeField]
    private Text friendName;
    [SerializeField]
    private InputField MessageToSend;
    [SerializeField]
    private Button SendBtn;
    [SerializeField]
    private Transform ChatScroll;

    private string friendId;

    private MessageList messageList;

    private Queue<MessageItem> addList = new Queue<MessageItem>();

    private MessageItem item;

    private void Start()
    {
        SendBtn.onClick.AddListener(SendBtnOnClick);
        messageList = new MessageList(GameController.Instance.Player.Uuid());
    }

    private void Update()
    {
        if(addList.Count > 0)
        {
            item = addList.Dequeue();
            AddChatContent(item);
        }
    }

    private void SendBtnOnClick()
    {
        SendToOne send = new SendToOne();
        send.FromId = GameController.Instance.Player.Uuid();
        send.ToId = friendId;
        send.Message = MessageToSend.text;
        SocketManager.Instance.Send(MessageID.SendToOne, send);

        //TODO:  服务器返回发送结果，再进行本地显示更新
        //AddChatContent(messageList.CreateItem(MessageType.FromMe, MessageToSend.text, NetworkUtils.GetTimeStamp()));
        AddMessage(MessageType.FromMe, MessageToSend.text, NetworkUtils.GetTimeStamp());
    }

    public void Init(string friendId, MessageList list = null)
    {
        this.friendId = friendId;
        if(list == null || list.isEmpty()) return;

        ClearScroll();
        ShowChatContent();
    }

    public bool CheckId(string id)
    {
        return friendId.Equals(id);
    }

    private void ShowChatContent()
    {
        List<MessageItem> list = messageList.GetList();
        for (int i = 0; i < list.Count; i++)
        {
            AddChatContent(list[i]);
        }
    }

    private void AddChatContent(MessageItem item)
    {
        if(item.type == MessageType.FromMe)
        {
            GameObject gameObject = GameObject.Instantiate(FromMePrefab, ChatScroll);
            gameObject.GetComponent<PrintMessage>().SetMessage(GameController.Instance.Player, item.message);
        }
        else if (item.type == MessageType.ToMe)
        {
            GameObject gameObject = GameObject.Instantiate(ToMePrefab, ChatScroll);
            gameObject.GetComponent<PrintMessage>().SetMessage(GameController.Instance.GetPlayerById(friendId), item.message);
        }
        else
        {
            Debug.LogError("错误的消息");
        }
    }

    private void ClearScroll()
    {
        Transform[] trans = ChatScroll.GetComponentsInChildren<Transform>();
        foreach(Transform t in trans)
        {
            DestroyImmediate(t.gameObject);
        }
    }

    private void SendCallBack(byte[] data)
    {

    }

    public void AddMessage(MessageItem messageItem)
    {
        addList.Enqueue(messageItem);
        this.messageList.Add(messageList);
    }

    public void AddMessage(MessageType type, string message, long stamp)
    {
        AddMessage(messageList.CreateItem(type, message, stamp));
    }

    private void AddMessage(List<MessageItem> messageList)
    {
        for (int i = 0; i < messageList.Count(); i++)
        {
            AddMessage(messageList[i]);
        }
    }

    private void AddMessage(MessageList messageList)
    {
        AddMessage(messageList.GetList());
    }

    

    

    
}
