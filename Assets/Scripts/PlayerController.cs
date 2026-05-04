using UnityEngine;
using System.Collections;
using System;

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
    private float _applyCameraLocalPosY; // 앉기, 엎드리기 시 목표 카메라 위치


    // 상태 변수
    private bool _isWalking;
    private bool _isRunning;
    private bool _isCrouching;
    private bool _isJumping;
    private bool _isGrounded;
    private bool _isCrawling;
    private bool _isRolling;

    // 구르기 변수
    float pressStartTime; // 누르기 시작한 시점을 저장
    float threshold = 0.2f; // 얼마나 짧게 눌러야 구르기를 할 지 구분을 위한 변수
    float rollInvincible = 0.2f; // 구르는동안 무적 시간

    // 컴포넌트
    private Rigidbody _myRigid;
    private CapsuleCollider _myCapsule;
    private RaycastHit _target;
    
    // 스탯 관련 가중치 변수 (임시)
    Stat _stat;
    private float _agiSpeedRate;

    // Start is called once before the first execution of In after the MonoBehaviour is created
    void Start()
    {
        // 컴포넌트 초기화
        _myRigid = GetComponent<Rigidbody>();
        _myCapsule = GetComponent<CapsuleCollider>();
        _theCameraLocalPosY = _theCamera.transform.localPosition.y;
        _stat = GetComponent<Stat>();

        // 속도 초기화
        _runSpeed = _walkSpeed * _runSpeedRate;
        _crouchSpeed = _walkSpeed * _crouchSpeedRate;
        _crawlSpeed = _walkSpeed * _crawlSpeedRate;

        _applySpeed = _walkSpeed;


        // 상태 초기화
        _isRunning = false;
        _isCrouching = false;
        _isCrawling = false;
        _isRolling = false;

    }

    // In is called once per frame
    void Update()
    {
        HandleInput();
        IsGrounded();
        //Move();
    }

    // 입력 관리
    private void HandleInput()
    {
        var input = InputManager.Instance;

        if (input._mouseY != 0)
            RotationCamera(input._mouseY);

        if (input._mouseX != 0)
            RotationCharacter(input._mouseX);

        if (input._moveX != 0 || input._moveY != 0)
            Move(input._moveX, input._moveY);

        
        if (input._runRollDown)
        {
            pressStartTime = Time.time;
        }

        if (input._runRollUp)
        {
            EndRun();

            float pressDuration = Time.time - pressStartTime;
            Debug.Log(pressDuration);
            if(pressDuration <= threshold)
                StartRoll();            
        }
        
        if (input._run && !_isRunning && Time.time - pressStartTime > threshold)
            StartRun();

        // if (input._runUp)
        //     EndRun();

        if (input._jump)
            StartJump();

        if (input._crouchDown)
            StartCrouch();      

        if (input._crawlDown)
            StartCrawl();     
            
        if (input._interactionDown)
            StartInteraction();
    }



    // 움직임
    private void Move(float moveDirX, float moveDirY)
    {
        if(_isRolling) return;
        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirY;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * (_applySpeed * _stat.moveSpeedRate);
        //Debug.Log(_applySpeed*_stat.moveSpeedRate); 

        _myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    // 달리기
    private void StartRun()
    {
        if (_isCrouching)
            EndCrouch();
        if (_isCrawling)
            EndCrawl();

        InRun();
    }

    private void InRun()
    {
        _isRunning = true;
        _applySpeed = _runSpeed;
    }

    private void EndRun()
    {
        _isRunning = false;
        _applySpeed = _walkSpeed;
    }

    // 점프
    private void StartJump()
    {
        if (_isCrouching)
            EndCrouch();
        if (_isCrawling)
            EndCrawl();

        if (_isGrounded)
        {
            InJump();
        }
    }

    private void InJump()
    {
        _myRigid.linearVelocity = transform.up * _jumpForce;

    }

    // 앉기
    private void StartCrouch()
    {
        if(_isCrawling)
            EndCrawl();
        if (!_isCrouching)
            InCrouch();
        else
            EndCrouch();
    }

    private void InCrouch()
    {
        _isCrouching = true;
        _applyCameraLocalPosY = _theCameraLocalPosY / 2;
        StartCoroutine(PostureCoroutine());
        _applySpeed = _crouchSpeed;
    }

    private void EndCrouch()
    {
        _isCrouching = false;
        _applyCameraLocalPosY = _theCameraLocalPosY;
        StartCoroutine(PostureCoroutine());
        _applySpeed = _walkSpeed;
    }

    // 엎드리기
    private void StartCrawl()
    {
        if (_isCrouching)
            EndCrouch();
        if (!_isCrawling)
            InCrawl();
        else
            EndCrawl();
    }

    private void InCrawl()
    {
        _isCrawling = true;
        _applyCameraLocalPosY = _theCameraLocalPosY / 4;
        StartCoroutine(PostureCoroutine());
        _applySpeed = _crawlSpeed;
    }

    private void EndCrawl()
    {
        _isCrawling = false;
        _applyCameraLocalPosY = _theCameraLocalPosY;
        StartCoroutine(PostureCoroutine());
        _applySpeed = _walkSpeed;
    }

    // 앉기, 엎드리기 시 카메라 위치 보정 코루틴
    IEnumerator PostureCoroutine()
    {
        float currentCameraY = _theCamera.transform.localPosition.y;
        int cnt = 0;
        while (currentCameraY != _applyCameraLocalPosY)
        {
            currentCameraY = Mathf.Lerp(currentCameraY, _applyCameraLocalPosY, 0.2f);
            _theCamera.transform.localPosition = new Vector3(0, currentCameraY, 0);
            yield return null;

            cnt++;
            if (cnt > 15)
                break;

        }
        _theCamera.transform.localPosition = new Vector3(0, _applyCameraLocalPosY, 0);
    }

    // 구르기
    private void StartRoll()
    {
        Debug.Log("구르기");
    }

    private void InRoll()
    {
        _isRolling = true;
    }

    private void EndRoll()
    {
        _isRolling = false;
    }

    IEnumerator RollCoroutine()
    {
        yield return null; //카메라 아래위 이동
        // input._moveX, Y 이용해서 방향 받아서 일정거리 이동
    }

    // 상태 확인
    private void IsGrounded()
    {
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _myCapsule.bounds.extents.y + 0.1f);
    }


    // 상호작용
    private void StartInteraction()
    {
        // 상호작용 로직 구현 (예: 레이캐스트로 앞에 있는 오브젝트 감지 후 상호작용)
        Debug.Log("상호작용 시도");

        if (Physics.Raycast(transform.position, transform.forward, out _target, 2f))
        {
            switch (_target.collider.tag)
            {
                case "NPC":
                    InInteractionNPC();
                    break;
                default:
                    Debug.Log("상호작용 실패");
                    break;
            }
        }
    }

    private void InInteractionNPC()
    {
        NPC targetNPC = _target.collider.gameObject.GetComponent<NPC>();
        targetNPC.hello();
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
