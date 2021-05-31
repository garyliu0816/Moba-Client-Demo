using System.Collections.Generic;
using Messages.Battle;
using UnityEngine;

public class RoleManager : MonoBehaviour
{
    private GameObject rolePrefab;
    private Dictionary<int, RoleController> roleControllers;

    public void Initialize()
    {
        rolePrefab = Resources.Load<GameObject>("Prefabs/Cube");
        roleControllers = new Dictionary<int, RoleController>();
        List<BattleUserInfo> userInfos = BattleData.Instance.userInfos;
        Vector3 spawnPoint = new Vector3(0, 1.1f, 0);
        for (int i = 0; i < userInfos.Count; i++)
        {
            BattleUserInfo info = userInfos[i];
            GameObject role = Instantiate(rolePrefab); // 由于就一种角色，只能是这个
            RoleController roleController = role.GetComponent<RoleController>();
            roleController.Initialize(spawnPoint);
            spawnPoint = spawnPoint + new Vector3(2, 0, 0);
            roleControllers[info.BattleUserId] = roleController;
        }
    }

    private void FixedUpdate() // 用于更新玩家输入的操作
    {
        Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Operation.Direction direction = new Operation.Direction();
        direction.X = inputDirection.x;
        direction.Y = inputDirection.z;
        BattleData.Instance.selfOperation.direction = direction;
        Debug.Log("x:" + BattleData.Instance.selfOperation.direction.X + " ,y:" + BattleData.Instance.selfOperation.direction.Y);
    }

    public void LogicUpdate(Operation[] ops)
    {
        foreach (var op in ops)
        {
            roleControllers[op.BattleUserId].LogicUpdate(op);
        }
    }
}