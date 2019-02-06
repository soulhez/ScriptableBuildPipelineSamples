using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Build.Player;
using UnityEngine;

using BuildCompression = UnityEngine.BuildCompression;

public static class PerBundleBuildLocation
{
	/// <summary>
	/// Example of using a custom BuildParameters class to override the GetOutputFilePathForIdentifier method
	/// This method is called during the ArchiveAndCompressBundles task to enquire about what final location
	/// the task should save AssetBundle to.
	/// </summary>
	[MenuItem( "SBP Samples/With per bundle Location" )]
	public static void Build()
	{
		var buildContent = new BundleBuildContent( ContentBuildInterface.GenerateAssetBundleBuilds() );
		var buildParams = new CustomBuildParameters( EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.selectedBuildTargetGroup, "Assets/StreamingAssets" );
		
		// This is a part of the CustomBuildParameters class that override the outputFolder on a per AssetBundle basic
		buildParams.m_PerBundleBuildFolder.Add( "textures", "Assets/StreamingAssets/OtherFolder" );
		
		// This is a part of the CustomBuildParameters class that ignores the outputFolder, and gives a direct location, including the filename. On a per AssetBundle basic
		buildParams.m_PerBundleAbsolutePath.Add( "prefabs", Application.dataPath.Substring( 0,Application.dataPath.Length-6 ) + "myPrefabObjects.ab" );

		buildParams.BundleCompression = BuildCompression.LZMA;
		
		List<IBuildTask> taskList = DefaultBuildTasks.Create(DefaultBuildTasks.Preset.AssetBundleBuiltInShaderExtraction) as List<IBuildTask>;
		//taskList.Add( new MoveAssetBundles() ); // no longer needed from "1.2.3-preview" as can use 
		taskList.Add( new RefreshAssetDatabase() );
		
		IBundleBuildResults results;
		ReturnCode exitCode = ContentPipeline.BuildAssetBundles( buildParams, buildContent, out results, taskList );
		Debug.Log( "Building per bundle completed with " + exitCode );
	}
}