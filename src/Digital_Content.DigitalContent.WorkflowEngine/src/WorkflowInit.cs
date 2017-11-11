using System;
using System.Xml.Linq;
using OptimaJet.Workflow.Core.Builder;
using OptimaJet.Workflow.Core.Bus;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.DbPersistence;
using System.Configuration;

namespace Digital_Content.DigitalContent.WorkflowEngine.src
{
    public static class WorkflowInit
    {
        private static readonly Lazy<WorkflowRuntime> LazyRuntime = new Lazy<WorkflowRuntime>(InitWorkflowRuntime);



        public static WorkflowRuntime Runtime
        {
            get { return LazyRuntime.Value; }
        }


        public static string ConnectionString { get; set; }



        private static WorkflowRuntime InitWorkflowRuntime()
        {
            try
            {

                var connectionString = ConnectionString;
                var dbProvider = new MSSQLProvider(connectionString);

                var builder = new WorkflowBuilder<XElement>(
                    dbProvider,
                    new OptimaJet.Workflow.Core.Parser.XmlWorkflowParser(),
                    dbProvider
                ).WithDefaultCache();

                var runtime = new WorkflowRuntime()
                    .WithBuilder(builder)
                    .WithPersistenceProvider(dbProvider)
                    .WithBus(new NullBus())
                    //TODO If you have planned use Timers uncomment following line of code
                    //.WithTimerManager(new TimerManager())
                    .EnableCodeActions()
                    .SwitchAutoUpdateSchemeBeforeGetAvailableCommandsOn();
                //events subscription
                runtime.ProcessActivityChanged += (sender, args) => { };
                runtime.ProcessStatusChanged += (sender, args) => { };
                //TODO If you have planned to use Code Actions functionality that required references to external assemblies you have to register them here
                //runtime.RegisterAssemblyForCodeActions(Assembly.GetAssembly(typeof(SomeTypeFromMyAssembly)));

                //starts the WorkflowRuntime
                //TODO If you have planned use Timers the best way to start WorkflowRuntime is somwhere outside of this function in Global.asax for example
                runtime.Start();

                return runtime;

            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
