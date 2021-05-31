using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    private static SceneManager instance;  // 私有单例
    // private static readonly object lockObj = new object();

    private Action onSceneLoaded = null;   // 场景加载完成回调
    private string nextSceneName = null;  // 将要加载的场景名
    private string curSceneName = null;   // 当前场景名，如若没有场景，则默认返回 Login
    private string preSceneName = null;   // 上一个场景名

    private GameObject loadProgressObj = null; // 加载进度显示对象

    private bool isLoading = false;     // 是否正在加载中
    private bool isAutoDestroy = true;  // 自动删除 loading 背景

    private const string loadSceneName = "LoadingScene";  // 加载场景名字

    //获取当前场景名
    public string loadedSceneName => instance.curSceneName;

    public static void CreateInstance(GameObject obj)
    {
        if(instance!=null){
            return;
        }
        instance = obj.AddComponent<SceneManager>();
        DontDestroyOnLoad(instance);
        instance.curSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public static void LoadPreScene()
    {
        if (string.IsNullOrEmpty(instance.preSceneName))
        {
            return;
        }
        LoadScene(instance.preSceneName);
    }

    public static void LoadScene(string strLevelName)
    {
        instance.LoadLevel(strLevelName, null);
    }

    public static void LoadScene(string strLevelName, Action onSecenLoaded)
    {
        instance.LoadLevel(strLevelName, onSecenLoaded);
    }

    private void LoadLevel(string strLevelName, Action onSecenLoaded, bool isDestroyAuto = true)
    {
        if (isLoading || curSceneName == strLevelName)
        {
            return;
        }

        isLoading = true;  // 锁屏
                           // *开始加载    
        onSceneLoaded = onSecenLoaded;
        nextSceneName = strLevelName;
        preSceneName = curSceneName;
        curSceneName = loadSceneName;
        isAutoDestroy = isDestroyAuto;

        //先异步加载 Loading 界面
        StartCoroutine(StartLoadSceneOnEditor(loadSceneName, OnLoadingSceneLoaded, null));
    }

    /**************************************
    * @fn OnLoadingSceneLoaded
    * @brief 过渡场景加载完成回调
    * @return void
    **************************************/
    private void OnLoadingSceneLoaded()
    {
        // 过渡场景加载完成后加载下一个场景
        StartCoroutine(StartLoadSceneOnEditor(nextSceneName, OnNextSceneLoaded, OnNextSceneProgress));
    }

    /**************************************
    * @fn StartLoadSceneOnEditor
    * @brief 开始加载
    * @param[in] string strLevelName
    * @param[in] Action OnSecenLoaded  场景加载完成后回调
    * @param[in] Action OnSceneProgress
    * @return System.Collections.Generic.IEnumerator
    **************************************/
    private IEnumerator StartLoadSceneOnEditor(string strLevelName, Action OnSecenLoaded, Action<float> OnSceneProgress)
    {
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(strLevelName);
        if (null == async)
        {
            yield break;
        }

        //*加载进度
        while (!async.isDone)
        {
            float fProgressValue;
            if (async.progress < 0.9f)
            {
                fProgressValue = async.progress;
            }
            else
            {
                fProgressValue = 1.0f;
            }
            OnSceneProgress?.Invoke(fProgressValue);
            yield return null;
        }
        OnSecenLoaded?.Invoke();
    }

    /**************************************
    * @fn OnNextSceneLoaded
    * @brief 加载下一场景完成回调
    * @return void
    **************************************/
    private void OnNextSceneLoaded()
    {
        isLoading = false;
        OnNextSceneProgress(1);
        curSceneName = nextSceneName;
        nextSceneName = null;
        onSceneLoaded?.Invoke();
    }

    /**************************************
    * @fn OnNextSceneProgress
    * @brief 场景加载进度变化
    * @param[in] float fProgress
    * @return void
    **************************************/
    private void OnNextSceneProgress(float fProgress)
    {
        if (loadProgressObj == null)
        {
            loadProgressObj = GameObject.Find("ProgressBar");
        }
        Slider progressBar = null;
        if (loadProgressObj == null)
        {
            return;
        }
        loadProgressObj.TryGetComponent<Slider>(out progressBar);
        progressBar.value = fProgress;
    }
}