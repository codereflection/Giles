using System;
using NUnit.Core;
using NUnit.Util;

namespace Giles.Runner.NUnit
{
    public class DomainRunner : ProxyTestRunner
    {
        private DomainAgent agent;

        private AppDomain domain;

        public DomainRunner()
                : this( 0 )
        {
        }

        public DomainRunner( int runnerId )
                : base( runnerId )
        {
            DomainManager = new DomainManager();
        }

        private DomainManager DomainManager { get; set; }

        public override bool Load( TestPackage package )
        {
            Unload();

            try
            {
                if ( domain == null )
                {
                    domain = DomainManager.CreateDomain( package );
                }

                if ( agent == null )
                {
                    agent = DomainAgent.CreateInstance( domain );
                    agent.Start();
                }

                if ( TestRunner == null )
                {
                    TestRunner = agent.CreateRunner( ID );
                }

                return TestRunner.Load( package );
            }
            catch
            {
                Unload();
                throw;
            }
        }

        public override void Unload()
        {
            if ( TestRunner != null )
            {
                TestRunner.Unload();
                TestRunner = null;
            }

            if ( agent != null )
            {
                agent.Dispose();
                agent = null;
            }

            if ( domain != null )
            {
                DomainManager.Unload( domain );
                domain = null;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Unload();
        }
    }
}