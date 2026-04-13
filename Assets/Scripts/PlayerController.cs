using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineInternal;

public class PlayerController : MonoBehaviour
{
    // 속도 조절 변수
    [Header("Speed Settings")]
    [SerializeField] private float _walkSpeed;   // 걷기
    [SerializeField] private float _runSpeed;    // 달리기
    [SerializeField] private float _crouchSpeed; // 앉기
    [SerializeField] private float _crawlSpeed;  // 엎드리기
    private float _applySpeed; // 가중치 적용 후 속도

    [SerializeField] private float _jumpForce; // 점프 가속도

    [Header("Camera Settings")]
    [SerializeField] private Camera _theCamera;
    [SerializeField] private float _lookSensitivity; // 마우스 감도
    [SerializeField] private float _cameraRotationLimit; // 카메라 회전 제한
    private float _currentCameraRotationX = 0; // 현재 카메라 상하 회전값

    // 상태 변수
    private bool _isWalking;
    private bool _isRunning;
    private bool _isCrouching;
    private bool _isJumping;
    private bool _isGrounded;
    private bool _isCrawling;



    // 컴포넌트
    private Rigidbody _myRigid;

    // 스탯 관련 가중치 변수 (임시)
    private float tempDexSpeed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _myRigid = GetComponent<Rigidbody>();
        _applySpeed = _walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        InputManage();
        //Move();
    }

    private void InputManage()
    {
        // 카메라 상하 조정
        float cameraRotationX = Input.GetAxisRaw("Mouse Y");
        if (cameraRotationX != 0)
        {
            RotationCamera(cameraRotationX);
        }

        // 카메라 좌우 조정(캐릭터 좌우 회전)
        float characterRotationY = Input.GetAxisRaw("Mouse X");
        if (characterRotationY != 0)
        {
            RotationCharacter(characterRotationY);
        }


        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirY = Input.GetAxisRaw("Vertical");
        // w, a, s, d 키 입력에 따른 이동 방향 계산
        if (moveDirX != 0 || moveDirY != 0)
        {
            Move(moveDirX, moveDirY);
        }
        // shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            EnterRun();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ExitRun();
        }
    }



    private void Move(float moveDirX, float moveDirY)
    {
        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirY;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * (_applySpeed * tempDexSpeed);

        _myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    private void EnterRun()
    {
        UpdateRun();
    }

    private void UpdateRun()
    {
        _isWalking = true;
        _applySpeed = _runSpeed;
    }

    private void ExitRun()
    {
        _isWalking = false;
        _applySpeed = _walkSpeed;
    }


    private void  RotationCamera(float cameraRotationX)
    {
        cameraRotationX *= _lookSensitivity;
        _currentCameraRotationX -= cameraRotationX;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -_cameraRotationLimit, _cameraRotationLimit);
        _theCamera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0, 0);
    }

    private void RotationCharacter(float characterRotationY)
    {
        Vector3 characterRotation = Vector3.up * _lookSensitivity * characterRotationY;
        _myRigid.MoveRotation(_myRigid.rotation * Quaternion.Euler(characterRotation));
    }
}
