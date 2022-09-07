using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Network
{
    public delegate void CallBack(byte[] data);

    public static class SocketClient
    {
        private class NetworkCoroutine : MonoSingleton<NetworkCoroutine>
        {
            private event Action ApplicatonQuitEvent;

            public void SetQuitEvent(Action func)
            {
                if (ApplicatonQuitEvent != null)
                    return;
                ApplicatonQuitEvent += func;
            }

            private void OnApplicationQuit()
            {
                if (ApplicatonQuitEvent != null)
                    ApplicatonQuitEvent();
            }
        }

        private enum ClientState
        {
            Connected,
            Disconnected,
        }

        private static Dictionary<MessageID, CallBack> _callBacks = new Dictionary<MessageID, CallBack>();

        private static ConcurrentQueue<byte[]> _messages;

        private static ClientState _state = ClientState.Disconnected;

        private static TcpClient _client;

        private static NetworkStream _stream;

        private static IPAddress _address;

        private static int _port;

        private static byte[] _sendBuffer = new byte[16];

        //心跳包机制
        private const float HEARTBEAT_TIME = 3;

        private static float _timer = HEARTBEAT_TIME;

        public static bool Received = true;

        private static IEnumerator _Connect()
        {
            _client = new TcpClient();

            //异步连接
            IAsyncResult async = _client.BeginConnect(_address, _port, null, null);
            while (!async.IsCompleted)
            {
                Debug.Log("服务器连接中.....");
                yield return null;
            }

            //异常处理
            try
            {
                _client.EndConnect(async);
            }
            catch (Exception ex)
            {
                Debug.LogError("服务器连接失败：" + ex.Message);
                yield break;
            }

            try
            {
                _stream = _client.GetStream();
            }
            catch (Exception ex)
            {
                Debug.LogError("获取数据流失败: " + ex.Message);
                yield break;
            }

            if (_stream == null)
            {
                Debug.LogError("服务器连接失败: 数据流为空");
                yield break;
            }

            _state = ClientState.Connected;
            _messages = new ConcurrentQueue<byte[]>();
            Debug.Log("服务器连接成功！");

            NetworkCoroutine.Instance.StartCoroutine(_Send());
            NetworkCoroutine.Instance.StartCoroutine(_Receive());
            NetworkCoroutine.Instance.SetQuitEvent(() =>
            {
                _client.Close();
                _state = ClientState.Disconnected;
            });
        }

        private static IEnumerator _Send()
        {
            while(_state == ClientState.Connected)
            {
                _timer += Time.deltaTime;

                if(_messages.Count > 0)
                {
                    _messages.TryDequeue(out _sendBuffer);
                    yield return _Write(_sendBuffer);
                }

                //心跳包机制
                if(_timer >= HEARTBEAT_TIME)
                {
                    if (!Received)
                    {
                        _state = ClientState.Disconnected;
                        Debug.LogError("心跳包接收失败，断开连接");
                        yield break;
                    }
                    _timer = 0;

                    _sendBuffer = NetworkUtils.Pack(MessageID.Ping);

                    yield return _Write(_sendBuffer);

                    Debug.Log("已经发送心跳包");
                    Received = false;
                }
                yield return null;
            }
        }

        private static IEnumerator _Receive()
        {
            Debug.Log("开始接收消息");
            //持续接受消息
            while (_state == ClientState.Connected)
            {
                //解析数据包过程
                //客户端与服务器按照一定协议指定数据包
                //消息ID（int 4字节）
                //服务器时间(long8字节)
                //消息长度 (4字节) 
                byte[] data = new byte[16];

                int length; //消息长度
                int receive = 0; //接收长度
                MessageID id; // 消息ID
                byte type; //消息类型
                long timestamep;  //时间戳

                //异步读取
                IAsyncResult async = _stream.BeginRead(data, 0, data.Length, null, null);
                while (!async.IsCompleted)
                    yield return null;


                //异常处理
                try
                {
                    receive = _stream.EndRead(async);
                }
                catch (Exception ex)
                {
                    _state = ClientState.Disconnected;
                    Debug.LogError("消息包头接受失败（异步读取）：" + ex.Message);
                }

                if (receive < data.Length)
                {
                    _state = ClientState.Disconnected;
                    Debug.LogError("消息包头接受失败（receive < data.Length）");
                    yield break;
                }

                using (MemoryStream stream = new MemoryStream(data))
                {
                    BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
                    try
                    {
                        //解析包头
                        id = (MessageID)reader.ReadInt32();
                        timestamep = reader.ReadInt64();
                        length = reader.ReadInt32();
                        Debug.LogFormat("解析包头结果：消息ID：{0}，时间戳：{1}，长度：{2}", id, timestamep, length);
                    }
                    catch (Exception ex)
                    {
                        _state = ClientState.Disconnected;
                        Debug.LogError("消息包头接受失败（解析包头失败）：" + ex.Message);
                        yield break;
                    }
                }

                //如果有包体
                if (length > 0)
                {
                    data = new byte[length];
                    //异步读取
                    async = _stream.BeginRead(data, 0, data.Length, null, null);
                    while (!async.IsCompleted)
                    {
                        yield return null;
                    }
                    //异常处理
                    try
                    {
                        receive = _stream.EndRead(async);
                    }
                    catch (Exception ex)
                    {
                        _state = ClientState.Disconnected;
                        Debug.LogError("消息包体接受失败（异步读取失败）：" + ex.Message);
                        yield break;
                    }
                    if (receive < data.Length)
                    {
                        _state = ClientState.Disconnected;
                        Debug.LogError("消息包体接受失败（receive < data.Length）");
                        yield break;
                    }

                }
                //没有包体
                else
                {
                    Debug.Log("没有包体");
                    data = new byte[0];
                    receive = 0;
                }

                Debug.Log("消息接收成功，消息id" + id);

                if (_callBacks.ContainsKey(id))
                {
                    //执行回调事件
                    CallBack callBack = _callBacks[id];
                    callBack(data);
                    Debug.Log("执行回调事件");
                }
                else
                {
                    Debug.LogError("未注册该类型的回调事件");
                }
            }
        }

        private static IEnumerator _Write(byte[] data)
        {
            //如果服务器下线，客户端依然会继续发送消息
            if (_state != ClientState.Connected || _stream == null)
            {
                Debug.LogError("连接失败无法发送消息");
                yield break;
            }

            //异步发送消息
            IAsyncResult async = _stream.BeginWrite(data, 0, data.Length, null, null);
            while (!async.IsCompleted)
                yield return null;
            //异常处理
            try
            {
                _stream.EndWrite(async);
            }
            catch (Exception ex)
            {
                _state = ClientState.Disconnected;
                Debug.LogError("发送消息失败: " + ex.Message);
            }
        }

        //连接服务器公有方法
        public static void Connect(string address = null, int port = 8848)
        {
            if(_state == ClientState.Connected)
            {
                Debug.LogError("服务器已经连接");
                return;
            }

            if (address == null)
            {
                Debug.LogError("IP地址为空");
                return;
            }

            //获取失败则取消连接
            if (!IPAddress.TryParse(address, out _address))
            {
                Debug.LogError("IP地址错误，请重新尝试");
                return;
            }

            _port = port;
            //与服务器建立连接  
            //连接ip跟端口号成功不保证网络流建立成功
            NetworkCoroutine.Instance.StartCoroutine(_Connect());
        }

        //注册消息回调事件
        public static void Register(MessageID id, CallBack method)
        {
            if (!_callBacks.ContainsKey(id))
                _callBacks.Add(id, method);
            else
                Debug.LogError("注册相同的回调事件");
        }

        //加入消息队列
        public static void Enqueue(MessageID id, byte[] data = null)
        {
            //把数据封装
            byte[] bytes = NetworkUtils.Pack(id, data);

            if (_state == ClientState.Connected)
                //加入队列
                _messages.Enqueue(bytes);
        }
    }
}
