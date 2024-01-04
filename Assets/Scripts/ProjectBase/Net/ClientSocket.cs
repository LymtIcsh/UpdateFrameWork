using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientSocket
{
    public Socket socket;
    public int clientID;
    private static int CLIENT_BEGIN_ID = 1;

    private byte[] cacheBytes = new byte[1024];
    private int cacheNum = 0;

    public ClientSocket(Socket socket)
    {
        this.clientID = CLIENT_BEGIN_ID++;
        this.socket = socket;

        //开始收消息
        this.socket.BeginReceive(cacheBytes, cacheNum, cacheBytes.Length, SocketFlags.None, ReceiveCallBack, null);
    }

    private void ReceiveCallBack(IAsyncResult result)
    {
        try
        {
            cacheNum = this.socket.EndReceive(result);
            //通过字符串去解析
            Debug.Log(Encoding.UTF8.GetString(cacheBytes, 0, cacheNum));
            //如果是连接状态再继续收消息
            //因为目前我们是以字符串的形式解析的 所以 解析完 就直接 从0又开始收
            cacheNum = 0;
            if (this.socket.Connected)
                this.socket.BeginReceive(cacheBytes, cacheNum, cacheBytes.Length, SocketFlags.None, ReceiveCallBack, this.socket);
            else
            {
                Debug.Log("没有连接，不用再收消息了");
            }
        }
        catch (SocketException e)
        {
            Debug.Log("接受消息错误" + e.SocketErrorCode + e.Message);
        }
    }

    public void Send(string str)
    {
        if (this.socket.Connected)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            this.socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendCallBack, null);
        }
        else
        {

        }
    }

    private void SendCallBack(IAsyncResult result)
    {
        try
        {
            this.socket.EndSend(result);
        }
        catch (SocketException e)
        {
            Debug.Log("发送失败" + e.SocketErrorCode + e.Message);
        }
    }

}
