using Protobuf;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using UnityEditor.VersionControl;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Manager
{
    public class ChatManager : MonoSingleton<ChatManager>
    {
        private List<ChatMainItem> ChatList = new List<ChatMainItem>();

        private List<MessageList> MessageLists = new List<MessageList>();

        private string currentId;

        private void Start()
        {
            SocketManager.Instance.Register(MessageID.SendToOne, ReceiveMessage);
        }

        public void Init()
        {

        }

        public void InitMessageList(List<MessageList> MessageList)
        {
            this.MessageLists  = MessageList;
        }

        public void SwithToFriend(string friendid)
        {
            if (currentId == friendid) return;
            CloseLastFriend();
            CloseLastChat();
            //for(int i = 0; i < FriendsList.Count; i++)
            //{
            //    if (FriendsList[i].CheckId(currentId))
            //    {
            //        OpenOrCreateChat(friendid);
            //        return;
            //    }
            //}
            Debug.LogError("不该出现的错误");
        }
        private void OpenOrCreateChat(string friendid)
        {
            for(int i = 0; i < ChatList.Count; i++)
            {
                if (ChatList[i].CheckId(friendid))
                {
                    ChatList[i].gameObject.SetActive(true);
                    return;
                }
            }

            CreateChat(friendid);
            currentId = friendid;
        }

        private void AddMessageOrCreateChat(string friendid, string message, long stamp)
        {
            ChatMainItem chat;
            for (int i = 0; i < ChatList.Count; i++)
            {
                if (ChatList[i].CheckId(friendid))
                {
                    chat = ChatList[i];
                    chat.AddMessage(MessageType.ToMe, message, stamp);
                    return;
                }
            }

            chat = CreateChat(friendid);

            chat.AddMessage(MessageType.ToMe, message, stamp);
        }

        private ChatMainItem CreateChat(string friendid)
        {
            GameObject go = new GameObject(typeof(ChatMainItem).Name);
            go.transform.parent = GameObject.Find("ChatMainList").transform;
            ChatMainItem chat = go.AddComponent<ChatMainItem>();

            MessageList list = new MessageList(friendid);
            for (int i = 0; i < MessageLists.Count; i++)
            {
                if (MessageLists[i].CheckId(friendid))
                {
                    list = MessageLists[i];
                    break;
                }
            }

            chat.Init(currentId, list);
            return chat;
        }

        private void CloseLastFriend()
        {
            if (string.IsNullOrEmpty(currentId)) return;

        }

        private void CloseLastChat()
        {
            if (string.IsNullOrEmpty(currentId)) return;
            for (int i = 0; i < ChatList.Count; i++)
            {
                if (ChatList[i].CheckId(currentId))
                {
                    ChatList[i].gameObject.SetActive(false);
                    return;
                }
            }
        }

        private void ReceiveFrom(string fromid, string message)
        {
            Player player = GameController.Instance.GetPlayerById(fromid);
            foreach(var item in ChatList)
            {
                if (item.CheckId(fromid))
                {
                    //TODO:替换为服务器传过来的时间戳
                    AddMessageOrCreateChat(fromid, message, NetworkUtils.GetTimeStamp());
                }
            }
        }

        private void ReceiveMessage(byte[] data)
        {
            SendToOne toOne = SendToOne.Parser.ParseFrom(data);
            ReceiveFrom(toOne.FromId, toOne.Message);
        }
    }
}
