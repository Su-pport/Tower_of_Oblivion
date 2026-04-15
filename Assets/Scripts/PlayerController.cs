using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineInternal;

public class PlayerController : MonoBehaviour
{
    // 속도 조절 변수
    [SerializeField] private float _walkSpeed;   // 걷기
    [Header("Speed Rate")]
    [SerializeField] private float _runSpeedRate;    // 달리기
    [SerializeField] private float _crouchSpeedRate; // 앉기
    [SerializeField] private float _crawlSpeedRate;  // 엎드리기
    private float _runSpeed; // 달리기 속도
    private float _crouchSpeed; // 앉기 속도
    private float _crawlSpeed; // 엎드리기 속도
    private float _applySpeed; // 가중치 적용 후 속도

    [SerializeField] private float _jumpForce; // 점프 가속도

    [Header("Camera Settings")]
    [SerializeField] private Camera _theCamera;
    [SerializeField] private float _lookSensitivity; // 마우스 감도
    [SerializeField] private float _cameraRotationLimit; // 카메라 회전 제한
    private float _currentCameraRotationX = 0; // 현재 카메라 상하 회전값
    private float _theCameraLocalPosY; // 카메라 로컬 Y 위치 (앉기 상태에서 원래 위치로 돌아가기 위해 저장)

    // 상태 변수
    private bool _isWalking;
    private bool _isRunning;
    private bool _isCrouching;
    private bool _isJumping;
    private bool _isGrounded;
    private bool _isCrawling;



    // 컴포넌트
    private Rigidbody _myRigid;
    private CapsuleCollider _myCapsule;

    // 스탯 관련 가중치 변수 (임시)
    private float tempDexSpeed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 컴포넌트 초기화
        _myRigid = GetComponent<Rigidbody>();
        _myCapsule = GetComponent<CapsuleCollider>();
        _theCameraLocalPosY = _theCamera.transform.localPosition.y;

        // 속도 초기화
        _runSpeed = _walkSpeed * _runSpeedRate;
        _crouchSpeed = _walkSpeed * _crouchSpeedRate;
        _crawlSpeed = _walkSpeed * _crawlSpeedRate;

        _applySpeed = _walkSpeed;

        // 상태 초기화
        _isCrouching = false;
        _isCrawling = false;

    }

    // Update is called once per frame
    void Update()
    {
        InputManage();
        IsGrounded();
        //Move();
    }

    // 입력 관리
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

        // w, a, s, d 키 입력에 따른 움직임
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirY = Input.GetAxisRaw("Vertical");
        if (moveDirX != 0 || moveDirY != 0)
        {
            Move(moveDirX, moveDirY);
        }

        // shift 키 입력에 따른 달리기 상태 전환
        if (Input.GetKey(KeyCode.LeftShift))
        {
            EnterRun();
        }
        // shift 키에서 손을 뗐을 때 달리기 상태 종료
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ExitRun();
        }

        // 스페이스 키 입력에 따른 점프
        if (Input.GetKey(KeyCode.Space))
        {
            EnterJump();
        }

        // c 키 입력에 따른 앉기 상태 토글
        if (Input.GetKeyDown(KeyCode.C))
        {
            EnterCrouch();
        }

        // z 키 입력에 따른 엎드리기 상태 토글
        if (Input.GetKeyDown(KeyCode.Z))
        {
            EnterCrawling();
        }
    }


    // 움직임
    private void Move(float moveDirX, float moveDirY)
    {
        Debug.Log(_applySpeed);
        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirY;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * (_applySpeed * tempDexSpeed);

        _myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    // 달리기
    private void EnterRun()
    {
        if (_isCrouching)
            ExitCrouch();
        if (_isCrawling)
            ExitCrawling();

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
    
    // 점프
    private void EnterJump()
    {
        if (_isCrouching)
            ExitCrouch();
        if (_isCrawling)
            ExitCrawling();

        if (_isGrounded)
        {
            UpdateJump();
        }
    }

    private void UpdateJump()
    {
        _myRigid.linearVelocity = transform.up * _jumpForce;

    }

    // 앉기
    private void EnterCrouch()
    {
        if(_isCrawling)
            ExitCrawling();
        if (!_isCrouching)
            UpdateCrouch();
        else
            ExitCrouch();
    }

    private void UpdateCrouch()
    {
        _isCrouching = true;
        _applySpeed = _crouchSpeed;
        _theCamera.transform.localPosition = new Vector3(0, _theCameraLocalPosY / 2, 0);
    }

    private void ExitCrouch()
    {
        _theCamera.transform.localPosition = new Vector3(0, _theCameraLocalPosY, 0);
        _isCrouching = false;
        _applySpeed = _walkSpeed;
    }

    // 엎드리기
    private void EnterCrawling()
    {
        if (_isCrouching)
            ExitCrouch();
        if (!_isCrawling)
            UpdateCrawling();
        else
            ExitCrawling();
    }

    private void UpdateCrawling()
    {
        _isCrawling = true;
        _applySpeed = _crawlSpeed;
        _theCamera.transform.localPosition = new Vector3(0, _theCameraLocalPosY / 4, 0);
    }

    private void ExitCrawling()
    {
        _isCrawling = false;
        _applySpeed = _walkSpeed;
        _theCamera.transform.localPosition = new Vector3(0, _theCameraLocalPosY, 0);
    }

    // 상태 확인
    private void IsGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _myCapsule.bounds.extents.y + 0.1f);
    }

    // 카메라
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
