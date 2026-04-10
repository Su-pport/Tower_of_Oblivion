using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 속도 조절 변수
    [SerializeField] private float _walkSpeed;   // 걷기
    [SerializeField] private float _runSpeed;    // 달리기
    [SerializeField] private float _crouchSpeed; // 앉기
    [SerializeField] private float _crawlSpeed;  // 엎드리기
    private float _applySpeed; // 가중치 적용 후 속도

    [SerializeField] private float _jumpForce; // 점프 가속도

    // 컴포넌트
    private Rigidbody _myRigid;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _myRigid = GetComponent<Rigidbody>();
        _applySpeed = _walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirY = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirY;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * _applySpeed;

        _myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }
}
