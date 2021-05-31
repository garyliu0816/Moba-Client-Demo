using Messages.Battle;
using UnityEngine;

public class RoleController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;
    [SerializeField]
    private float rotateSpeed = 15f;

    Animator mAnimator;
    Rigidbody mRigidbody;

    private Vector3 logicPosition; // 逻辑位置
    private Vector3 logicDirection; // 逻辑角度

    private void Start()
    {
        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 position)
    {
        transform.position = position;
        transform.rotation = Quaternion.identity;
    }

    private void Update() // 用于更新展示的画面
    {
        Vector3 renderPosition = Vector3.Lerp(mRigidbody.position, logicPosition, moveSpeed * Time.deltaTime);
        Vector3 renderDirection = Vector3.Lerp(transform.forward, logicDirection, rotateSpeed * Time.deltaTime);
        mRigidbody.MovePosition(renderPosition);
        transform.rotation = Quaternion.LookRotation(renderDirection);
    }

    // private void FixedUpdate() // 用于更新玩家输入的操作
    // {
    //     Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
    //     BattleData.Instance.selfOperation.direction.X = inputDirection.x;
    //     BattleData.Instance.selfOperation.direction.Y = inputDirection.z;
    // }

    public void LogicUpdate(Operation op) // 收到消息后更新逻辑数据
    {
        // 角度
        logicDirection = new Vector3(op.direction.X, 0, op.direction.Y);
        // 位置
        logicPosition = logicPosition + moveSpeed * logicDirection * 0.001f * NetConfig.FRAME_TIME;
    }
}
// using UnityEngine;

// public class RoleController : MonoBehaviour
// {
//     [SerializeField]
//     private float moveSpeed = 2f;
//     [SerializeField]
//     private float rotateSpeed = 15f;

//     Animator mAnimator;
//     Rigidbody mRigidbody;

//     private void Start()
//     {
//         mAnimator = GetComponent<Animator>();
//         mRigidbody = GetComponent<Rigidbody>();
//     }

//     private void FixedUpdate()
//     {
//         Move();
//     }

//     private void Move()
//     {
//         Vector3 toDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
//         Vector3 moveDelta = toDirection * moveSpeed * Time.fixedDeltaTime;
//         mRigidbody.MovePosition(mRigidbody.position + moveDelta);

//         Vector3 fromDirection = transform.forward;
//         Vector3 curDirection = Vector3.Lerp(fromDirection, toDirection, rotateSpeed * Time.fixedDeltaTime);
//         transform.rotation = Quaternion.LookRotation(curDirection);
//     }
// }