using Manager;
using UnityEngine;
using UnityEngine.UI;
using Protobuf;
using UnityEditor.VersionControl;

public class LoginPanel : MonoBehaviour
{
    public InputField userid;
    public InputField password;

    public Button Register;
    public Button Login;
    public Button Exit;

    public Text Message;

    void Start()
    {
        Register.onClick.AddListener(RegisterOnClick);
        Login.onClick.AddListener(LoginOnClick);
        Exit.onClick.AddListener(ExitOnClick);

        Message.gameObject.SetActive(false);
    }

    private void RegisterOnClick()
    {
        GameController.Instance.SwitchPanel(PanelName.Register);
    }

    private void LoginOnClick()
    {
        if (string.IsNullOrEmpty(userid.text) || string.IsNullOrEmpty(password.text))
        {
            Message.text = "请输入用户名和密码";
            Message.gameObject.SetActive(true);
            return;
        }
        LoginCS loginCS = new LoginCS();
        loginCS.Uuid = userid.text;
        loginCS.Password = password.text;
        SocketManager.Instance.Send(MessageID.LoginCS, loginCS, (data) =>
        {
            LoginSC loginSC = new LoginSC();
            loginSC = NetworkUtils.GetProto(loginSC, data) as LoginSC;
            
            if (!loginSC.Result) 
            { 
                Message.text = "用户名或者密码不正确";
                Message.gameObject.SetActive(true);
                return;
            }

            GameController.Instance.InitPlayer(loginSC.Info);
            GameController.Instance.SwitchPanel(PanelName.Main);
        });
    }

    private void ExitOnClick()
    {
        Application.Quit();
        Debug.Log("程序退出");
    }
}
