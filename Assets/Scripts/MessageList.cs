using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class MessageList
{
    private string FromId;

    private List<MessageItem> messageItems = new List<MessageItem>();

    private static int curid;

    public MessageList(string fromId)
    {
        FromId = fromId;
        curid = 0;
    }

    public MessageList(string fromId, List<MessageItem> messageItems) : this(fromId)
    {
        this.messageItems = messageItems;
    }

    public MessageItem CreateItem(MessageType type, string message, long stamp)
    {
        MessageItem item = new MessageItem(type, message, stamp, ++curid);
        return item;
    }
    
    public void Add(MessageType type, string message , long stamp)
    {
        messageItems.Add(new MessageItem(type, message, stamp, ++ curid));
    }

    public void Add(MessageItem item)
    {
        messageItems.Add(item);
    }

    public void Add(List<MessageItem> list)
    {
        messageItems.AddRange(list);
    }

    public void Add(MessageList list)
    {
        messageItems.AddRange(list.GetList());
    }

    public bool isEmpty()
    {
        return messageItems == null || messageItems.Count == 0;
    }

    public bool CheckId(string friendid)
    {
        return this.FromId == friendid;
    }

    public int Count()
    {
        return messageItems.Count;
    }

    public List<MessageItem> GetList()
    {
        return messageItems;
    }

    public MessageItem GetItem(int index)
    {
        if(index > messageItems.Count - 1)
        {
            Debug.LogError("超过列表最大长度");
            return new MessageItem(MessageType.None, "", 0, -1);
        }
        return messageItems[index];
    }
}

public struct MessageItem
{
    public MessageItem(MessageType type, string message, long timestamp, int id)
    {
        this.type = type;
        this.message = message;
        this.timestamp = timestamp;
        this.id = id;
    }
    public MessageType type { get; private set; }
    public string message { get; private set; }
    public long timestamp { get; private set; }
    public int id { get; private set; }
}

public enum MessageType
{
    None,
    FromMe,
    ToMe,
}
