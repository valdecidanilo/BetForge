using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLoader
{

    #region SceneLoaderHelper

    public class SceneLoaderHelper : MonoBehaviour
    {
        private static SceneLoaderHelper _instance;
        public static SceneLoaderHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject helperObject = new GameObject("SceneLoaderHelper");
                    _instance = helperObject.AddComponent<SceneLoaderHelper>();
                    DontDestroyOnLoad(helperObject);
                }
                return _instance;
            }
        }
    }
    #endregion

    #region WebGLSceneManager

    public static class WebGLSceneManager
    {

        #region LoadScenes Methods

        /// <summary>
        /// Loads a primary scene and a list of additive scenes asynchronously.
        /// Ensures that already loaded scenes are not reloaded.
        /// </summary>
        public static void LoadScenes(string primarySceneName, List<string> additiveSceneNames, Action OnCompleteLoad = null)
        {
            LoadScenes(primarySceneName, additiveSceneNames, null, OnCompleteLoad);
        }

        /// <summary>
        /// Overload: Loads a primary scene and a list of additive scenes asynchronously.
        /// Calls OnBeforeLoad before starting the process.
        /// </summary>
        public static void LoadScenes(string primarySceneName, List<string> additiveSceneNames, Action OnBeforeLoad, Action OnCompleteLoad)
        {
            Scene primaryScene = SceneManager.GetSceneByName(primarySceneName);
            bool needsPrimarySceneLoad = !primaryScene.isLoaded;

            List<string> scenesToLoad = additiveSceneNames.FindAll(sceneName =>
            {
                Scene s = SceneManager.GetSceneByName(sceneName);
                return !s.isLoaded;
            });

            if (!needsPrimarySceneLoad && scenesToLoad.Count == 0)
            {
                Debug.Log("All requested scenes are already loaded. Skipping load process.");
                return;
            }

            OnBeforeLoad?.Invoke();

            SceneLoaderHelper.Instance.StartCoroutine(LoadScenesCoroutine(primarySceneName, scenesToLoad, OnCompleteLoad));
        }

        private static IEnumerator LoadScenesCoroutine(string primarySceneName, List<string> additiveSceneNames, Action OnCompleteLoad)
        {
            Scene primaryScene = SceneManager.GetSceneByName(primarySceneName);
            if (!primaryScene.isLoaded)
            {
                AsyncOperation primaryOp = SceneManager.LoadSceneAsync(primarySceneName, LoadSceneMode.Single);
                primaryOp.allowSceneActivation = true;
                while (!primaryOp.isDone)
                {
                    yield return null;
                }
            }
            
            if (additiveSceneNames.Count == 0)
            {
                OnCompleteLoad?.Invoke();
                yield break;
            }

            int loadedScenes = 0;
            foreach (string sceneName in additiveSceneNames)
            {
                LoadAdditiveScene(sceneName, () =>
                {
                    loadedScenes++;
                    if (loadedScenes == additiveSceneNames.Count)
                    {
                        OnCompleteLoad?.Invoke();
                    }
                });
            }        
        }

        #endregion

        #region LoadAdditiveScene Methods

        /// <summary>
        /// Loads a single additive scene asynchronously.
        /// If already loaded, calls OnCompleteLoad immediately.
        /// </summary>
        public static void LoadAdditiveScene(string additiveSceneName, Action OnCompleteLoad = null)
        {
            Scene additiveScene = SceneManager.GetSceneByName(additiveSceneName);
            if (additiveScene.isLoaded)
            {
                Debug.Log($"Additive scene '{additiveSceneName}' is already loaded. Skipping reload.");
                return;
            }

            SceneLoaderHelper.Instance.StartCoroutine(LoadAdditiveSceneCoroutine(additiveSceneName, null, OnCompleteLoad));
        }

        public static void LoadAdditiveScene(string additiveSceneName, Action OnBeforeLoad, Action OnCompleteLoad)
        {
            Scene additiveScene = SceneManager.GetSceneByName(additiveSceneName);
            if (additiveScene.isLoaded)
            {
                Debug.Log($"Additive scene '{additiveSceneName}' is already loaded. Skipping reload.");
                return;
            }

            SceneLoaderHelper.Instance.StartCoroutine(LoadAdditiveSceneCoroutine(additiveSceneName, OnBeforeLoad, OnCompleteLoad));
        }

        private static IEnumerator LoadAdditiveSceneCoroutine(string additiveSceneName, Action OnBeforeLoad, Action OnCompleteLoad)
        {
            OnBeforeLoad?.Invoke();

            AsyncOperation additiveOp = SceneManager.LoadSceneAsync(additiveSceneName, LoadSceneMode.Additive);
            while (!additiveOp.isDone)
            {
                yield return null;
            }

            OnCompleteLoad?.Invoke();
        }

        #endregion

        #region UnloadScene Methods

        /// <summary>
        /// Unloads a scene if it is currently loaded.
        /// Calls OnCompleteUnload immediately if the scene is already unloaded.
        /// </summary>
        public static void UnloadScene(string sceneName, Action OnCompleteUnload = null)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.Log($"Scene '{sceneName}' is not loaded. Skipping unload.");
                return;
            }

            SceneLoaderHelper.Instance.StartCoroutine(UnloadSceneCoroutine(sceneName, null, OnCompleteUnload));
        }

        public static void UnloadScene(string sceneName, Action OnBeforeUnload, Action OnCompleteUnload)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.Log($"Scene '{sceneName}' is not loaded. Skipping unload.");
                return;
            }

            SceneLoaderHelper.Instance.StartCoroutine(UnloadSceneCoroutine(sceneName, OnBeforeUnload, OnCompleteUnload));
        }

        private static IEnumerator UnloadSceneCoroutine(string sceneName, Action OnBeforeUnload, Action OnCompleteUnload)
        {
            OnBeforeUnload?.Invoke();

            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
            while (unloadOp != null && !unloadOp.isDone)
            {
                yield return null;
            }

            OnCompleteUnload?.Invoke();
        }

        #endregion

        #region ReloadAdditiveScene Method

        public static void ReloadAdditiveScene(string additiveSceneName, Action OnBeforeReload = null, Action OnCompleteReload = null)
        {
            Scene scene = SceneManager.GetSceneByName(additiveSceneName);
            if (scene.isLoaded)
            {
                OnBeforeReload?.Invoke();
                UnloadScene(additiveSceneName, () =>
                {
                    LoadAdditiveScene(additiveSceneName, OnCompleteReload);
                });
            }
            else
            {
                LoadAdditiveScene(additiveSceneName, OnCompleteReload);
            }
        }

        #endregion

    }

    #endregion
}
