using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TrailCaster : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;

    
    public int trailCount = 6;
    private Vector3[] oldPos;
    private Vector3[] curPos;
    private Vector3[] localTrailPos;
    private bool m_isChecking = false;
    public bool useFixedUpdate = false;

    private Dictionary<long, RaycastHit> attackedList = new ();
    private List<RaycastHit> buffer = new();

    private void OnValidate()
    {
        Init();
    }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        if (trailCount <= 1) return;
        localTrailPos = new Vector3[trailCount];
        oldPos = new Vector3[trailCount];
        curPos = new Vector3[trailCount];
        localTrailPos[0] = start;
        for (int i = 1; i < trailCount-1; i++)
        {
            localTrailPos[i] = start +  (end-start) * i / (trailCount - 1);
        }
        localTrailPos[trailCount - 1] = end;
    }

    public void StartCheck()
    {
        for (int i = 0; i < curPos.Length; i++)
        {
            oldPos[i] = curPos[i] = transform.TransformPoint(localTrailPos[i]);
        }
        m_isChecking = true;
    }

    public IEnumerable<RaycastHit> PopBuffer()
    {
        var result = new List<RaycastHit>(buffer);
        buffer.Clear();
        return result;
    }

    public void EndCheck()
    {
        m_isChecking = false;
        attackedList.Clear();
    }

    private void Update()
    {
        if (m_isChecking)
        {
            TrailCast();
        }
    }

    void TrailCast()
    {
        for (int i = 0; i < trailCount; i++)
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

    private void OnDrawGizmos()
    {
        for (int i = 0; i < trailCount; i++)
        {
            Gizmos.color = Color.red;
            Matrix4x4 parentMat = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.matrix = parentMat;
            Gizmos.DrawWireSphere(localTrailPos[i], 0.05f);
        }
    }
}
