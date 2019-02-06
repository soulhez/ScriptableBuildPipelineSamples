using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using BuildCompression = UnityEngine.BuildCompression;
using CompressionType = UnityEngine.CompressionType;

/*
 * The new ScriptableBuildPipeline is strict about how you identify assets within the AssetBundle.
 * By default this can only be done by using the full filepath e.g. "Assets/Gameplay/Bots/SuperBot.prefab"
 * This could be changed to what ever you need. e.g. If you have a way of identifying assets by GUID, you could set the Address
 * to the GUID of the asset then use AssetBundle.LoadAsset<GameObject>( <GUID> );
 */

public static class AssignAddressableNames
{
    private static string folderPath = "Assets/StreamingAssets";
    
    [MenuItem( "SBP Samples/With Addressable Names" )]
    public static void Build()
    {
        IBundleBuildResults results;
        ReturnCode r = BuildAssetBundles( folderPath, CompressionType.Lz4, EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.selectedBuildTargetGroup, out results );
        Debug.Log( "Build with AddressableName complete with " + r );
        AssetDatabase.Refresh();
    }
    
    /// <summary>
    /// Using the Compatible code path, this setup is limited to be the same as legacy pipeline.
    /// With this setup an array of content for each AssetBundle is passed into the pipeline.
    /// ContentBuildInterface.GeneratAssetBundleBuilds can be used to get an array for the bundleNames
    /// set in the AssetImporters in your project (as seen at the bottom of the inspector when selecting an asset)
    /// There are two arrays,
    /// .assetNames which contains the fullpath to the Asset to be included
    /// .addressableNames which is the string used when loading the Asset.
    /// These are connected by index, so assigning .addressableNames[8] = "Robo" is assigning the asset at .assetNames[8]
    /// to load via AssetBundle.LoadAsset<T>( "Robo" );
    /// </summary>
    /// <param name="outputPath"></param>
    /// <param name="forceRebuild"></param>
    /// <param name="compression"></param>
    /// <param name="buildTarget"></param>
    /// <returns></returns>
    public static bool BuildCompatibilityAssetBundles(string outputPath, bool forceRebuild, CompressionType compression, BuildTarget buildTarget)
    {
        var options = BuildAssetBundleOptions.None;
        
        switch( compression )
        {
            case CompressionType.None:
                options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                break;
            case CompressionType.Lz4:
                options |= BuildAssetBundleOptions.ChunkBasedCompression;
                break;
        }

        if (forceRebuild)
            options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;

        AssetBundleBuild[] bundles = ContentBuildInterface.GenerateAssetBundleBuilds();
        
        // go through each asset in the bundle and assign the addressable name to filename
        for (int i = 0; i < bundles.Length; i++)
            bundles[i].addressableNames = bundles[i].assetNames.Select(Path.GetFileNameWithoutExtension).ToArray();

        var manifest = CompatibilityBuildPipeline.BuildAssetBundles( outputPath, bundles, options, buildTarget );
        return manifest != null;
    }
    
    /// <summary>
    /// The BundleBuildContent class contains information about all of the Assets you want to build into the BuildMap
    /// Assets are referenced to by a GUID object, and the Addresses can be obtained and modified by using a GUID to
    /// refer to each asset and identify its Address
    /// </summary>
    /// <param name="outputPath"></param>
    /// <param name="compressionMode"></param>
    /// <param name="buildTarget"></param>
    /// <param name="buildGroup"></param>
    /// <param name="results"></param>
    /// <returns></returns>
    public static ReturnCode BuildAssetBundles( string outputPath, CompressionType compressionMode, BuildTarget buildTarget, BuildTargetGroup buildGroup, out IBundleBuildResults results )
    {
        BundleBuildContent buildContent = new BundleBuildContent(ContentBuildInterface.GenerateAssetBundleBuilds());

        // Go through assets content and set their address to its filename
        for( int i = 0; i < buildContent.Assets.Count; ++i )
        {
            GUID g = buildContent.Assets[i];
            // Get the current address as the full filepath and change it to just be the filename
            buildContent.Addresses[g] = Path.GetFileNameWithoutExtension( buildContent.Addresses[g] );
        }

        BundleBuildParameters buildParams = new BundleBuildParameters(buildTarget, buildGroup, outputPath);
        
        switch( compressionMode )
        {
            case CompressionType.None:
                buildParams.BundleCompression = BuildCompression.Uncompressed;
                break;
            case CompressionType.Lz4:
                buildParams.BundleCompression = BuildCompression.LZ4;
                break;
            default:
                buildParams.BundleCompression = BuildCompression.LZMA;
                break;
        }

        return ContentPipeline.BuildAssetBundles( buildParams, buildContent, out results );
    }
}
