using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneFromAssetBundles : MonoBehaviour
{
    public List<string> m_assetBundlesToLoad;
    public string m_sceneName;
    
    private List<AssetBundle> m_bundles = new List<AssetBundle>();
    
    void Start()
    {
        foreach( string streamingAssetBundlePath in m_assetBundlesToLoad )
        {
            AssetBundle b = LoadAssetBundle( streamingAssetBundlePath );
            if( b != null )
                m_bundles.Add( b );
        }
        
        LoadSceneContainingStringFromAssetBundle( m_sceneName );
    }

    private void OnDestroy()
    {
        foreach( AssetBundle bundle in m_bundles )
        {
            bundle.Unload( true );
        }
    }

    public void LoadSceneContainingStringFromAssetBundle( string sceneString )
    {
        foreach( AssetBundle assetBundle in m_bundles )
        {
            if( assetBundle.isStreamedSceneAssetBundle )
            {
                string[] scenePaths = assetBundle.GetAllScenePaths();
                foreach( string path in scenePaths )
                {
                    if( path.Contains( sceneString ) )
                    {
                        SceneManager.LoadScene( path, LoadSceneMode.Additive );
                        return;
                    }
                }
            }
        }
    }

    private static string StreamingAssetsPath( string relativePath )
    {
        return System.IO.Path.Combine( Application.streamingAssetsPath, relativePath );
    }
    
    public static AssetBundle LoadAssetBundle( string relativePath )
    {
        return AssetBundle.LoadFromFile( StreamingAssetsPath( relativePath ) );
    }
}
