﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;


public class net : MonoBehaviour {

	//与服务端端套接字
	Socket socket;
	//服务端端IP和端口
	public InputField hostInput;
	public InputField portInput;
	//文本框
	public Text recvText;
	public string recvStr;
    //显示客户端IP和端口
	public Text clientText;
	//聊天输入框
    public InputField textInput;
	//接收缓冲区
	const int BUFFER_SIZE = 1024;
	byte[] readBuff = new byte[BUFFER_SIZE];


	//因为只有主线程能够修改UI组件属性
    //因此在Update里更换文本
    void Update()
    {
        recvText.text = recvStr;
    }

	public void Connetion(){
		//清理text
        recvText.text = "";

		//Socket
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		//Connet
		string host = hostInput.text;
		int port = int.Parse(portInput.text);
		socket.Connect(host, port);
		clientText.text = "客户端地址 " + socket.LocalEndPoint.ToString();

		// //Send
		// string str = "Hello Unity!";
		// byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
		// socket.Send(bytes);

		// //Recv
		// int count = socket.Receive(readBuff);
		// str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
		// recvText.text = str;

		// //Close
		// socket.Close();

		//Recv
        socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
	}

	//接收回调
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            //count是接收数据的大小
            int count = socket.EndReceive(ar);
            //数据处理
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            if (recvStr.Length > 300) recvStr = "";
            recvStr += str + "\n";
            //继续接收	
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch (Exception e)
        {
            recvText.text += "链接已断开";
            socket.Close();
        }
    }

    //发送数据
    public void Send()
    {
        string str = textInput.text;
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        try
        {
            socket.Send(bytes);
        }
        catch { }
    }
	
}
