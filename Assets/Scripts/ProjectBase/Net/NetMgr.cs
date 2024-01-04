using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetMgr : SingletonAutoMono<NetMgr>
{

    //和服务器进行连接的 Socket
    private Socket socket;

    //接受消息用的 缓存容器
    private byte[] cacheBytes = new byte[1024 * 1024];
    private int cacheNum = 0;

    //连接服务器的代码
    public void Connect(string ip, int port)
    {
        if (socket != null && socket.Connected)
            return;

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.RemoteEndPoint = ipPoint;
        args.Completed += (socket, args) =>
        {
            if (args.SocketError == SocketError.Success)
            {
                print("连接成功");
                //收消息
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(cacheBytes, 0, cacheBytes.Length);
                receiveArgs.Completed += ReceiveCallBack;
                this.socket.ReceiveAsync(receiveArgs);
            }
            else
            {
                print("连接失败" + args.SocketError);
                Connect(ip, port);
            }
        };
        socket.ConnectAsync(args);
    }

    //收消息完成的回调函数
    private void ReceiveCallBack(object obj, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            //解析消息 目前用的字符串规则
            print(Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred));
            //继续去收消息
            args.SetBuffer(0, args.Buffer.Length);
            //继续异步收消息
            if (this.socket != null && this.socket.Connected)
                socket.ReceiveAsync(args);
            else
                Close();
        }
        else
        {
            print("接受消息出错" + args.SocketError);
            //关闭客户端连接
            Close();
        }
    }

    public void Close()
    {
        if (socket != null)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);
            socket.Close();
            socket = null;
        }
    }

    public void Send(string str)
    {
        if (this.socket != null && this.socket.Connected)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(bytes, 0, bytes.Length);
            args.Completed += (socket, args) =>
            {
                if (args.SocketError != SocketError.Success)
                {
                    print("发送消息失败" + args.SocketError);
                    Close();
                }

            };
            this.socket.SendAsync(args);
        }
        else
        {
            Close();
        }
    }
}
