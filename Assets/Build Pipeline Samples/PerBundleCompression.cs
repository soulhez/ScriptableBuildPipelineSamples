using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Player;
using UnityEngine;

using BuildCompression = UnityEngine.BuildCompression;

public static class PerBundleCompression
{
	/// <summary>
	/// Example of using a custom BuildParameters class to override the GetCompressionForIdentifier
	/// This method is called during the ArchiveAndCompressBundles task to enquire about what compression format
	/// the task should use on that AssetBundle.
	/// </summary>
	[MenuItem( "SBP Samples/With per bundle compression" )]
	public static void Build()
	{
		var buildContent = new BundleBuildContent( ContentBuildInterface.GenerateAssetBundleBuilds() );
		var buildParams = new CustomBuildParameters( EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.selectedBuildTargetGroup, "Assets/StreamingAssets" );
		
		// set three different Assetbundles to be the different compression options available
		buildParams.m_PerBundleCompression.Add( "textures", BuildCompression.LZMA );
		buildParams.m_PerBundleCompression.Add( "objects", BuildCompression.LZ4 );
		buildParams.m_PerBundleCompression.Add( "prefabs", BuildCompression.Uncompressed );

		buildParams.BundleCompression = BuildCompression.LZMA;
		
		IBundleBuildResults results;
		ReturnCode exitCode = ContentPipeline.BuildAssetBundles( buildParams, buildContent, out results );
		Debug.Log( "Building per bundle completed with " + exitCode );
	}
}