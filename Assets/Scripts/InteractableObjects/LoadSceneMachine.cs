using LOONACIA.Unity.Managers;
using UnityEngine;

public class LoadSceneMachine : InteractableObject
{
    [SerializeField]
    private SceneName m_sceneName;
    
    protected override void OnInteract(Actor actor)
    {
        IsInteractable = false;
        ManagerRoot.Input.Disable<CharacterInputActions>();
        _ = SceneHelper.LazyLoadAsync(m_sceneName.ToString());
    }
}
