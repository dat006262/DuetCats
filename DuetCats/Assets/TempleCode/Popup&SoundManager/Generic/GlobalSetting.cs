using BulletHell;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GlobalSetting : SingletonMonoBehaviour<GlobalSetting>
{
    #region REF,INIT
    public SoundDataSO soundDataSO;
    public ParticalDataSO particalDataSO;
    #endregion

    #region DATA

    public SceneEnum sceneEnum = SceneEnum.LOADING;

    #endregion

    #region FUNC UNITY
    public override void Awake()
    {
        if (GlobalSetting.Instance != null)
        {
            if (GlobalSetting.Instance != this)
            {
                Destroy(gameObject);
                return;
            }

        }

        base.Awake();
        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        _CheckDevice.CheckDevice();
#if !UNITY_EDITOR
       // SetFPS();
#endif

        InitDoTween();
    }
    #endregion

    #region PUBLIC
    public static bool NetWorkRequirements()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    public static void LoadScene(SceneEnum _sceneEnum)
    {
        GlobalSetting.Instance.sceneEnum = _sceneEnum;
        switch (_sceneEnum)
        {
            case SceneEnum.LOADING: SceneManager.LoadScene(Const.SCENE_CREATSOUND); break;
            case SceneEnum.GAMEPLAY: SceneManager.LoadScene(Const.SCENE_GAME); break;
            default: break;
        }
    }

    #endregion

    #region PRIVATE
    private void SetFPS()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    private void InitDoTween()
    {
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly).SetCapacity(100, 10);
    }
    #endregion

    #region EVENT OUTPUT


    #endregion




}
public enum SceneEnum
{
    LOADING,
    HOME,
    GAMEPLAY,
    PIZZAGAME

}
public enum ChalengeMode
{
    LOCK,
    CANPLAY
}
public enum GameMode
{
    Normal,
    Challenge,
    Tutorial
}

public enum TypeTutorial
{
    NONE,
    HOWTOMOVETILE,
    HOWTOUNDO,
    HOWTOSUFFFER,
    HOWTOHINT
}


public static class _CheckDevice
{
    public static bool IsTable { get; private set; }

    private static float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = UnityEngine.Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

        return diagonalInches;
    }

    public static void CheckDevice()
    {
#if UNITY_IOS
    bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
            if (deviceIsIpad)
            {
                IsTable = true;
            }
            bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
            if (deviceIsIphone)
            {
                IsTable = false;
            }
#elif UNITY_ANDROID

        float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        IsTable = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);
#endif
    }
}

public class _LinkPrivacy
{
    [SerializeField] private string _linkTandC;
    [SerializeField] private string _linkPrivacy;
    [SerializeField] private string _linkGame;
    private string _linkShareGame;

    public void Init()
    {
        _linkShareGame = "https://play.google.com/store/apps/details?id=" + Application.identifier;
    }

    public string LinkGame => _linkGame;

    public void OpenTandC()
    {
        Application.OpenURL(_linkTandC);
    }

    public void OpenPrivacy()
    {
        Application.OpenURL(_linkShareGame == string.Empty ? _linkGame : _linkShareGame);
    }
}
public static class GameObjectCreate
{
    public static void ReturnNode(ref this Pool<ProjectileData>.Node node)
    {
        if (node.Active)
        {
            Debug.LogWarning("ReturnNode");
            node.Item.TimeToLive = -1;
            if (node.Item.Outline.Item != null)
            {
                node.Item.Outline.Item = null;
            }

            node.Active = false;
        }
    }
    public static GameObject CreateGameObject(this GameObject prefab, Transform parent)
    {
        var pos = prefab.transform.position;
        var rot = prefab.transform.rotation;
        return Object.Instantiate(prefab, pos, rot, parent);
    }
    public static T CreateInstance<T>(this T prefab,
    Transform parent = null) where T : MonoBehaviour
    {
        return parent is null
            ? Object.Instantiate(prefab)
            : Object.Instantiate(prefab, parent);
    }
    public static void SetActiveCanvasGroup(this CanvasGroup canvasGroup, bool isActive)
    {
        canvasGroup.alpha = isActive ? 1 : 0;
        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
    }
    public static Texture2D GetSlicedSpriteTexture(this Sprite sprite)
    {
        Rect rect = sprite.rect;
        Texture2D slicedTex = new Texture2D((int)rect.width, (int)rect.height);
        slicedTex.filterMode = sprite.texture.filterMode;

        slicedTex.SetPixels(0, 0, (int)rect.width, (int)rect.height, sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height));
        slicedTex.Apply();

        return slicedTex;
    }
}
