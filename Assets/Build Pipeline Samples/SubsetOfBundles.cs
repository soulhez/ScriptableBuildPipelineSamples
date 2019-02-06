using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Build.Pipeline.WriteTypes;
using UnityEngine;

/// <summary>
/// In this example we hook into the BuildPipeline at the point where the build map has been generated but no
/// AssetBundles have yet been created.
/// Then we skip bundles that we do not want to create during the final steps of creating the actual bundle
/// Because the full BuildMap is generated before hand. If you were to create selected AssetBundles from two copies
/// of the same project. Dependencies between AssetBundles will be maintained.
/// </summary>
public static class SubsetOfBundles
{
    private static string folderPath = "Assets/StreamingAssets";
    const string k_TmpPath = "buildpipeline_tmp";
    private static List<string> includedBundles = null;

    /// <summary>
    /// In this example we additionally use command line arguments to determine what AssetBundles to build
    /// This will be a likely component of this use-case, for use on build machine.
    /// </summary>
    private static void ReadCommandLine()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for( int i = 0; i < args.Length; i++ )
        {
            if( args[i] == "-buildFolder" )
            {
                folderPath = args[i + 1];
            }
            else if( args[i] == "-buildBundles" )
            {
                string csv = args[i + 1];
                includedBundles = new List<string>( csv.Split( ',' ) );
            }
        }
    }
    
    [MenuItem("SBP Samples/Build with selective Bundles")]
    public static void Build()
    {
        ReadCommandLine();
        
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
        
        // we add a callback after generating information about how to build each bundle
        ContentPipeline.BuildCallbacks.PostPackingCallback += PostPackingCallback;
        
        ReturnCode exitCode = ContentPipeline.BuildAssetBundles(buildParams, buildContent, out results, taskList);
        Debug.Log( "Building completed with " + exitCode );
    }

    /// <summary>
    /// This callback is what remove the unwanted AssetBundle writing operation from being processed with the WriteSerializedFiles
    /// and ArchiveAndCompressBundles taskes
    /// </summary>
    /// <param name="buildParams"></param>
    /// <param name="dependencyData"></param>
    /// <param name="writeData"></param>
    /// <returns></returns>
    private static ReturnCode PostPackingCallback( IBuildParameters buildParams, IDependencyData dependencyData, IWriteData writeData )
    {
        if( includedBundles != null && includedBundles.Count > 0 )
        {
            for( int i=writeData.WriteOperations.Count-1; i>=0; --i )
            {
                // get the AssetBundle name that the writeOperation is for
                AssetBundleWriteOperation op = writeData.WriteOperations[i] as AssetBundleWriteOperation;
                string bundleName = null;
                if( op != null )
                    bundleName = op.Info.bundleName;
                else
                {
                    // scene bundles are also different
                    if( writeData.WriteOperations[i] is SceneBundleWriteOperation s_op )
                        bundleName = s_op.Info.bundleName;
                    else
                    {
                        Debug.LogError( "Unexpected write operation" );
                        return ReturnCode.Error;
                    }
                }
                
                // if we do not want to build that bundle, remove the write operation from the list
                if( includedBundles.Contains( bundleName ) == false )
                    writeData.WriteOperations.RemoveAt( i );
            }
        }
        
        return ReturnCode.Success;
    }
}
