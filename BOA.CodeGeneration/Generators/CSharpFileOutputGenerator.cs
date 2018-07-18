using System;
using System.Globalization;
using System.Linq;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Services;
using BOA.CodeGeneration.Util;
using WhiteStone.IO;

namespace BOA.CodeGeneration.Generators
{
    public class CSharpFileOutputGenerator
    {
        #region Static Fields
        static readonly CultureInfo CultureInfo = new CultureInfo("en-US");
        #endregion

        #region Fields
        readonly FileService _fs = new FileService();
        #endregion

        #region Public Properties
        public WriterContext Context { get; set; }
        #endregion

        #region Public Methods
        public void Generate()
        {
            if (Context == null)
            {
                throw new ArgumentNullException(nameof(Context));
            }

            var config      = Context.Config;
            var table       = Context.Table;
            var namingModel = Context.Naming;

            var typesPath = config.FolderPathForExtractTypesFiles;
            var fs        = _fs;

            var typeContractCanBeExportable = Context.Config.TypeContractIsExportable;
            if (typeContractCanBeExportable)
            {
                var contractGenerator = new ContractGenerator(Context);

                if (fs.Exists(typesPath + namingModel.OutputFileNameContract + ".cs") == false)
                {
                    fs.Write(typesPath + namingModel.OutputFileNameContract + ".cs", contractGenerator.UserFile);
                }

                WriteFileIfContentNotEqual(typesPath + namingModel.OutputFileNameContract + ".designer.cs", contractGenerator.DesignerFile);
            }

            var businessPath = config.FolderPathForExtractBusinessFiles;

            var businessClassGenerator = new BusinessClassGenerator(Context);

            if (!fs.Exists(businessPath + namingModel.OutputFileNameBusiness + ".cs"))
            {
                var userFilePath = businessPath + namingModel.OutputFileNameBusiness + ".cs";

                fs.Write(userFilePath, businessClassGenerator.UserFile);
            }

            var designerFilePath = businessPath + namingModel.OutputFileNameBusiness + ".designer.cs";

            WriteFileIfContentNotEqual(designerFilePath, businessClassGenerator.DesignerFile);

            var orchestrationClassGenerator = new OrchestrationFileGenerator(Context);
            var orchestrationPath           = config.FolderPathForExtractOrchestrationFiles;
            if (config.FolderPathForExtractOrchestrationFiles != null)
            {
                if (!fs.Exists(orchestrationPath + namingModel.OutputFileNameOrchestration + ".cs"))
                {
                    var userFilePath = orchestrationPath + namingModel.OutputFileNameOrchestration + ".cs";

                    fs.Write(userFilePath, orchestrationClassGenerator.UserFile);
                }

                WriteFileIfContentNotEqual(orchestrationPath + namingModel.OutputFileNameOrchestration + ".designer.cs", orchestrationClassGenerator.DesignerFile);
            }

            var storedProcedureFolder = config.FolderPathForExtractStoredProcedureFiles ?? businessPath;

            #region Procedures
            string procedureText;
            string sqlFilePath;

            foreach (var c in config.CustomSelects)
            {
                string sqlProcedureName;

                if (c.IsSelectByValueList)
                {
                    var generator = new SelectByValueArraySql(Context, c.SelectByValueListColumnName);
                    procedureText    = generator.Generate();
                    sqlProcedureName = generator.NameOfSqlProcedure;
                }
                else
                {
                    var generator = new SelectByColumnsSql(Context, c);
                    procedureText    = generator.Generate();
                    sqlProcedureName = generator.SqlProcedureName;
                }

                sqlFilePath = storedProcedureFolder + string.Format(CultureInfo, "{0}.{1}.sql", namingModel.SchemaName, sqlProcedureName);

                WriteFileIfContentNotEqual(sqlFilePath, procedureText);
            }

            foreach (var c in config.CustomUpdates)
            {
                procedureText = new UpdateSqlCustom(Context, c).Generate();
                WriteFileIfContentNotEqual(storedProcedureFolder + "{0}.{1}.sql".FormatCode(namingModel.SchemaName, c.SqlProcedureName), procedureText);
            }

            foreach (var c in config.CustomExists)
            {
                procedureText = new CustomExistSql(Context, c).Generate();
                WriteFileIfContentNotEqual(storedProcedureFolder + "{0}.{1}.sql".FormatCode(namingModel.SchemaName, c.SqlProcedureName), procedureText);
            }

            if (config.CanGenerateInsert)
            {
                procedureText = new InsertSql(Context).Generate();

                WriteFileIfContentNotEqual(storedProcedureFolder + namingModel.NameOfSqlProcedureInsertWithSchema + ".sql", procedureText);
            }

            if (config.CanGenerateInsertStructured)
            {
                procedureText = new InsertStructuredSql(Context).Generate();

                WriteFileIfContentNotEqual(storedProcedureFolder + namingModel.NameOfSqlProcedureInsertWithSchema + InsertStructuredSql.Structured + ".sql", procedureText);
            }

            if (table.PrimaryKeyColumns.Any())
            {
                if (config.CanGenerateUpdate)
                {
                    procedureText = new UpdateSql(Context).Generate();
                    WriteFileIfContentNotEqual(storedProcedureFolder + namingModel.NameOfSqlProcedureUpdateWithSchema + ".sql", procedureText);
                }

                if (config.CanGenerateDelete)
                {
                    procedureText = new DeleteSql(Context).Generate();

                    WriteFileIfContentNotEqual(storedProcedureFolder + namingModel.NameOfSqlProcedureDeleteWithSchema + ".sql", procedureText);
                }

                if (config.CanGenerateSelectByKey)
                {
                    procedureText = new SelectByKeySql(Context).Generate();

                    WriteFileIfContentNotEqual(storedProcedureFolder + namingModel.NameOfSqlProcedureSelectByKeyWithSchema + ".sql", procedureText);
                }

                if (config.CanGenerateSelectByKeyList)
                {
                    procedureText = new SelectByKeyListSql(Context).Generate();

                    WriteFileIfContentNotEqual(storedProcedureFolder + namingModel.NameOfSqlProcedureSelectByKeyListWithSchema + ".sql", procedureText);
                }
            }
            #endregion
        }
        #endregion

        #region Methods
        void WriteFileIfContentNotEqual(string path, string content)
        {
            if (!_fs.Exists(path))
            {
                _fs.Write(path, content);
                return;
            }

            var existingData = _fs.Read(path);

            var isEqual = SpaceCaseInsensitiveComparator.Compare(existingData, content);

            if (!isEqual)
            {
                new TFSAccessForBOA().CheckoutFile(path);
                _fs.Write(path, content);
            }

            if (Context.Config.CanUpdateProcedureOnTargetDatabase)
            {
                if (path.EndsWith(".sql"))
                {
                    DatabaseAccess.Extensions.RunScript(Context.Config.GetDatabase(), content);
                }
            }
        }
        #endregion
    }
}