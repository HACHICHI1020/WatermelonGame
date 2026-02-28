using UnityEngine;

public class GameOver : MonoBehaviour
{
    public enum DetectState
    {
        Detecting, Detected
    }
    public DetectState detectState = DetectState.Detecting; //감지 중 상태

    public Vector2 range; //게임 오버 범위
    public LayerMask fruitLayer; //과일 레이어

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; //기즈모 컬러 빨간색
        Vector2 origin = transform.position;
        Vector2 size = range;
        
        //큐브를 그린다.
        Gizmos.DrawWireCube(origin, size);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(detectState)
        {
            case DetectState.Detecting: //감지 중 상태
                {
                    //박스 형태의 레이를 사용한다.(위치, 크기, 회전, 방향, 거리, 레이어)
                    RaycastHit2D hit = Physics2D.BoxCast(transform.position,
                        range, 0, Vector2.right, 0, fruitLayer);

                    //감지되는 대상이 있다면
                    if (hit.collider != null)
                    {
                        GameManager.instance.FruitGameOver(); //게임 오버 알림
                        detectState = DetectState.Detected;//감지 완료 상태
                    }
                    break;
                }
        }
    }
}
