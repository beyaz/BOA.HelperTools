using System;
using System.Data;
using System.Globalization;
using System.Text;
using BOA.CodeGeneration.Common;
using BOA.DatabaseAccess;
using WhiteStone.Services;

namespace BOAPlugins.SearchProcedure
{
    public class Handler
    {
        #region Fields
        readonly Input _input;
        #endregion

        #region Constructors
        public Handler(Input input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            _input = input;
            Result = new Result();
        }
        #endregion

        #region Public Properties
        public Result Result { get; }
        #endregion

        #region Properties
        static SpaceCaseInsensitiveComparator Comparer
        {
            get
            {
                // ignore creation times
                return new SpaceCaseInsensitiveComparator(CultureInfo.CurrentCulture).IgnoreLines(line => line.StartsWith("CertificateInformation | K", StringComparison.Ordinal));
            }
        }
        #endregion

        #region Public Methods
        public void Handle()
        {
            var selectedText = _input.ProcedureName;

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                CreateNewSqlFile(Environment.NewLine, "Temp Sql File");
                return;
            }

            selectedText = ProcedureTextUtility.ClearProcedureText(selectedText);

            string query = null;

            var dbItemNameParser = DbItemNameInfoParser.Parse(selectedText);

            var procedureName = dbItemNameParser.Name;
            var schemaName = dbItemNameParser.SchemaName;
            var databaseName = dbItemNameParser.DatabaseName;

            var foundCount = 0;
            foreach (var info in DatabaseConnectionStrings.Connections)
            {
                if (databaseName != null && info.DatabaseName != databaseName)
                {
                    continue;
                }

                Result.ProcedureContainerDatabaseConnectionInfo = info;

                var dev = new SqlDatabase(info.ConnectionStringDev);
                var prep = new SqlDatabase(info.ConnectionStringPrep);

                using (dev)
                {
                    using (prep)
                    {
                        string devVersion = null;
                        string prepVersion = null;
                        var found = false;

                        var fileName = info.DatabaseName + "." + selectedText;

                        query = "SELECT sm.definition,SCHEMA_NAME(O.schema_id) AS SchemaName " +
                                "  from sys.sql_modules sm WITH(NOLOCK) INNER JOIN " +
                                "       sys.objects o WITH(NOLOCK) ON sm.object_id = o.object_id " +
                                " where o.name = " + " '" + procedureName + "' AND SCHEMA_NAME(O.schema_id)= '" + schemaName + "'";

                        dev.CommandText = query;
                        var dr = dev.ExecuteReader();
                        while (dr.Read())
                        {
                            devVersion = dr.GetString(0);
                            found = true;
                            foundCount++;
                            break;
                        }
                        dr.Close();

                        prep.CommandText = query;
                        dr = prep.ExecuteReader();
                        while (dr.Read())
                        {
                            prepVersion = dr.GetString(0);
                            found = true;
                            foundCount++;
                            break;
                        }

                        dr.Close();

                        // maybe table information was requested.
                        if (!found)
                        {
                            if (prep.IsTable(procedureName, schemaName))
                            {
                                prepVersion = sp_Help(prep, procedureName, schemaName);
                                found = true;
                                foundCount++;
                            }

                            if (dev.IsTable(procedureName, schemaName))
                            {
                                devVersion = sp_Help(dev, procedureName, schemaName);
                                found = true;
                                foundCount++;
                            }
                        }

                        if (found)
                        {
                            var isEqual = Comparer.Compare(devVersion, prepVersion);
                            if (isEqual)
                            {
                                CreateNewSqlFile(devVersion, fileName);
                            }
                            else
                            {
                                if (devVersion != null)
                                {
                                    CreateNewSqlFile(devVersion, "DEV - " + fileName);
                                }
                                if (prepVersion != null)
                                {
                                    CreateNewSqlFile(prepVersion, "PREP - " + fileName);
                                }
                            }

                            // do not search in other databases.
                            return;
                        }
                    }
                }
            }

            if (foundCount == 0)
            {
                Result.ErrorMessage = "Sp bulunamadı.";
            }
        }
        #endregion

        #region Methods
        static DataSet ExecuteDataSet(IDatabase db)
        {
            var sqlDataReader = db.ExecuteReader();
            var dataSet = new DataSet();
            while (!sqlDataReader.IsClosed)
            {
                var dataTable = new DataTable();
                dataTable.Load(sqlDataReader);
                dataSet.Tables.Add(dataTable);
            }
            sqlDataReader.Close();
            sqlDataReader.Dispose();
            return dataSet;
        }

        static string sp_Help(IDatabase db, string tableName, string schemaName)
        {
            db.CommandText = "sp_Help";
            db["objname"] = schemaName + "." + tableName;
            db.CommandIsStoredProcedure = true;
            var ds = ExecuteDataSet(db);

            var beautifier = new ServiceManager().GetService<IDataTableStringifier>();

            var sb = new StringBuilder();
            foreach (DataTable dt in ds.Tables)
            {
                if (dt.TableName == "Table1")
                {
                    dt.Columns.Remove("Created_datetime");
                }
                if (dt.TableName == "Table6")
                {
                    dt.Columns.Remove("index_name");
                }
                if (dt.TableName == "Table7")
                {
                    dt.Columns.Remove("constraint_name");
                }

                sb.AppendLine();
                sb.AppendLine(beautifier.StringifyDataTable(dt));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        void CreateNewSqlFile(string text, string name)
        {
            Result.AddSqlFile(new SqlFileInfo {Content = text, FileName = name});
        }
        #endregion
    }
}