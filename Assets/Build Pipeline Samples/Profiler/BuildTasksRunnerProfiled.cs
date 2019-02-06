using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Profiler;


/*
 *
 *
 *
 *    Made a copy,
 *
 *
 *
 *     Edits added to add profiler samples for task times
 *
 *
 *
 *         ContextInjector was internal but needed for a duplicate
 *
 * 
 */

namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Basic static class containing default implementations for BuildTask validation and running.
    /// </summary>
    public static class BuildTasksRunnerProfiled
    {
        /// <summary>
        /// Basic run implementation that takes a set of tasks, a context, and runs returning the build results.
        /// <seealso cref="IBuildTask"/>, <seealso cref="IBuildContext"/>, and <seealso cref="ReturnCode"/>
        /// </summary>
        /// <param name="pipeline">The set of build tasks to run.</param>
        /// <param name="context">The build context to use for this run.</param>
        /// <returns>Return code with status information about success or failure causes.</returns>
        public static ReturnCode Run(IList<IBuildTask> pipeline, IBuildContext context)
        {
            // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            if (pipeline == null)
            {
                BuildLogger.LogException(new ArgumentNullException("pipeline"));
                return ReturnCode.Exception;
            }

            // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            if (context == null)
            {
                BuildLogger.LogException(new ArgumentNullException("context"));
                return ReturnCode.Exception;
            }

            IProgressTracker tracker;
            if (context.TryGetContextObject(out tracker))
                tracker.TaskCount = pipeline.Count;


            IContextObject ctx;
            Profiler profiler = null;
            if( context.TryGetContextObject( typeof( Build.Pipeline.IProfiler), out ctx ) )
                profiler = ctx as Profiler;

            foreach (IBuildTask task in pipeline)
            {
                try
                {
                    if (!tracker.UpdateTaskUnchecked(task.GetType().Name.HumanReadable()))
                        return ReturnCode.Canceled;

                    ContextInjector.Inject(context, task);
                    
                    if( profiler != null )
                        profiler.PushSample( task.GetType().Name );
                    var result = task.Run();
                    if( profiler != null )
                        profiler.PopSample();
                    
                    if (result < ReturnCode.Success)
                        return result;
                    ContextInjector.Extract(context, task);
                }
                catch (Exception e)
                {
                    BuildLogger.LogException(e);
                    return ReturnCode.Exception;
                }
            }

            return ReturnCode.Success;
        }

    }
}
