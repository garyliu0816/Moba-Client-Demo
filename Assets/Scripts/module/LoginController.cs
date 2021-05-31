using System.Net.Sockets;
using Messages.Login;
using Messages.Type;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public InputField usernameField;
    public InputField passwordField;
    void Start()
    {
        NetManager.GetInstance();
        GameObject sceneManagerObj = new GameObject("SceneManager");
        SceneManager.CreateInstance(sceneManagerObj);
        Application.targetFrameRate = 30;
        TcpClient.Instance.handle_login_response = HandleLoginResponse;
    }

    void OnDestroy()
    {
        TcpClient.Instance.handle_login_response = null;
    }

    public void OnClickLogin()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        TcpClient.Instance.ConnectServer((isConnected) =>
        {
            if (isConnected)
            {
                Debug.Log("连接成功");
                LoginRequest request = new LoginRequest();
                request.Token = SystemInfo.deviceUniqueIdentifier;
                request.Username = username;
                request.Password = password;
                // 连接成功后发送消息
                Debug.Log("向服务器发送登陆请求...");
                Socket clientSocket = TcpClient.Instance.GetClientSocket();
                clientSocket.Send(ProtoBufHelper.PackMessage<LoginRequest>(request, RequestType.LoginRequest));
            }
            else
            {
                Debug.Log("连接失败");
            }
        });
    }

    void HandleLoginResponse(LoginResponse response)
    {
        if (response.Result)
        {
            GameManager.Instance.userId = response.UserId;
            // 切换场景
            SceneManager.LoadScene("MatchScene");
        }
        else
        {
            Debug.Log("登陆失败，用户名或密码出错");
        }
    }
}