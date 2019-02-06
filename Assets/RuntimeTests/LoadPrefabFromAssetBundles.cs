using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPrefabFromAssetBundles : MonoBehaviour
{
    public List<string> m_assetBundlesToLoad;
    public string m_prefabContainedIn;
    public string m_prefabName;
    
    private List<AssetBundle> m_bundles = new List<AssetBundle>();
    private AssetBundle m_prefabBundle;
    
    void Start()
    {
        foreach( string streamingAssetBundlePath in m_assetBundlesToLoad )
        {
            AssetBundle b = LoadAssetBundle( streamingAssetBundlePath );
            if( b != null )
            {
                m_bundles.Add( b );
                if( streamingAssetBundlePath == m_prefabContainedIn )
                    m_prefabBundle = b;
            }
        }

        if( m_prefabBundle != null )
        {
            GameObject prefab = m_prefabBundle.LoadAsset<GameObject>( m_prefabName );
            Instantiate( prefab );
        }
    }

    private void OnDestroy()
    {
        foreach( AssetBundle bundle in m_bundles )
        {
            bundle.Unload( true );
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
