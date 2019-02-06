using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using BuildCompression = UnityEngine.BuildCompression;

/*
 * This is an example of how you might setup a custom class for inheriting and changing the BuildParameters class
 */

[Serializable]
public class CustomBuildParameters : BundleBuildParameters
{
	public Dictionary<string, BuildCompression> m_PerBundleCompression;
	public Dictionary<string, string> m_PerBundleBuildFolder;
	public Dictionary<string, string> m_PerBundleAbsolutePath;
	
	public CustomBuildParameters(BuildTarget target, BuildTargetGroup group, string outputFolder) : base(target, group, outputFolder)
	{
		m_PerBundleCompression = new Dictionary<string, BuildCompression>();
		m_PerBundleBuildFolder = new Dictionary<string, string>();
		m_PerBundleAbsolutePath = new Dictionary<string, string>();
	}
	
	/// <summary>
	/// This is called when Compressing the AssetBundle contents to determine which UnityEngine.BuildCompression method
	/// to use. By default this will use what is set in BundleCompression property.
	/// This allows mixing of compression types within the same build of AssetBundles.
	/// e.g. LZMA for bundles you want to download and LZ4 for bundles in SteamingAssets
	///
	/// See PerBundleCompression.cs for an example of usage
	/// </summary>
	/// <param name="identifier"></param>
	/// <returns></returns>
	public override BuildCompression GetCompressionForIdentifier(string identifier)
	{
		BuildCompression value;
		
		if( m_PerBundleCompression.TryGetValue( identifier, out value ) )
			return value;
	
		return BundleCompression;
	}

	/// <summary>
	/// This is called after Compressing and Archiving the AssetBundle to determine where the AssetBundle should be output
	/// This can be handy if you have a large complicated collection of AssetBundles and need more control about the folder
	/// structure for storing the resulting AssetBundles.
	/// An simple example using maybe to store some in StreamingAssets and others outside of the project to be uploaded to a server
	///
	/// See PerBundleBuildLocation.cs for an example usage
	/// </summary>
	/// <param name="identifier"></param>
	/// <returns></returns>
	public override string GetOutputFilePathForIdentifier( string identifier )
	{
		string value;

		if( m_PerBundleBuildFolder.TryGetValue( identifier, out value ) )
		{
			if (string.IsNullOrEmpty(value))
				Debug.LogError( "path for " + identifier + " cannot be null");

			return string.Format( "{0}/{1}", value, identifier );
		}
		
		if( m_PerBundleAbsolutePath.TryGetValue( identifier, out value ) )
		{
			return value;
		}

		return string.Format("{0}/{1}", OutputFolder, identifier);
	}
}