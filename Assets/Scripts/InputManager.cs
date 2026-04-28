using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    // Mouse
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    // Move
    public float MoveX { get; private set; }
    public float MoveY { get; private set; }

    // Actions
    public bool Run { get; private set; }
    public bool RunUp { get; private set; }
    public bool Jump { get; private set; }
    public bool CrouchDown { get; private set; }
    public bool CrawlDown { get; private set; }
    public bool InteractionDown { get; private set; }

    

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
        MouseX = Input.GetAxisRaw("Mouse X");
        MouseY = Input.GetAxisRaw("Mouse Y");

        // 이동
        MoveX = Input.GetAxisRaw("Horizontal");
        MoveY = Input.GetAxisRaw("Vertical");

        // 행동
        Run = Input.GetKey(KeyCode.LeftShift);
        RunUp = Input.GetKeyUp(KeyCode.LeftShift);
        Jump = Input.GetKey(KeyCode.Space);
        CrouchDown = Input.GetKeyDown(KeyCode.C);
        CrawlDown = Input.GetKeyDown(KeyCode.Z);
        InteractionDown = Input.GetMouseButtonDown(1);



    }
}