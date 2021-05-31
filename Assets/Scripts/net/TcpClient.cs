using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Messages.Battle;
using Messages.Login;
using Messages.Match;
using Messages.Type;
using UnityEngine;

class TcpClient : Singleton<TcpClient>
{
    private Socket clientSocket;

    bool isRunning;

    private Action<bool> connect_result_action; // 返回客户端是否成功连接服务器

    public delegate void MessageHandleDelegate<T>(T message); // 将Message的处理委托给具体的Controller
    public MessageHandleDelegate<LoginResponse> handle_login_response { get; set; }
    public MessageHandleDelegate<MatchResponse> handle_match_response { get; set; }
    public MessageHandleDelegate<CancelMatchResponse> handle_cancel_match_response { get; set; }
    public MessageHandleDelegate<EnterBattleResponse> handle_enter_battle_response { get; set; }
    public MessageHandleDelegate<BattleStartResponse> handle_battle_start_response { get; set; }
    public MessageHandleDelegate<OperationsResponse> handle_operations_response { get; set; }
    public MessageHandleDelegate<DeltaFramesResponse> handle_delta_frames_response { get; set; }
    public MessageHandleDelegate<GameOverResponse> handle_game_over_response { get; set; }

    protected override void Initialize()
    {
        isRunning = false;
    }

    public void ConnectServer(Action<bool> action)
    {
        connect_result_action = action;
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.BeginConnect(IPAddress.Parse(NetConfig.SERVER_IP), NetConfig.SERVER_PORT, RequestConnectCallBack, clientSocket);
    }

    private void RequestConnectCallBack(IAsyncResult result)
    {
        try
        {
            Socket mClientSocket = (Socket)result.AsyncState;
            mClientSocket.EndConnect(result);

            isRunning = true;
            // 网络请求要放入主线程
            NetManager.GetInstance().AddAction(() =>
            {
                if (connect_result_action != null)
                {
                    connect_result_action(true);
                }
            });
            new Thread(ReceiveMessage).Start();
        }
        catch (Exception e)
        {
            NetManager.GetInstance().AddAction(() =>
            {
                if (connect_result_action != null)
                {
                    connect_result_action(false);
                }
            });
            Debug.Log(e.Message);
        }
    }

    private void ReceiveMessage()
    {
        while (isRunning)
        {
            try
            {
                byte[] data = new byte[1024];
                int size = clientSocket.Receive(data);
                ResponseType responseType = (ResponseType)data[PackageConstant.TypeOffset];
                Int16 dataLength = BitConverter.ToInt16(data, PackageConstant.LengthOffset);
                int bodyLength = dataLength - PackageConstant.HeadLength;
                byte[] body = new byte[bodyLength];
                Array.Copy(data, PackageConstant.HeadLength, body, 0, bodyLength);

                AnalyzeMessage(responseType, body);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    private void AnalyzeMessage(ResponseType responseType, byte[] body)
    {
        switch (responseType)
        {
            case ResponseType.LoginResponse:
                {
                    LoginResponse response = ProtoBufHelper.Deserialize<LoginResponse>(body);
                    NetManager.GetInstance().AddAction(() =>
                    {
                        handle_login_response(response);
                    });
                }
                break;
            case ResponseType.MatchResponse:
                {
                    MatchResponse response = ProtoBufHelper.Deserialize<MatchResponse>(body);
                    NetManager.GetInstance().AddAction(() =>
                    {
                        handle_match_response(response);
                    });
                }
                break;
            case ResponseType.CancelMatchResponse:
                {
                    CancelMatchResponse response = ProtoBufHelper.Deserialize<CancelMatchResponse>(body);
                    NetManager.GetInstance().AddAction(() =>
                    {
                        handle_cancel_match_response(response);
                    });
                }
                break;
            case ResponseType.EnterBattleResponse:
                {
                    EnterBattleResponse response = ProtoBufHelper.Deserialize<EnterBattleResponse>(body);
                    NetManager.GetInstance().AddAction(() =>
                    {
                        handle_enter_battle_response(response);
                    });
                }
                break;
            case ResponseType.BattleStartResponse:
                {
                    BattleStartResponse response = ProtoBufHelper.Deserialize<BattleStartResponse>(body);
                    NetManager.GetInstance().AddAction(() =>
                    {
                        handle_battle_start_response(response);
                    });
                }
                break;
            case ResponseType.OperationsResponse:
                {
                    OperationsResponse response = ProtoBufHelper.Deserialize<OperationsResponse>(body);
                    NetManager.GetInstance().AddAction(() =>
                    {
                        handle_operations_response(response);
                    });
                }
                break;
            case ResponseType.DeltaFramesResponse:
                {
                    DeltaFramesResponse response = ProtoBufHelper.Deserialize<DeltaFramesResponse>(body);
                    NetManager.GetInstance().AddAction(() =>
                    {
                        handle_delta_frames_response(response);
                    });
                }
                break;
            case ResponseType.GameOverResponse:
                {
                    GameOverResponse response = ProtoBufHelper.Deserialize<GameOverResponse>(body);
                    NetManager.GetInstance().AddAction(() =>
                    {
                        handle_game_over_response(response);
                    });
                }
                break;
            default:
                break;
        }
    }

    public void DisConnectServer()
    {
        if (isRunning == false)
        {
            return;
        }
        isRunning = false;
        try
        {
            clientSocket.Close();
            clientSocket = null;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public Socket GetClientSocket()
    {
        return clientSocket;
    }
}