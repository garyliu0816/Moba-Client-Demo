using System.Collections;
using System.Net.Sockets;
using Messages.Battle;
using Messages.Type;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [HideInInspector]
    public RoleManager roleManager;

    private int frameId;

    private bool isBattleStart;

    private static BattleController instance;

    public static BattleController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        roleManager = gameObject.AddComponent<RoleManager>();
        roleManager.Initialize();

        frameId = 0;
        isBattleStart = false;
        TcpClient.Instance.handle_battle_start_response = HandleBattleStartResponse;
        TcpClient.Instance.handle_operations_response = HandleOperationsResponse;
        TcpClient.Instance.handle_delta_frames_response = HandleDeltaFramesResponse;
        TcpClient.Instance.handle_game_over_response = HandleGameOverResponse;
        Debug.Log("开始发送战斗准备请求...");
        InvokeRepeating("SendBattleReadyRequest", 0f, 0.2f);
    }

    private void SendBattleReadyRequest()
    {
        BattleReadyRequest request = new BattleReadyRequest();
        request.BattleId = BattleData.Instance.battleId;
        request.BattleUserId = BattleData.Instance.battleUserId;
        Socket clientSocket = TcpClient.Instance.GetClientSocket();
        clientSocket.Send(ProtoBufHelper.PackMessage<BattleReadyRequest>(request, RequestType.BattleReadyRequest));
    }

    private void HandleBattleStartResponse(BattleStartResponse response)
    {
        if (isBattleStart)
        {
            return;
        }
        isBattleStart = true;
        Debug.Log("停止发送战斗准备请求");
        CancelInvoke("SendBattleReadyRequest");
        Debug.Log("开始发送操作...");
        InvokeRepeating("SendOperation", 0f, NetConfig.FRAME_TIME * 0.001f);
        StartCoroutine("WaitForFirstMessage");
    }

    private void SendOperation()
    {
        frameId++;
        OperationRequest request = new OperationRequest();
        request.Operation = BattleData.Instance.selfOperation;
        request.Operation.FrameId = frameId;
        request.BattleId = BattleData.Instance.battleId;
        Socket clientSocket = TcpClient.Instance.GetClientSocket();
        clientSocket.Send(ProtoBufHelper.PackMessage<OperationRequest>(request, RequestType.OperationRequest));
    }

    IEnumerator WaitForFirstMessage()
    {
        yield return new WaitUntil(() =>
        {
            return BattleData.Instance.GetFrameOperationsCount() > 0; // 在这里等待第一帧，第一帧没更新之前不会做更新。
        });
        Debug.Log("开始逻辑更新...");
        InvokeRepeating("LogicUpdate", 0f, 0.02f);
    }

    private void LogicUpdate()
    {
        Operation[] ops;
        if (BattleData.Instance.TryGetNextFrame(out ops))
        {
            roleManager.LogicUpdate(ops);
            BattleData.Instance.UpdateSuccess();
            Debug.Log("更新成功，当前运行到第" + BattleData.Instance.curFrameId + "帧");
        }
    }

    private void HandleOperationsResponse(OperationsResponse response)
    {
        BattleData.Instance.UpdateFrameOperations(response.FrameId, response.Operations.ToArray());
    }

    private void HandleDeltaFramesResponse(DeltaFramesResponse response)
    {
        if (response.DeltaFrames.Count > 0)
        {
            foreach (var item in response.DeltaFrames)
            {
                BattleData.Instance.FillMissingFrameOperations(item.FrameId, item.Operations.ToArray());
            }
        }
    }

    public void OnClickGameOver()
    {
        CancelInvoke("SendOperation");
        InvokeRepeating("SendGameOver", 0f, 0.5f);
    }

    private void SendGameOver(int battleUserId)
    {
        GameOverRequest request = new GameOverRequest();
        request.BattleUserId = battleUserId;
        Socket clientSocket = TcpClient.Instance.GetClientSocket();
        clientSocket.Send(ProtoBufHelper.PackMessage<GameOverRequest>(request, RequestType.GameOverRequest));
    }

    private void HandleGameOverResponse(GameOverResponse response)
    {
        CancelInvoke("SendGameOver");
    }

    private void OnDestroy()
    {
        BattleData.Instance.Clear();
        TcpClient.Instance.Destroy();
        instance = null;
    }
}