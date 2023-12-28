using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;


public class TrailCaster : MonoBehaviour
{
    #region PublicVariables

    public bool useFixedUpdate = false;

    #endregion

    #region PrivateVariables

    [Header("TrailCast 점의 시작점, 끝점 좌표의 y축을 따릅니다.")]
    [SerializeField]
    private float m_trailStart;

    [SerializeField]
    private float m_trailEnd;
    
    [Header("TrailCast 점의 기준 각도")]
    [SerializeField]
    private Quaternion m_trailRotation = Quaternion.identity;

    [Header("TrailCast 점들의 수, 최소 2개 이상이어야 정상작동")]
    [SerializeField]
    private int m_trailCountY = 6;

    [Header("TrailCast 점의 두께, 최소 1개 이상이어야 정상작동")]
    [SerializeField]
    private int m_thicknessX = 1;

    [SerializeField]
    private float m_thicknessInterval = 0.2f;

    //Cast 위치 체크용 변수
    private Vector3[,] oldPos;
    private Vector3[,] curPos;
    private Vector3[,] localTrailPos;

    //위치 체킹 중에 중복된 적을 검출하지 않기 위한 변수
    private Dictionary<long, HitInfo> attackedList = new();
    private List<HitInfo> buffer = new();

    [Header("Gizmo 관련 변수, localScale에 영향을 받습니다.")]
    [SerializeField]
    private bool m_showGizmo = true;

    [SerializeField]
    private float m_gizmoRadius = 0.05f;
    
    #endregion
    
    public event EventHandler CheckStarted;
    
    public event EventHandler CheckFinished;

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        if (IsChecking)
        {
            TrailCast();
        }
    }

    private void OnDrawGizmos()
    {
        if (!m_showGizmo) return;

        for (int i = 0; i < m_thicknessX; i++)
        {
            for (int j = 0; j < m_trailCountY; j++)
            {
                Gizmos.color = Color.red;
                Matrix4x4 parentMat = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.matrix = parentMat;
                Gizmos.DrawWireSphere(m_trailRotation * localTrailPos[i, j], m_gizmoRadius);
            }
        }
    }

    private void OnValidate()
    {
        Init();
    }

    #region PublicMethods

    public bool IsChecking { get; private set; }

    /// <summary>
    /// 이 TrailCaster의 체킹을 시작합니다. PopBuffer와 GetHitObjects로 공격 중 발생한 충돌을 얻어올 수 있습니다.
    /// </summary>
    public void StartCheck()
    {
        if (IsChecking) return;

        CheckStarted?.Invoke(this, EventArgs.Empty);
        //점들의 값 초기화
        for (int i = 0; i < m_thicknessX; i++)
        {
            for (int j = 0; j < m_trailCountY; j++)
            {
                oldPos[i, j] = curPos[i, j] = transform.TransformPoint(m_trailRotation * localTrailPos[i, j]);
            }
        }

        IsChecking = true;
    }

    public void EndCheck()
    {
        if (!IsChecking) return;

        CheckFinished?.Invoke(this, EventArgs.Empty);
        IsChecking = false;
        attackedList.Clear();
    }

    /// <summary>
    /// 마지막 PopBuffer()호출 전까지의 충돌 정보를 반환합니다. 한 공격 사이클 동안 반환되는 값은 중복이 없는 것이 보장됩니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<HitInfo> PopBuffer()
    {
        var result = new List<HitInfo>(buffer);
        buffer.Clear();
        return result;
    }

    #endregion

    #region PrivateMethods

    /// <summary>
    /// trailCount에 맞게 변수들의 위치, 배열의 크기를 초기화합니다. OnValidate와 Awake에서 호출됩니다.
    /// </summary>
    private void Init()
    {
        if (m_trailCountY <= 1) return;
        localTrailPos = new Vector3[m_thicknessX, m_trailCountY];
        oldPos = new Vector3[m_thicknessX, m_trailCountY];
        curPos = new Vector3[m_thicknessX, m_trailCountY];


        //Vector3 interval = Quaternion.AngleAxis(m_thickAngle, m_trailEndPoint - m_trailStartPoint);


        float startX = -0.5f * (m_thicknessInterval) * (m_thicknessX - 1);
        for (int i = 0; i < m_thicknessX; i++)
        {
            Vector3 xValue = Vector3.right * (startX + (float)i * m_thicknessInterval);
            localTrailPos[i, 0] = Vector3.up * m_trailStart
                                  + xValue;
            for (int j = 1; j < m_trailCountY; j++)
            {
                localTrailPos[i, j]
                    = Vector3.up * (m_trailStart + (m_trailEnd - m_trailStart) * j / (m_trailCountY - 1))
                      + xValue;
            }
            //localTrailPos[i, m_trailCountY - 1] = Vector3.up * m_trailStart + xValue;
        }
    }

    /// <summary>
    /// 지난 프레임의 TrailPoint들의 위치와 현재 프레임의 TrailPoint들의 위치 사이에 RayCast를 사용합니다.
    /// </summary>
    private void TrailCast()
    {
        RaycastHit[] hits = ArrayPool<RaycastHit>.Shared.Rent(50);
        for (int i = 0; i < m_thicknessX; i++)
        {
            for (int j = 0; j < m_trailCountY; j++)
            {
                oldPos[i, j] = curPos[i, j]; //직전 월드 좌표를 저장
                curPos[i, j] = transform.TransformPoint(m_trailRotation * localTrailPos[i, j]); //월드 좌표를 curPos로 저장

                Vector3 trailOrg = oldPos[i, j];
                Vector3 trailDir = (curPos[i, j] - oldPos[i, j]).normalized;
                int hitCounts = Physics.RaycastNonAlloc(
                    trailOrg, trailDir, hits, trailDir.magnitude,
                    LayerMask.GetMask("Monster", "Damageable"));
                if (hitCounts > hits.Length)
                {
                    Debug.LogWarning("TrailCast: 감지한 적의 수가 한계를 초과함");
                }
                for (int k = 0; k < hitCounts; k++)
                {
                    //딕셔너리에 등록되어있지 않다면, 등록 후 리스트에 추가
                    int jid = hits[k].transform.gameObject.GetInstanceID();
                    if (!attackedList.ContainsKey(jid))
                    {
                        HitInfo info = new HitInfo(trailDir, hits[k]);
                        attackedList.Add(jid, info);
                        buffer.Add(info);
                    }
                }

                //DrawRay
                Debug.DrawRay(trailOrg, trailDir, Color.magenta, 3f, true);
            }
        }
        ArrayPool<RaycastHit>.Shared.Return(hits);
    }

    #endregion

    [Serializable]
    public struct HitInfo
    {
        //TrailCaster를 통해 충돌한 정보 반환을 위한 구조체
        public HitInfo(Vector3 attackDirection, RaycastHit hit)
        {
            AttackDirection = attackDirection;
            Hit = hit;
        }

        public Vector3 AttackDirection { get; }
        public RaycastHit Hit;
    }
}