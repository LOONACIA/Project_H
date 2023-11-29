using LOONACIA.Unity.Managers;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager
{
    private GameSettings m_settings;
    
    private PlayerController m_controller;

    private ActorType m_actorType;
    
    private Vector3 m_respawnPosition;
    
    private Quaternion m_respawnRotation;

    public void Init(GameSettings settings)
    {
        m_settings = settings;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void FindController()
    {
        m_controller = Object.FindObjectOfType<PlayerController>();
    }

    public void SaveInformation(Actor actor, Transform respawnPosition)
    {
        m_actorType = actor.Data.Type;
        m_respawnPosition = respawnPosition.position;
        m_respawnRotation = respawnPosition.rotation;
    }

    private void Respawn()
    {
        if (m_respawnPosition == Vector3.zero)
        {
            return;
        }
        
        var prefab = m_settings.ActorPrefabs.Single(x => x.Type == m_actorType);
        
        var character = Object.Instantiate(prefab.Prefab, m_respawnPosition, m_respawnRotation);
        m_controller.ChangeActor(character.GetComponent<Actor>());
    }
    
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        FindController();
        Respawn();
    }
}
