using Protobuf;

public class Player
{
    PlayerInfo playerInfo;
    PlayerState playerState;

    public Player(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
        this.playerState = PlayerState.Online;
    }

    public string Name() { return playerInfo.Name; }

    public string Uuid() { return playerInfo.Uuid; }

    public PlayerState State() { return playerState; }

    public void Offline() { playerState = PlayerState.Offline; }
}

public enum PlayerState
{
    Online,
    Offline,
}