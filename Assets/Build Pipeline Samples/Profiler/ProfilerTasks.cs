using System.IO;
using System.Text;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace UnityEditor.Build.Pipeline
{

    // This was not of use if sampling tasks, 
    /// <summary>
    /// push and pop system does not work for this, because starting one during a task cannot be popped outside of that task
    /// in this example. because we are profiling around the task will mean the pop for that pops it
    /// </summary>
    
//     public class BeginProfilerSample : IBuildTask
//     {
//         public int Version { get { return 1; } }
//
//         private string m_SampleName;
//         
// #pragma warning disable 649
//         [InjectContext(ContextUsage.In)]
//         IProfiler m_Profiler;
// #pragma warning restore 649
//
//         public BeginProfilerSample( string sampleName )
//         {
//             m_SampleName = sampleName;
//         }
//
//         public ReturnCode Run()
//         {
//             m_Profiler.PushSample( m_SampleName );
//             return ReturnCode.Success;
//         }
//     }
//     
//     public class EndProfilerSample : IBuildTask
//     {
//         public int Version { get { return 1; } }
//
//         private string m_SampleName;
//         
// #pragma warning disable 649
//         [InjectContext(ContextUsage.In)]
//         IProfiler m_Profiler;
// #pragma warning restore 649
//
//         public ReturnCode Run()
//         {
//             m_Profiler.PopSample();
//             return ReturnCode.Success;
//         }
//     }
    
    public class CreateBuiltTimeReport : IBuildTask
    {
        public int Version { get { return 1; } }
        
#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IProfiler m_Profiler;
#pragma warning restore 649

        public ReturnCode Run()
        {
            StringBuilder stringBuilder = new StringBuilder("--- Built time results ---");
            for( int i = 0; i < m_Profiler.m_Samples.Count; ++i )
            {
                GrabLine( m_Profiler.m_Samples[i], "", stringBuilder );
            }
            
            Debug.Log( stringBuilder.ToString() );

            string path = Application.dataPath.Substring( 0, Application.dataPath.Length - 6 ) + "/Logs/AssetBundleBuildTimeLog.txt";
            string directory = Path.GetDirectoryName(path);
            if( string.IsNullOrEmpty( directory ) == false )
                Directory.CreateDirectory(directory);
            
            System.IO.StreamWriter file = new System.IO.StreamWriter( path );
            file.Write(stringBuilder.ToString());
            file.Close();
            
            return ReturnCode.Success;
        }

        void GrabLine( ProfilerSample sample, string prefix, StringBuilder appendTo )
        {
            appendTo.Append( "\n" + prefix + sample.m_Name + " - " + sample.m_Tracker.ElapsedMilliseconds );
            
            for( int i = 0; i < sample.children.Count; ++i )
            {
                GrabLine( sample.children[i], prefix + "  ", appendTo );
            }
        }
    }
}
