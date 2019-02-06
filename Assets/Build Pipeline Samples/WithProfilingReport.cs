using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Build.Player;
using UnityEngine;

using BuildCompression = UnityEngine.BuildCompression;

public static class WithProfilingReport
{
	private static string folderPath = "Assets/StreamingAssets";
	const string k_TmpPath = "buildpipeline_tmp";
	
	[MenuItem( "SBP Samples/With Profiler" )]
	public static void Build()
	{
		if (Directory.Exists(folderPath))
			Directory.Delete(folderPath, true);
		if (Directory.Exists(k_TmpPath))
			Directory.Delete(k_TmpPath, true);

		Directory.CreateDirectory(folderPath);
		Directory.CreateDirectory(k_TmpPath);

		IBundleBuildParameters buildParams = new BundleBuildParameters(EditorUserBuildSettings.activeBuildTarget, BuildTargetGroup.Unknown, folderPath);
		buildParams.TempOutputFolder = k_TmpPath;

		IBundleBuildContent buildContent = new BundleBuildContent(ContentBuildInterface.GenerateAssetBundleBuilds());
		IBundleBuildResults results;

		List<IBuildTask> taskList = DefaultBuildTasks.Create(DefaultBuildTasks.Preset.AssetBundleBuiltInShaderExtraction) as List<IBuildTask>;
		taskList.Add( new RefreshAssetDatabase() );
		taskList.Add( new CreateBuiltTimeReport() );
        
		ReturnCode exitCode = ContentPipelineProfiled.BuildAssetBundles(buildParams, buildContent, out results, taskList, new Profiler( "Total") );
		Debug.Log( "Building completed with " + exitCode );
	}
}





