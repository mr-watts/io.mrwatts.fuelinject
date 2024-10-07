#if UNITY_EDITOR

#pragma warning disable

using System;
using UnityEditor;
using System.Reflection;
using Microsoft.Unity.VisualStudio.Editor;

/// <summary>
/// Helper script that can be called from the command line to generate C# SDK-style projects.
/// </summary>
public sealed class TriggerCSharpProjectEditorScript
{
    public static void GenerateCSharpSDKStyleProjects()
    {
        UnityEngine.Debug.Log($"Executing {nameof(GenerateCSharpSDKStyleProjects)}...");

        Type sdkStyleProjectGenerationType = typeof(VisualStudioEditor).Assembly.GetType("Microsoft.Unity.VisualStudio.Editor.SdkStyleProjectGeneration");

        object sdkStyleProjectGeneration = Activator.CreateInstance(sdkStyleProjectGenerationType);

        MethodInfo syncMethod = sdkStyleProjectGenerationType.GetMethod("Sync");
        syncMethod.Invoke(sdkStyleProjectGeneration, null);
    }
}
#endif