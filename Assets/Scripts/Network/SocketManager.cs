using Google.Protobuf;
using Network;
using Protobuf;
using UnityEngine;

namespace Manager
{
    public class SocketManager : MonoSingleton<SocketManager>
    {
        public void Init()
        {
            SocketClient.Register(MessageID.Pong, MessageEvent._Pong);
            SocketClient.Register(MessageID.ErrorCode, MessageEvent._ErrorCode);
            SocketClient.Connect("192.168.159.1");
        }

        public void Send(MessageID id, IMessage message, CallBack callBack = null)
        {
            if (callBack != null) SocketClient.Register(id + 1, callBack);
            SocketClient.Enqueue(id, message.ToByteArray());
        }

        #region 特殊事件
        //心跳
        public static void _Pong(byte[] bytes)
        {
            SocketClient.Received = true;
            Debug.Log("收到心跳包回应");
        }
        //主动推送错误码消息
        public static void _ErrorCode(byte[] bytes)
        {
            ErrorCode errorCode = new ErrorCode();
            errorCode = NetworkUtils.GetProto(errorCode, bytes) as ErrorCode;

            Debug.Log("错误码: " + errorCode.Code);
        }
        #endregion
    }
}
