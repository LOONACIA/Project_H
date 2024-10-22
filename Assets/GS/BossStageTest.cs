using UnityEngine;

#if UNITY_EDITOR
public class BossStageTest : MonoBehaviour
{
    public Transform position1;
    public Transform position2;
    public Transform position3;
    public Transform position4;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.Character.Controller.Character.transform.position = position1.position;
            Stop();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        { 
            GameManager.Character.Controller.Character.transform.position = position2.position;
            Stop();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameManager.Character.Controller.Character.transform.position = position3.position;
            Stop();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameManager.Character.Controller.Character.transform.position = position4.position;
            Stop();
        }
    }

    private void Stop()
    {
        GameManager.Character.Controller.Character.gameObject.TryGetComponent<Rigidbody>(out var rigidbody);
        rigidbody.velocity = Vector3.zero;
    }
}
#endif 
