using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーンの管理を行い、シーンのロードやフェードアニメーションを制御するクラス
/// </summary>
public class SceneManager : SingletonMonoBehaviour<SceneManager>
{
    // シーン遷移時にオブジェクトを破棄しない設定
    protected override bool DontDestroyOnLoad => true;

    // フェードアニメーションを制御するコンポーネント
    private FadeAnimation _fadeAnimation;

    // フェード中かどうかを示すプロパティ
    public bool IsFading => _fadeAnimation.IsFading;

    private SceneName _currentScene = SceneName.Title;
    public SceneName CurrentScene => _currentScene;

    // シーン名を管理するディクショナリ
    private Dictionary<SceneName, string> _sceneNameDictionary = new Dictionary<SceneName, string>
    {
        { SceneName.Title, "TitleScene" },
        { SceneName.Setting, "SettingScene" },
        { SceneName.Game, "GameScene" },
        { SceneName.Result, "ResultScene" },
    };

    /// <summary>
    /// オブジェクトが初期化されたときに呼び出されるメソッド
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _fadeAnimation = GetComponent<FadeAnimation>();
        this.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    /// <summary>
    /// 指定されたシーンをフェードアウトしながらロードします
    /// </summary>
    /// <param name="sceneName">ロードするシーンの名前</param>
    public async void LoadScene(SceneName sceneName)
    {
        if (IsFading) return;

        // フェードアウトを実行
        await _fadeAnimation.FadeOut();

        // 指定されたシーンをロード
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneNameDictionary[sceneName]);

        // フェードインを実行
        await _fadeAnimation.FadeIn();

        _currentScene = sceneName;
    }
}

/// <summary>
/// シーンの名前を列挙する列挙型
/// </summary>
public enum SceneName
{
    Title,
    Setting,
    Game,
    Result,
}
