using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using Messages.Battle;
using Messages.Type;
using static Messages.Battle.Operation;

class BattleData : Singleton<BattleData>
{
    public int randomSeed;
    public int battleId;
    public int battleUserId;
    public Operation selfOperation; // 该玩家的操作，将会上传至服务器
    public List<BattleUserInfo> userInfos;

    public int curFrameId;
    private int maxFrameId;
    private int maxSendNum;
    private Dictionary<int, int> playerCurFrameId; // 每个玩家执行到多少帧
    private Dictionary<int, Operation[]> frameOperations; // 每帧所有玩家的操作，通过服务器同步
    private List<int> missingFrames;

    protected override void Initialize() // 自动调用
    {
        curFrameId = 0;
        maxFrameId = 0;
        maxSendNum = 5;
        selfOperation = new Operation();
        ResetSelfOperation();
        playerCurFrameId = new Dictionary<int, int>();
        frameOperations = new Dictionary<int, Operation[]>();
        missingFrames = new List<int>();
    }

    public void Initialize(int _randomSeed, int _battleId, List<BattleUserInfo> _userInfos) // 手动调用
    {
        randomSeed = _randomSeed;
        battleId = _battleId;
        userInfos = new List<BattleUserInfo>(_userInfos);
        foreach (var item in userInfos)
        {
            playerCurFrameId[item.BattleUserId] = 0;
            if (item.UserId == GameManager.Instance.userId)
            {
                battleUserId = item.BattleUserId;
                selfOperation.UserId = item.UserId;
                selfOperation.BattleUserId = battleUserId;
            }
        }
    }

    public void Clear()
    {
        ResetSelfOperation();
        curFrameId = 0;
        maxFrameId = 0;
        maxSendNum = 0;
        playerCurFrameId.Clear();
        frameOperations.Clear();
        missingFrames.Clear();
    }

    public override void Destroy()
    {
        userInfos.Clear();
        userInfos = null;
        base.Destroy();
    }

    public void UpdateSelfOperation(OperationType type)
    {
        selfOperation.operation_type = type;
        selfOperation.FrameId = curFrameId;
    }

    public bool IsValidOperation(int _battleUserId, int frameId)
    {
        return frameId > playerCurFrameId[_battleUserId];
    }

    public void UpdateFrameId(int _battleUserId, int frameId, OperationType type)
    {
        playerCurFrameId[_battleUserId] = frameId; // 更新玩家执行的帧数
        if (battleUserId == _battleUserId)
        {
            curFrameId++;
            if (type == selfOperation.operation_type)
            {
                ResetSelfOperation();
            }
        }
    }

    private void ResetSelfOperation()
    {
        selfOperation.operation_type = OperationType.Idle;
        selfOperation.FrameId = 0;
    }

    public int GetFrameOperationsCount()
    {
        if (frameOperations == null)
        {
            return 0;
        }
        return frameOperations.Count;
    }

    public void UpdateFrameOperations(int frameId, Operation[] ops)
    {
        frameOperations[frameId] = ops;
        for (int i = maxFrameId + 1; i < frameId; i++)
        {
            missingFrames.Add(i);
        }
        maxFrameId = frameId;
        if (missingFrames.Count > 0)
        {
            if (missingFrames.Count > maxSendNum)
            {
                DeltaFramesRequest request = new DeltaFramesRequest();
                request.BattleId = battleId;
                request.Frames = missingFrames.GetRange(0, maxSendNum).ToArray();
                Socket clientSocket = TcpClient.Instance.GetClientSocket();
                clientSocket.Send(ProtoBufHelper.PackMessage<DeltaFramesRequest>(request, RequestType.DeltaFramesRequest));
            }
            else
            {
                DeltaFramesRequest request = new DeltaFramesRequest();
                request.BattleId = battleId;
                request.Frames = missingFrames.ToArray();
                Socket clientSocket = TcpClient.Instance.GetClientSocket();
                clientSocket.Send(ProtoBufHelper.PackMessage<DeltaFramesRequest>(request, RequestType.DeltaFramesRequest));
            }
        }
    }

    public void FillMissingFrameOperations(int frameId, Operation[] ops)
    {
        if (missingFrames.Contains(frameId))
        {
            frameOperations[frameId] = ops;
            missingFrames.Remove(frameId);
        }
    }

    public bool TryGetNextFrame(out Operation[] ops)
    {
        return frameOperations.TryGetValue(curFrameId + 1, out ops);
    }

    public void UpdateSuccess()
    {
        curFrameId++;
    }
}