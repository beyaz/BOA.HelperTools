using System;
using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Generators;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Services;

namespace BOA.CodeGeneration.Input
{
    public class ArgumentRunnerInput
    {
        #region Constructors
        public ArgumentRunnerInput()
        {
        }

        public ArgumentRunnerInput(params TableConfig[] tableConfigs)
        {
            TableConfigs = tableConfigs.ToList();
        }
        #endregion

        #region Public Properties
        public IReadOnlyList<TableConfig> TableConfigs { get; set; }
        #endregion

        #region Public Methods
        #endregion
    }

    public class ArgumentRunner
    {
        #region Public Methods
        public void Run(ArgumentRunnerInput input)
        {
            foreach (var tableConfig in input.TableConfigs)
            {
                Log("Generating for : " + tableConfig.DatabaseTableFullPath);

                GenerateTable(tableConfig);
            }
        }
        #endregion

        #region Methods
        static void GenerateTable(TableConfig config)
        {
            var context = new WriterContext
            {
                Config = config
            };

            new NamingConvention
            {
                Context = context
            }.InitializeNames();

            new CSharpFileOutputGenerator
            {
                Context = context
            }.Generate();
        }

        static void Log(string logMessage)
        {
            Console.WriteLine(logMessage);
        }
        #endregion
    }
}