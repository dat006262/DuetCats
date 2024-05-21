#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenScene
{
    //[MenuItem("OpenScene/Release/Loading")]
    //private static void OpenSceneLoading()
    //{
    //    EditorSceneManager.OpenScene("Assets/Templates/Scenes/Loading.unity");
    //}

    //[MenuItem("OpenScene/Release/GamePlay")]
    //private static void OpenSceneGamePlay()
    //{
    //    EditorSceneManager.OpenScene("Assets/Scenes/GamePlay.unity");
    //}

    //[MenuItem("OpenScene/Release/Home")]
    //private static void OpenSceneHome()
    //{
    //    EditorSceneManager.OpenScene("Assets/Templates/Scenes/Home.unity");
    //}

    //[MenuItem("Window/UnityLog/Enable")]
    //public static void EnableLog()
    //{
    //    UnityEngine.Debug.unityLogger.logEnabled = true;
    //    var valueCompilation = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup);
    //    const string str = "SHOW_LOG";
    //    if (!valueCompilation.Split(';').Contains(str))
    //    {
    //        valueCompilation += ";";
    //        valueCompilation += str;
    //        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup, valueCompilation);
    //    }
    //}

    //[MenuItem("Window/UnityLog/Disable")]
    //public static void DisableLog()
    //{
    //    UnityEngine.Debug.unityLogger.logEnabled = false;
    //    var valueCompilation = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup);
    //    const string str = "SHOW_LOG";
    //    var list = valueCompilation.Split(';').ToList();
    //    if (list.Contains(str))
    //    {
    //        list.Remove(str);
    //    }
    //    else
    //    {
    //        return;
    //    }

    //    valueCompilation = string.Empty;
    //    foreach (var s in list)
    //    {
    //        valueCompilation += s;
    //        valueCompilation += ';';
    //    }

    //    var remove = valueCompilation.Remove(valueCompilation.Length - 1, 1);
    //    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup, remove);
    //}

    private static BuildTargetGroup BuildTargetGroup
    {
        get
        {
#if UNITY_ANDROID
            return BuildTargetGroup.Android;
#elif UNITY_IOS
                return BuildTargetGroup.iOS;
#else
            return BuildTargetGroup.Standalone;
#endif
        }
    }
}

#endif