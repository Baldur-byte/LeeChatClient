using Protobuf;
using System.Collections.Generic;

public class Player
{
    PlayerInfo playerInfo;
    PlayerState playerState;
    List<string> friendList = new List<string>();

    public Player()
    {
        playerInfo = new PlayerInfo();
    }


    public Player(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
        this.playerState = PlayerState.Online;
    }

    public void AddFriend(List<string> friendList)
    {
        foreach(string id in friendList)
        {
            if (isFriendExist(id)) this.friendList.Add(id);
        }
    }

    public void AddFriend(string friendId)
    {
        if (isFriendExist(friendId)) this.friendList.Add(friendId);
    }

    public bool isFriendExist(string friendId)
    {
        return friendList.Contains(friendId);
    }
    
    public string Name() { return playerInfo.Name; }

    public string Uuid() { return playerInfo.Uuid; }

    public PlayerState State() { return playerState; }

    public void Offline() { playerState = PlayerState.Offline; }

    public List<string> Friends() { return friendList; }
}

public enum PlayerState
{
    Online,
    Offline,
}