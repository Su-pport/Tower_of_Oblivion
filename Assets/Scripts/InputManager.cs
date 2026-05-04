using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    // Mouse
    public float _mouseX { get; private set; }
    public float _mouseY { get; private set; }

    // Move
    public float _moveX { get; private set; }
    public float _moveY { get; private set; }

    // Actions
    public bool _run { get; private set; }
    public bool _runRollUp { get; private set; }
    public bool _jump { get; private set; }
    public bool _crouchDown { get; private set; }
    public bool _crawlDown { get; private set; }
    public bool _interactionDown { get; private set; }
    public bool _runRollDown {get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 마우스
        _mouseX = Input.GetAxisRaw("Mouse X");
        _mouseY = Input.GetAxisRaw("Mouse Y");

        // 이동
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");

        // 행동
        _runRollDown = Input.GetKeyDown(KeyCode.LeftShift);
        _run = Input.GetKey(KeyCode.LeftShift);
        _runRollUp = Input.GetKeyUp(KeyCode.LeftShift);
        _jump = Input.GetKey(KeyCode.Space);
        _crouchDown = Input.GetKeyDown(KeyCode.C);
        _crawlDown = Input.GetKeyDown(KeyCode.Z);
        _interactionDown = Input.GetMouseButtonDown(1);
    }
}