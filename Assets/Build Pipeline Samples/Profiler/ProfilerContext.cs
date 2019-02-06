using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


namespace UnityEditor.Build.Pipeline
{
    public class ProfilerSample
    {
        internal Stopwatch m_Tracker;
        internal string m_Name;
        internal List<ProfilerSample> children;
        internal ProfilerSample m_Parent;

        internal ProfilerSample( string name )
        {
            m_Name = name;
            m_Tracker = new Stopwatch();
            children = new List<ProfilerSample>();
        }

        public void Start()
        {
            m_Tracker.Start();
        }

        public void Stop()
        {
            m_Tracker.Stop();
        }
    }

    public interface IProfiler : IContextObject
    {
        List<ProfilerSample> m_Samples { get; set; }
        void PushSample( string name );

        void PopSample();
    }

    [System.Serializable]
    public class Profiler : IProfiler
    {
        public List<ProfilerSample> m_Samples { get; set; }

        private ProfilerSample m_ActiveSample;

        internal Profiler()
        {
            m_Samples = new List<ProfilerSample>();
        }

        public Profiler( string autoStartSample = null )
        {
            m_Samples = new List<ProfilerSample>();
            if( string.IsNullOrEmpty( autoStartSample ) == false )
                PushSample( autoStartSample );
        }


        public void PushSample( string name )
        {
            if( string.IsNullOrEmpty( name ) )
                throw new ArgumentException( "Sample name cannot be null or empty", "name" );

            ProfilerSample s = new ProfilerSample( name );
            s.Start();
            if( m_ActiveSample != null )
            {
                m_ActiveSample.children.Add( s );
                s.m_Parent = m_ActiveSample;
                m_ActiveSample = s;
            }
            else
            {
                m_ActiveSample = s;
                m_Samples.Add( m_ActiveSample );
            }
        }

        public void PopSample()
        {
            if( m_ActiveSample == null )
            {
                Debug.LogError( "Too many pop commands on profiler samples" );
                return;
            }

            m_ActiveSample.Stop();
            m_ActiveSample = m_ActiveSample.m_Parent;
        }

        ~Profiler()
        {
            while( m_ActiveSample != null )
            {
                PopSample();
            }
        }
    }
}