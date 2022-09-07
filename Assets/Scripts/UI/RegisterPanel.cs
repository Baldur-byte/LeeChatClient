using Manager;
using Protobuf;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : MonoBehaviour
{
    public InputField username;
    public InputField uuid;
    public InputField password;
    public InputField passwordCheck;

    public Button Register;
    public Button Login;

    public Text Message;

    void Start()
    {
        Register.onClick.AddListener(RegisterOnClick);
        Login.onClick.AddListener(LoginOnClick);

        Message.gameObject.SetActive(false);
    }

    private void RegisterOnClick()
    {
        if(string.IsNullOrEmpty(username.text) || string.IsNullOrEmpty(uuid.text))
        {
            Message.text = "��Ϣ��д������";
            Message.gameObject.SetActive(true);
            return;
        }

        if (string.IsNullOrEmpty(password.text))
        {
            Message.text = "���������룡";
            Message.gameObject.SetActive(true);
            return;
        }

        if (string.IsNullOrEmpty(passwordCheck.text))
        {
            Message.text = "��ȷ�����룡";
            Message.gameObject.SetActive(true);
            return;
        }

        if (!password.text.Equals(passwordCheck.text))
        {
            Message.text = "����������д��һ�£�";
            Message.gameObject.SetActive(true);
            return;
        }

        RegisterCS registerCS = new RegisterCS();
        registerCS.Name = username.text;
        registerCS.Uuid = uuid.text;
        registerCS.Password = password.text;

        SocketManager.Instance.Send(MessageID.RegisterCS, registerCS, (data) =>
        {
            RegisterSC registerSC = new RegisterSC();
            registerSC = NetworkUtils.GetProto(registerSC, data) as RegisterSC;

            if (!registerSC.Result)
            {
                Message.text = "���û��Ѵ��ڣ�";
                Message.gameObject.SetActive(true);
                return;
            }

            GameController.Instance.InitPlayer(registerSC.Info);
            GameController.Instance.SwitchPanel(PanelName.Main);
        });
    }

    private void LoginOnClick()
    {
        GameController.Instance.SwitchPanel(PanelName.Login);
    }
}
