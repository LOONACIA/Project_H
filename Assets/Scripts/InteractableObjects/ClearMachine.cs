using LOONACIA.Unity.Managers;

public class ClearMachine : InteractableObject
{
    protected override void OnInteract(Actor actor)
    {
        if (actor.IsPossessed)
        {
            //GameManager.Instance.SetGameClear();
            SceneManagerEx.LoadScene("Stage2");
        }
    }
}
