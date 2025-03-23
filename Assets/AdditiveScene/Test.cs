using SceneLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SceneBenchmark : MonoBehaviour
{
    private List<string> Scene_To_Load = new List<string> { "BetForm", "UI" };

    void Start()
    {
        BenchmarkMethod("LoadScenes", () =>
        {
            WebGLSceneManager.LoadScenes("Core", Scene_To_Load,
                () => { Debug.Log("Preparing to load scenes."); }, // OnBeforeLoad event
                () => { Debug.Log("All scenes loaded successfully!"); }); // OnCompleteLoad event
        });
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            BenchmarkMethod("LoadAdditiveScene (Bonus)", () =>
            {
                WebGLSceneManager.LoadAdditiveScene("Bonus",
                    () => { Debug.Log("Preparing bonus scene."); }, // OnBeforeLoad event
                    () => { Debug.Log("Bonus scene loaded successfully!"); }); // OnCompleteLoad event
            });
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            BenchmarkMethod("UnloadScene (Bonus)", () =>
            {
                WebGLSceneManager.UnloadScene("Bonus",
                    () => { Debug.Log("Unloading Scene"); }, // OnBeforeLoad event
                    () => { Debug.Log("Bonus was unloaded successfully!"); }); // OnCompleteLoad event
            });
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            BenchmarkMethod("ReloadAdditiveScene (Bonus)", () =>
            {
                WebGLSceneManager.ReloadAdditiveScene("Bonus",
                    () => { Debug.Log("Reloading Scene"); }, // OnBeforeLoad event
                    () => { Debug.Log("Bonus was reloaded successfully!"); }); // OnCompleteLoad event
            });
        }
    }

    private void BenchmarkMethod(string methodName, Action methodToBenchmark)
    {
        StartCoroutine(BenchmarkCoroutine(methodName, methodToBenchmark));
    }

    private IEnumerator BenchmarkCoroutine(string methodName, Action methodToBenchmark)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long memoryBefore = GC.GetTotalMemory(true);
        Stopwatch stopwatch = Stopwatch.StartNew();

        methodToBenchmark();

        yield return new WaitForEndOfFrame();

        stopwatch.Stop();
        long memoryAfter = GC.GetTotalMemory(true);

        Debug.Log($"<color=green> {methodName} - Execution Time: {stopwatch.ElapsedMilliseconds} ms, Memory Allocated: {FormatMemorySize(memoryAfter - memoryBefore)} </color>");
    }

    private string FormatMemorySize(long bytes)
    {
        return $"{bytes / 1_048_576f:F2} MB"; // Convert bytes to MB with 2 decimal places
    }

}

