namespace Rhino.DSL.Tests.DslFactoryFixture
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Boo.Lang.Compiler;
    using MbUnit.Framework;

    [TestFixture]
    public class DslFixtureIntegrationFixture
    {
        private readonly string path = @"DslFactoryFixture/Integration.boo";

        [Test]
        public void When_file_is_changed_will_automatically_get_new_version()
        {
            DslFactory factory = new DslFactory();
            factory.Register<DemoDslBase>(new DemoDslEngine());
            File.WriteAllText(path, "print 'test'");
            
            DemoDslBase demo = factory.Create<DemoDslBase>(path);
            demo.Execute();
            Assert.AreEqual("test", demo.Messages[0]);

            File.WriteAllText(path,"print 'changed'");
            Thread.Sleep(200);//let it time to refresh
            demo = factory.Create<DemoDslBase>(path);
            demo.Execute();
            Assert.AreEqual("changed", demo.Messages[0]);
        }
    }

    public abstract class DemoDslBase
    {
        public List<string> Messages = new List<string>();
        public abstract void Execute();

        public void print(string msg)
        {
            Messages.Add(msg);
        }
    }

    public class DemoDslEngine : DslEngine
    {
        protected override void CustomizeCompiler(BooCompiler compiler, CompilerPipeline pipeline, Uri[] urls)
        {
            pipeline.Insert(1, new AnonymousBaseClassCompilerStep(typeof (DemoDslBase), "Execute"));
        }
    }
}