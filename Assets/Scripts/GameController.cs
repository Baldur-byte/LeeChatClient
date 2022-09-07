using Manager;
using UnityEngine;
using UnityEngine.UI;
using Protobuf;

public class GameController : MonoSingleton<GameController>
{
    [SerializeField]
    public Transform LoginPanel;
    [SerializeField]
    public Transform RegisterPanel;
    [SerializeField]
    public Transform MainPanel;

    public Player Player { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SocketManager.Instance.Init();
    }
    void Start()
    {
        LoginPanel.gameObject.SetActive(true);
        RegisterPanel.gameObject.SetActive(false);
        MainPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void SwitchPanel(PanelName panel)
    {
        switch (panel)
        {
            case PanelName.Login:
                LoginPanel.gameObject.SetActive(true);
                RegisterPanel.gameObject.SetActive(false);
                MainPanel.gameObject.SetActive(false);
                break;
            case PanelName.Register:
                LoginPanel.gameObject.SetActive(false);
                RegisterPanel.gameObject.SetActive(true);
                MainPanel.gameObject.SetActive(false);
                break;
            case PanelName.Main:
                LoginPanel.gameObject.SetActive(false);
                RegisterPanel.gameObject.SetActive(false);
                MainPanel.gameObject.SetActive(true);
                break;
            default:
                LoginPanel.gameObject.SetActive(true);
                RegisterPanel.gameObject.SetActive(false);
                MainPanel.gameObject.SetActive(false);
                break;
        }
    }

    public void InitPlayer(PlayerInfo info)
    {
        Player = new Player(info);
    }
}
