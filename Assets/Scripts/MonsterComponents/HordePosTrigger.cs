using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordePosTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawnGroup;

    private LayerMask m_targetLayer;
    // Start is called before the first frame update
    void Start()
    {
        m_targetLayer = LayerMask.NameToLayer("PlayerbleCamera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO: 몬스터 호출 시 생성된 몬스터들에게 현재 캐릭터를 Target화 해서 정보 건네주기
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == m_targetLayer) 
        {
            Debug.Log("트리거 진입");
            foreach (GameObject obj in spawnGroup)
            {
                obj.SetActive(true);
            }
            this.gameObject.SetActive(false);
        }
    }
}
