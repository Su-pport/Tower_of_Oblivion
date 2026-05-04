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
    private float _maxMP; // MP 총략
    private float _maxST; // 스테미너 총량

    //읽기 전용 프로퍼티
    public float moveSpeedRate => _moveSpeedRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetMoveSpeedRate(); // 지금은 항상 확인하지만 나중에 스탯을 올리는 함수를 짜면 올릴때만 적용하면 됨
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
}
