using System.IO;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Build.Pipeline;

/*
 * This is an example of how you might easily convert old buildpipeline code to the new pipeline with minimal code changes.
 */

public static class BuildCompatibilityAssetBundles
{
    private static string folderPath = "Assets/StreamingAssets";

    [MenuItem( "SBP Samples/Build Asset Bundles - Compatiblilty" )]
    public static void Build()
    {
        BuildAssetBundles( folderPath, false, CompressionType.Lz4, EditorUserBuildSettings.activeBuildTarget );
        AssetDatabase.Refresh();
    }
    
    public static bool BuildAssetBundles( string outputPath, bool forceRebuild, bool useChunkBasedCompression, BuildTarget buildTarget )
    {
        CompatibilityAssetBundleManifest manifest = BuildAssetBundles( outputPath, forceRebuild, useChunkBasedCompression ? CompressionType.Lz4 : CompressionType.Lzma, buildTarget );
        return manifest != null;
    }
    
    public static CompatibilityAssetBundleManifest BuildAssetBundles( string outputPath, bool forceRebuild, CompressionType compressionMode, BuildTarget buildTarget )
    {
        var options = BuildAssetBundleOptions.None;
        switch( compressionMode )
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

        Directory.CreateDirectory(outputPath);
        
        return CompatibilityBuildPipeline.BuildAssetBundles( outputPath, options, buildTarget );
    }
}
