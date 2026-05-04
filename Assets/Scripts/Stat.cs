using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField] public float _statAttackPower; // 공격력
    [SerializeField] public float _statMagicPower;  // 마력
    [SerializeField] public float _statAgility;     // 순발력
    [SerializeField] public float _statHealth;      // 체력
    [SerializeField] public float _statWillPower;   // 정신력
    [SerializeField] public float _statStamina;     // 지구력

    private float _attackPower; // 물리 공격력
    private float _magicPower; // 마법 공격력
    private float _moveSpeedRate; // 이동속도 배율  
    private float _shotSpeed; // 연사속도
    private float _maxHP; // HP 총량
    private float _currentHP; // 현재 HP
    private float _maxMP; // MP 총량
    private float _currentMP; // 현재 MP
    private float _maxST; // 스테미너 총량
    private float _currentST; // 현재 스테미너

    //읽기 전용 변수
    public float moveSpeedRate => _moveSpeedRate;

    //스테미너 관리 변수
    float checkInterval = 1.5f; // 이 시간동안 스테미너의 변동이 없으면 회복 시작
    float timer = 0f; // 스테미너 변동이 없었던 시간
    float lastValue; // 마지막으로 스테미너가 변동된 값
    float stRecoveryAmount; // 초당 스테미너 회복량
    bool initialized = false; // 초기화 여부 lastValue가 초기화되지 않았을 때 false, 초기화된 후 true


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _maxST = _statStamina * 10; // 스테미너 총량은 스테미너의 10배로 설정
        _currentST = _maxST; // 현재 스테미너는 총량으로 초기화
        stRecoveryAmount = _statStamina * 2; // 초당 스테미너 회복량 초기화
    }

    // Update is called once per frame
    void Update()
    {
        SetMoveSpeedRate(); // 지금은 항상 확인하지만 나중에 스탯을 올리는 함수를 짜면 올릴때만 적용하면 됨
        RegenerateStamina();
    }

    private void SetMoveSpeedRate()
    {
        if(_statAgility<31) // 30 까지는 (_statAgility+100)/100 으로 증가
            _moveSpeedRate = 1.0f + _statAgility / 100;  
        else if (_statAgility < 51) // 30~50은 그 전의 절반
        {
            _moveSpeedRate = 1.3f + (_statAgility-30) / 200;
        }
        else // 그 이후로는 거의 미비하게
            _moveSpeedRate = 1.4f + (_statAgility-50) / 500; 

    }

    // 스테미너 회복 함수
    private void RegenerateStamina()
    {
        if (!initialized)
        {
            lastValue = _currentST; // 초기화되지 않았으면 현재 스테미너로 초기화
            initialized = true;
            timer = 0f;
        }

        if(_currentST < _maxST) // 현재 스테미너가 최대 스테미너보다 작을 때만 회복 로직 실행
        {
            if (_currentST < lastValue) // 스테미너가 사용되었는지 확인
            {
                timer = 0f; // 변동이 있으면 타이머 초기화
            }
            else
            {
                timer += Time.deltaTime; // 변동이 없으면 타이머 증가
                if (timer >= checkInterval) // 타이머가 체크 간격을 초과하면 회복 시작
                {
                    _currentST += stRecoveryAmount * Time.deltaTime; // 스테미너 회복량 계산
                    Debug.Log(_currentST+"/"+_maxST);
                    if (_currentST >= _maxST) // 최대 스테미너를 초과하지 않도록 제한
                    {
                        _currentST = _maxST;
                        timer = 0f; // 최대 스테미너에 도달하면 타이머 초기화
                    }
                }
            }
        }
    }

    // 사용할 스테미너 양을 입력받고, 사용가능하면 true, 부족하면 false를 반환하는 함수
    public bool UseStamina(float amount)
    {
        if(amount> _currentST)
        {
            Debug.Log("스테미너가 부족합니다.");
            return false; // 스테미너가 부족하여 사용할 수 없음
        }
        else {
            Debug.Log(_currentST + "/" + _maxST);
            _currentST -= amount;
            initialized = false;
            return true;
        }
    }
}
