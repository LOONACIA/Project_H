using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class TrailCaster : MonoBehaviour
{
    #region PublicVariables
    
    public bool useFixedUpdate = false;
    
    #endregion
    
    #region PrivateVariables
    [Header("TrailCast 점들의 시작점, 끝점")]
    [SerializeField]private Vector3 m_trailStartPoint;
    [SerializeField]private Vector3 m_trailEndPoint;
    
    [Header("TrailCast 점들의 수, 최소 2개 이상이어야 정상작동")]
    [SerializeField] private int m_trailCount = 6;

    //Cast 위치 체크용 변수
    private Vector3[] oldPos;
    private Vector3[] curPos;
    private Vector3[] localTrailPos;

    //위치 체킹 중에 중복된 적을 검출하지 않기 위한 변수
    private Dictionary<long, RaycastHit> attackedList = new ();
    private List<RaycastHit> buffer = new();

    #endregion

    #region PublicMethods
    
    public bool IsChecking { get; private set; }

    /// <summary>
    /// 이 TrailCaster의 체킹을 시작합니다. PopBuffer와 GetHitObjects로 공격 중 발생한 충돌을 얻어올 수 있습니다.
    /// </summary>
    public void StartCheck()
    {
        //점들의 값 초기화
        for (int i = 0; i < curPos.Length; i++)
        {
            oldPos[i] = curPos[i] = transform.TransformPoint(localTrailPos[i]);
        }
        IsChecking = true;
    }
    
    public void EndCheck()
    {
        IsChecking = false;
        attackedList.Clear();
    }
    
    /// <summary>
    /// 마지막 PopBuffer()호출 전까지의 충돌 정보를 반환합니다. 한 공격 사이클 동안 반환되는 값은 중복이 없는 것이 보장됩니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<RaycastHit> PopBuffer()
    {
        var result = new List<RaycastHit>(buffer);
        buffer.Clear();
        return result;
    }
    

    #endregion

    #region PrivateMethods

    #region UnityEventFunctions

    private void OnValidate()
    {
        Init();
    }

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
        for (int i = 0; i < m_trailCount; i++)
        {
            Gizmos.color = Color.red;
            Matrix4x4 parentMat = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.matrix = parentMat;
            Gizmos.DrawWireSphere(localTrailPos[i], 0.05f);
        }
    }

    #endregion
    
    
    /// <summary>
    /// trailCount에 맞게 변수들의 위치, 배열의 크기를 초기화합니다. OnValidate와 Awake에서 호출됩니다.
    /// </summary>
    private void Init()
    {
        if (m_trailCount <= 1) return;
        localTrailPos = new Vector3[m_trailCount];
        oldPos = new Vector3[m_trailCount];
        curPos = new Vector3[m_trailCount];
        localTrailPos[0] = m_trailStartPoint;
        for (int i = 1; i < m_trailCount-1; i++)
        {
            localTrailPos[i] = m_trailStartPoint +  (m_trailEndPoint-m_trailStartPoint) * i / (m_trailCount - 1);
        }
        localTrailPos[m_trailCount - 1] = m_trailEndPoint;
    }
    
    /// <summary>
    /// 지난 프레임의 TrailPoint들의 위치와 현재 프레임의 TrailPoint들의 위치 사이에 RayCast를 사용합니다.
    /// </summary>
    private void TrailCast()
    {
        for (int i = 0; i < m_trailCount; i++)
        {
            oldPos[i] = curPos[i];  //직전 월드 좌표를 저장
            curPos[i] = transform.TransformPoint(localTrailPos[i]);     //월드 좌표를 curPos로 저장
            
            Vector3 localDir = oldPos[i] - curPos[i];
            RaycastHit[] hits = Physics.RaycastAll(
                curPos[i], 
                oldPos[i] - curPos[i],
                localDir.magnitude,
                LayerMask.GetMask("Monster"));
            for (int j = 0; j < hits.Length; j++)
            {
                //딕셔너리에 등록되어있지 않다면, 등록 후 리스트에 추가
                int jid = hits[j].transform.gameObject.GetInstanceID();
                if(!attackedList.ContainsKey(jid))
                {
                    //현재 레이를 실제 무기의 진행방향의 정 반대로 쏘므로, 충돌 방향을 수정해줌
                    hits[j].normal = hits[j].normal * -1;
                    
                    attackedList.Add(jid,hits[j]);
                    buffer.Add(hits[j]);
                }
            }
            
            //DrawRay
            Debug.DrawRay(curPos[i], oldPos[i] - curPos[i], Color.magenta, 0.3f, true);

        }
    }

    #endregion
}
