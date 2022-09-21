using Protobuf;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;

namespace Manager
{
    public class FriendListManager : Singleton<FriendListManager>
    {
        private Transform FriendListRoot;

        private GameObject FriendItemPrefab;

        private List<string> FriendList;

        private List<FriendItem> FriendsItemList;

        public void Init()
        {
            FriendListRoot = GameObject.Find("FriendScroll").transform;
            FriendList = GameController.Instance.Player.Friends();

            InitFriendList();
        }

        private void InitFriendList()
        {

        }

        private bool isExistFriend(string friendId)
        {
            return FriendList.Contains(friendId);
        }

        public FriendItem GetFriendById(string friendId)
        {
            for(int i = 0; i < FriendsItemList.Count; i++)
            {
                if (FriendsItemList[i].CheckId(friendId)) return FriendsItemList[i];
            }
            return null;
        }

        private FriendItem FindFriendItemById(string friendId)
        {
            if (!isExistFriend(friendId)) 
            {
                Debug.LogError("用户未添加该好友");
                return null;
            }

            foreach(FriendItem friendItem in FriendsItemList)
            {
                if(friendItem.CheckId(friendId)) return friendItem;
            }
            return null;
        }

        private void CreateFriendItem(string friendId)
        {
            FriendItem friendItem = GameObject.Instantiate(FriendItemPrefab, FriendListRoot).GetComponent<FriendItem>();
            friendItem.Init(GetPlayerInfoById(friendId));

            FriendsItemList.Add(friendItem);
        }

        private PlayerInfo GetPlayerInfoById(string friendId)
        {
            PlayerInfo info = new PlayerInfo(); 
            GetPlayerInfoCS getPlayerInfoCS = new GetPlayerInfoCS();
            getPlayerInfoCS.Uuid = friendId;

            SocketManager.Instance.Send(MessageID.GetPlayerInfoCS, getPlayerInfoCS, 
                (data) =>
                    {
                        info = GetPlayerInfoSC.Parser.ParseFrom(data).Info;
                    });

            return info;
        }
    }
}
