using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LOONACIA.Unity.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * * 주의사항
 * 1. GameManager에 추가 로직을 넣는 것을 지양하고, 별도의 매니저 클래스를 만들어서 작성할 것
 *      (Game Over 처리 등 핵심 로직을 제외한 나머지는 별도 클래스에 있어도 무방하며, 그렇게 하는 것이 가독성 향상에 도움이 됨)
 * 
 * 2. 매니저 클래스는 가급적 MonoBehaviour를 상속받지 않도록 할 것 (수명 관리가 어려움)
 */
public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;

    private static string s_logFilePath;
    
    private static bool s_isApplicationQuitting;

    private GameSettings m_settings;

    // 매니저가 Monobehaviour를 상속받는 경우 CreateInstance 메서드에서 초기화
    // 매니저 클래스에는 SerializedField를 사용할 수 없으므로 ScriptableObject를 상속받는 클래스를 만들어서 사용
    private CameraManager m_camera = new();
    
    private EffectManager m_effect = new();
    
    public static CameraManager Camera => Instance.m_camera;
    
    public static EffectManager Effect => Instance.m_effect;

    public static GameSettings Settings => Instance.m_settings; 

    public bool IsGameOver { get; private set; }

    public static GameManager Instance
    {
        get
        {
            CreateInstance();
            return s_instance;
        }
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (m_settings != null)
        {
            m_settings = ManagerRoot.Resource.Load<GameSettings>($"Settings/{nameof(GameSettings)}");
        }
    }

    private void OnEnable()
    {
        IsGameOver = false;
    }

    public void SetGameOver()
    {
        // TODO: 게임 오버 처리
        IsGameOver = true;
    }

    private static void CreateInstance()
    {
        if (s_isApplicationQuitting)
        {
            return;
        }
        
        if (s_instance == null)
        {
            if (FindObjectOfType<GameManager>() is not { } manager)
            {
                GameObject managerRoot = new() { name = "@Game Manager" };
                manager = managerRoot.AddComponent<GameManager>();
            }
            s_instance = manager;
            DontDestroyOnLoad(s_instance);
            
            // 매니저 클래스 초기화
            s_instance.m_effect.Init();
            
            // 매니저가 Monobehaviour를 상속받는 경우 여기에서 초기화
            // Example:
            //      var manager = s_instance.AddComponent<Manager>();

            #if !UNITY_EDITOR
            InitializeLog();
            Application.logMessageReceived += s_instance.OnLogMessageReceived;
            #endif
            
            SceneManagerEx.SceneChanging += s_instance.OnSceneChanging;
            
            Debug.Log("Game Manager Initialized\n");
        }
    }
    
    private void OnSceneChanging(Scene obj)
    {
        // 씬이 변경될 때 별도 처리가 필요한 경우 여기에 작성
    }
    
    private static void InitializeLog()
    {
        // TODO: 별도 클래스로 분리
        string directory = Path.Join(Application.dataPath, ConstVariables.LOG_PATH);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        s_logFilePath = Path.Join(directory,
            string.Format(ConstVariables.LOG_FILE_FORMAT, DateTime.Now.ToString("yyMMdd_HHmmss")));
    }

    private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
    {
        // TODO: 별도 클래스로 분리
        string line = condition is { Length: > 0 }
            ? $"[{type}] {DateTime.Now:yyyy-MM-dd hh:mm:ss} {condition} {stacktrace}\n"
            : Environment.NewLine;
        File.AppendAllText(s_logFilePath, line);
    }

    private void OnDestroy()
    {
        s_isApplicationQuitting = true;
        Application.logMessageReceived -= s_instance.OnLogMessageReceived;
        SceneManagerEx.SceneChanging -= s_instance.OnSceneChanging;
    }

    private void OnApplicationQuit()
    {
        s_isApplicationQuitting = true;
        Application.logMessageReceived -= s_instance.OnLogMessageReceived;
        SceneManagerEx.SceneChanging -= s_instance.OnSceneChanging;
    }
}