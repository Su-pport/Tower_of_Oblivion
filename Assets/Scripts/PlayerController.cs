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
    private RaycastHit _target;
    
    // 스탯 관련 가중치 변수 (임시)
    private float tempDexSpeed = 1f;

    // Start is called once before the first execution of In after the MonoBehaviour is created
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

    // In is called once per frame
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
            StartRun();
        }
        // shift 키에서 손을 뗐을 때 달리기 상태 종료
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            EndRun();
        }

        // 스페이스 키 입력에 따른 점프
        if (Input.GetKey(KeyCode.Space))
        {
            StartJump();
        }

        // c 키 입력에 따른 앉기 상태 토글
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCrouch();
        }

        // z 키 입력에 따른 엎드리기 상태 토글
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCrawling();
        }

        // 우클릭 입력에 따른 상호작용
        if ( Input.GetMouseButtonDown(1))
        {
            StartInteraction();
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
    private void StartRun()
    {
        if (_isCrouching)
            EndCrouch();
        if (_isCrawling)
            EndCrawling();

        InRun();
    }

    private void InRun()
    {
        _isWalking = true;
        _applySpeed = _runSpeed;
    }

    private void EndRun()
    {
        _isWalking = false;
        _applySpeed = _walkSpeed;
    }
    
    // 점프
    private void StartJump()
    {
        if (_isCrouching)
            EndCrouch();
        if (_isCrawling)
            EndCrawling();

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
            EndCrawling();
        if (!_isCrouching)
            InCrouch();
        else
            EndCrouch();
    }

    private void InCrouch()
    {
        _isCrouching = true;
        _applySpeed = _crouchSpeed;
        _theCamera.transform.localPosition = new Vector3(0, _theCameraLocalPosY / 2, 0);
    }

    private void EndCrouch()
    {
        _theCamera.transform.localPosition = new Vector3(0, _theCameraLocalPosY, 0);
        _isCrouching = false;
        _applySpeed = _walkSpeed;
    }

    // 엎드리기
    private void StartCrawling()
    {
        if (_isCrouching)
            EndCrouch();
        if (!_isCrawling)
            InCrawling();
        else
            EndCrawling();
    }

    private void InCrawling()
    {
        _isCrawling = true;
        _applySpeed = _crawlSpeed;
        _theCamera.transform.localPosition = new Vector3(0, _theCameraLocalPosY / 4, 0);
    }

    private void EndCrawling()
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
