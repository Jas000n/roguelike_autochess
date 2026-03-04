using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class BuildScript
{
    public static void BuildMac()
    {
        var projectRoot = Directory.GetCurrentDirectory();
        var buildDir = Path.Combine(projectRoot, "Builds", "Mac");
        Directory.CreateDirectory(buildDir);

        var scenePath = "Assets/Scenes/Main.unity";
        if (!File.Exists(scenePath))
        {
            Directory.CreateDirectory(Path.Combine(projectRoot, "Assets", "Scenes"));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, scenePath);
        }

        var options = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = Path.Combine(buildDir, "DragonChessLegends.app"),
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new System.Exception("Build failed: " + report.summary.result);
        }

        UnityEngine.Debug.Log("Build succeeded: " + options.locationPathName);
    }
}
