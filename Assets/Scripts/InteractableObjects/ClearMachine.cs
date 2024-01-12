using LOONACIA.Unity.Managers;
using System.Collections;
using UnityEngine;

public class ClearMachine : InteractableObject
{
    protected override void OnInteract(Actor actor)
    {
        if (actor.IsPossessed)
        {
            GameManager.Instance.SetGameClear();

            //24.01.12: 클리어 시 EndingScene으로 넘어가는 것으로 변경.
            IsInteractable = false;
            ManagerRoot.Input.Disable<CharacterInputActions>();
            _ = SceneHelper.LazyLoadAsync(nameof(SceneName.EndingScene));
        }
    }
}
