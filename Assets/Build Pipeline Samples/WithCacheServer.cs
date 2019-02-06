using System.IO;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine.Build.Pipeline;
using BuildCompression = UnityEngine.BuildCompression;
using CompressionType = UnityEngine.CompressionType;

public static class WithCacheServer
{
    private static string folderPath = "Assets/StreamingAssets";
    
    [MenuItem( "SBP Samples/With CacheServer connection" )]
    public static void Build()
    {
        IBundleBuildResults results;
        BuildAssetBundles( folderPath, CompressionType.Lz4, EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.selectedBuildTargetGroup, out results );
        AssetDatabase.Refresh();
    }
    
    public static ReturnCode BuildAssetBundles( string outputPath, CompressionType compressionMode, BuildTarget buildTarget, BuildTargetGroup buildGroup, out IBundleBuildResults results )
    {
        BundleBuildContent buildContent = new BundleBuildContent(ContentBuildInterface.GenerateAssetBundleBuilds());
        BundleBuildParameters buildParams = new BundleBuildParameters(buildTarget, buildGroup, outputPath);
        SetupBuildCacheServer( buildParams );
        
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

    /// <summary>
    /// The cache server serving ScriptableBuildPipeline must be dedicated to the build process.
    /// Be sure to not mix the two. Due to how the cache server stores the data, data collisions are
    /// like and problems will occur
    ///
    /// see: https://github.com/Unity-Technologies/unity-cache-server
    /// </summary>
    public static void SetupBuildCacheServer( BundleBuildParameters buildParams )
    {
        // setup to a cache server running locally with ip set to 8127
        // we can set the port when starting the cache server as showing in https://github.com/Unity-Technologies/unity-cache-server#usage with -p
        buildParams.UseCache = true;
        buildParams.CacheServerHost = "127.0.0.1";
        buildParams.CacheServerPort = 8127; // 8126 is default, 
    }
}
