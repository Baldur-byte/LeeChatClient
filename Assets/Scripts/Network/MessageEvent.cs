using Protobuf;
using UnityEditor.PackageManager;
using UnityEngine;
using ErrorCode = Protobuf.ErrorCode;

namespace Network
{
    public static class MessageEvent
    {
        #region BaseMessage
        //登陆
        public static void _Login(byte[] bytes)
        {
            LoginSC login = new LoginSC();
            login = NetworkUtils.GetProto(login, bytes) as LoginSC;

            Debug.LogFormat("登录返回：name:{0},  id:{1},  iconURL:{2}", login.Info.Name, login.Info.Uuid, login.Info.IconUrl);
        }

        //心跳
        public static void _Pong(byte[] bytes)
        {
            SocketClient.Received = true;
            Debug.Log("收到心跳包回应");
        }
        //断线重连
        public static void _Connection(byte[] bytes)
        {
            Debug.Log("断线重连");
        }
        #endregion

        #region CommMessage
        //主动推送错误码消息
        public static void _ErrorCode(byte[] bytes)
        {
            ErrorCode errorCode = new ErrorCode();
            errorCode = NetworkUtils.GetProto(errorCode, bytes) as ErrorCode;

            Debug.Log("错误码: " + errorCode.Code);
        }
        #endregion

        #region MainMessage
        public static void _SendToOne(byte[] bytes)
        {
            SendToOne sendToOne = new SendToOne();
            sendToOne = NetworkUtils.GetProto(sendToOne, bytes) as SendToOne;

            Debug.LogFormat("One消息来自: {0} 发送给: {1} 内容为: {2}" + sendToOne.FromId, sendToOne.ToId, sendToOne.Message);
        }

        public static void _SendToGroup(byte[] bytes)
        {
            SendToGroup sendToGroup = new SendToGroup();
            sendToGroup = NetworkUtils.GetProto(sendToGroup, bytes) as SendToGroup;

            Debug.LogFormat("Group消息来自: {0} 发送给: {1} 内容为: {2}" + sendToGroup.FromId, sendToGroup.ToRoomId, sendToGroup.Message);
        }
        #endregion

        #region PlayerMessage
        //更新角色信息
        public static void _UpdateRoleInfo(byte[] bytes)
        {
            Debug.Log("更新角色信息返回");
        }
        #endregion
    }
}
