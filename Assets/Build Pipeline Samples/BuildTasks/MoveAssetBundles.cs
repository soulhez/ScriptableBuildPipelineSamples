using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

/* This task is no longer needed. IBuildParameters.GetOutputFilePathForIdentifier was added. Allowing you to easily specify the
 output location of any individual AssetBundle easily that way
 
 See: CustomBuildParameters.cs for an example
 */

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class MoveAssetBundles : IBuildTask
    {
        public int Version { get { return 1; } }
        
#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;
#pragma warning restore 649
	    

        public ReturnCode Run()
        {
	        if( m_Parameters is CustomBuildParameters )
	        {
		        CustomBuildParameters custom = m_Parameters as CustomBuildParameters;

		        foreach( KeyValuePair<string,string> pair in custom.m_PerBundleBuildFolder )
		        {
			        string fromP = Application.dataPath.Substring( 0, Application.dataPath.Length - 6 ) + m_Parameters.GetOutputFilePathForIdentifier( pair.Key );
			        string toP =  Application.dataPath.Substring( 0, Application.dataPath.Length - 6 ) + string.Format( "{0}/{1}", pair.Value, pair.Key );
			        
			        string directory = toP.Substring( 0, toP.LastIndexOf( '/' ) );
			        Directory.CreateDirectory( directory );
			        
			        File.Move( fromP, toP );
		        }
	        }
            return ReturnCode.Success;
        }
    }
}
