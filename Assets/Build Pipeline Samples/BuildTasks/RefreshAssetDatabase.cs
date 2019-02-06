using System.IO;
using System.Linq;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class RefreshAssetDatabase : IBuildTask
    {
        public int Version { get { return 1; } }

        public ReturnCode Run()
        {
            AssetDatabase.Refresh();
            return ReturnCode.Success;
        }
    }
}
