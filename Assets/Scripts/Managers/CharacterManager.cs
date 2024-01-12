using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager
{
    private string m_savedSceneName;
    
    private GameSettings m_settings;

    private ActorType m_actorType;
    
    private Vector3 m_respawnPosition;
    
    private Quaternion m_respawnRotation;

    public BossPhaseType CurrentBossPhase { get; set; }
    
    public PlayerController Controller { get; private set; }

    public void Init(GameSettings settings)
    {
        m_settings = settings;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SaveInformation(Actor actor, Transform respawnPosition)
    {
        m_actorType = actor.Data.Type;
        m_respawnPosition = respawnPosition.position;
        m_respawnRotation = respawnPosition.rotation;
        
        m_savedSceneName = SceneManager.GetActiveScene().name;
    }

    private void RespawnPlayerCharacter()
    {
        if (m_respawnPosition == Vector3.zero)
        {
            return;
        }
        
        var prefab = m_settings.ActorPrefabs.Single(x => x.Type == m_actorType);
        
        var character = Object.Instantiate(prefab.Prefab, m_respawnPosition, m_respawnRotation);
        Controller.ChangeActor(character.GetComponent<Actor>());
    }
    
    private void FindController()
    {
        Controller = Object.FindObjectOfType<PlayerController>();
    }
    
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        FindController();
        if (arg0.name == m_savedSceneName)
        {
            RespawnPlayerCharacter();
        }
        else
        {
            // 다른 씬으로 이동했을 때 초기화
            m_savedSceneName = string.Empty;
            CurrentBossPhase = BossPhaseType.None;
        }
    }
}
