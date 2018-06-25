using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//添加用于socket的类
using System.Net;
using System.Net.Sockets;

namespace socket_client
{
    class Program
    {
        private static string myhostdc = "";
        private static string otherhostdc = "";
        private static string sendMsg = "";
        private static string MsgId = "";
        private static string flag = "again";
        static void Main(string[] args)
        {
            string ipstr ="";
            string portnumber="";
            otherhostdc="";
            myhostdc="";
            do
            {
                do
                {
                    Console.Write("请输入IP地址:");
                    ipstr = Console.ReadLine();
                } while (string.IsNullOrEmpty(ipstr));
                
                do
                {
                    Console.Write("请输入端口号:");
                    portnumber = Console.ReadLine();
                } while (string.IsNullOrEmpty(portnumber));

                do
                {
                    Console.Write("请输入对方机器计算机代号:");
                    otherhostdc = Console.ReadLine().ToUpper();
                } while (string.IsNullOrEmpty(otherhostdc));

                do
                {
                    Console.Write("请输入本机计算机代号:");
                    myhostdc = Console.ReadLine().ToUpper();
                } while (string.IsNullOrEmpty(myhostdc));
                
                do
                {
                    try
                    {
                        int port = Convert.ToInt32(portnumber);//端口号
                        string host = ipstr;//IP地址

                        if (!string.IsNullOrEmpty(host))
                        {
                            if (port != 0)
                            {
                                IPAddress ip = IPAddress.Parse(host);
                                IPEndPoint ipe = new IPEndPoint(ip, port);

                                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                Console.WriteLine("Connecting...");
                                c.Connect(ipe);
                                if (!c.Connected)
                                {
                                    Console.WriteLine("连接超时");
                                }
                                else
                                {
                                    Console.WriteLine("连接成功");
                                }

                                if (c.Connected)
                                {
                                    do
                                    {
                                        Console.Write("请输入电文号：");
                                        MsgId = Console.ReadLine().ToUpper();
                                    } while (string.IsNullOrEmpty(MsgId));

                                    do
                                    {
                                        Console.Write("请输入想发送的内容：");
                                        sendMsg = Console.ReadLine();
                                    } while (string.IsNullOrEmpty(sendMsg));

                                    string sendStr = OrgMsg(sendMsg);
                                    Console.WriteLine("Send message:{0}", sendStr);
                                    byte[] bsTemp = Encoding.ASCII.GetBytes(sendStr);   //把字符串编码为字节
                                    int byteLen = bsTemp.Length;
                                    int realLen = bsTemp.Length + 1;
                                    byte[] bs = new byte[realLen];
                                    for (int i = 0; i < bsTemp.Length; i++)
                                    {
                                        bs[i] = bsTemp[i];
                                    }
                                    bs[byteLen] = 0x0a;

                                    Console.WriteLine("Send message[BYTE]:");
                                    for (int i = 0; i < realLen; i++)
                                    {
                                        if ((i % 5 == 0)||(i==realLen-1))
                                        {
                                            Console.Write(bs[i].ToString() + "\n");
                                        }
                                        else
                                        {
                                            Console.Write(bs[i].ToString() + " ");
                                        }

                                        
                                    }
                                    c.Send(bs, bs.Length, 0); //发送信息

                                    //接受从服务器返回的信息
                                    string recvStr = "";
                                    byte[] recvBytes = new byte[1024];
                                    int bytes;
                                    bytes = c.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息
                                    recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);

                                    if (recvStr.Substring(28, 1).CompareTo("A") == 0)
                                    {
                                        Console.WriteLine("发送成功");    //回显服务器的返回信息
                                    }
                                    else
                                    {
                                        Console.WriteLine("client get message:{0}", recvStr);    //回显服务器的返回信息
                                    }

                                    Console.ReadLine();
                                    //一定记着用完Socket后要关闭
                                    c.Close();
                                }
                            }
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("ArgumentException:{0}", e);
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("SocketException:{0}", e);
                    }
                    Console.Write("是否需要重设连接（Y/N）:");
                    string result = Console.ReadLine().ToLower();
                    if (result.CompareTo("y") == 0)
                    {
                        flag = "reset";
                    }
                    else
                    {
                        flag = "again";
                    }
                } while (flag == "again");
            } while (flag == "reset");
        }

        private static string OrgMsg(string content)
        {
            string Msg = "";
            int MsgLength = content.Length + 30;
            string strMsgLen = MsgLength.ToString();
            if(strMsgLen.Length<4)
            {
                strMsgLen=strMsgLen.PadLeft(4,'0');
            }
            string time = System.DateTime.Now.ToString("yyyyMMddhhmmss");
            string function = "D";

            Msg = strMsgLen + MsgId + time + myhostdc + otherhostdc + function + content;
            return Msg;
        }
    }
}
