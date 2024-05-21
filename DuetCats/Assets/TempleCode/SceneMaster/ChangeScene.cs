#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;


public class ChangeScene : Editor
{
    [MenuItem("Open Scene/CreateSound #1")]
    public static void OpenLoading()
    {
        OpenScene(Const.SCENE_CREATSOUND);
    }



    [MenuItem("Open Scene/Game #3")]
    public static void OpenGame()
    {
        OpenScene(Const.SCENE_GAME);
    }

    //[MenuItem("Open Scene/Fong.Test #4")]
    //public static void OpenPhongDev()
    //{
    //    OpenScene(Const.SCENE_PHONG_DEV);
    //}
    private static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Game/Scenes/" + sceneName + ".unity");
        }
    }
}
#endif
