using System.Net.Sockets;
using Messages.Battle;
using Messages.Match;
using Messages.Type;
using UnityEngine;

public class MatchController : MonoBehaviour
{

    void Start()
    {
        GameObject sceneManagerObj = new GameObject("SceneManager");
        SceneManager.CreateInstance(sceneManagerObj);
        TcpClient.Instance.handle_match_response = HandleMatchResponse;
        TcpClient.Instance.handle_cancel_match_response = HandleCancelMatchResponse;
        TcpClient.Instance.handle_enter_battle_response = HandleEnterBattleResponse;
    }

    void OnDestroy()
    {
        TcpClient.Instance.handle_match_response = null;
        TcpClient.Instance.handle_cancel_match_response = null;
        TcpClient.Instance.handle_enter_battle_response = null;
    }

    public void OnClickMatch()
    {
        //开始匹配 
        MatchRequest request = new MatchRequest();
        request.UserId = GameManager.Instance.userId;
        request.RoleId = 1; // 暂时不能选择角色
        Socket clientSocket = TcpClient.Instance.GetClientSocket();
        clientSocket.Send(ProtoBufHelper.PackMessage<MatchRequest>(request, RequestType.MatchRequest));
    }

    public void OnClickCancelMatch()
    {
        //取消匹配
        CancelMatchRequest request = new CancelMatchRequest();
        request.UserId = GameManager.Instance.userId;
        Socket clientSocket = TcpClient.Instance.GetClientSocket();
        clientSocket.Send(ProtoBufHelper.PackMessage<CancelMatchRequest>(request, RequestType.CancelMatchRequest));
    }

    void HandleMatchResponse(MatchResponse response)
    {

    }

    void HandleCancelMatchResponse(CancelMatchResponse response)
    {

    }

    // 进入战场
    void HandleEnterBattleResponse(EnterBattleResponse response)
    {
        Debug.Log("初始化战场数据...");
        BattleData.Instance.Initialize(response.RandomSeed,response.BattleId, response.BattleUserInfos);
        Debug.Log("初始化战场数据完毕，进入战斗!");
        SceneManager.LoadScene("BattleScene");
    }
}
