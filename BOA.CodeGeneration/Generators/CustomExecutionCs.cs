using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Services;
using BOA.CodeGeneration.SQLParser;
using BOA.CodeGeneration.Util;
using BOA.Common.Helpers;

namespace BOA.CodeGeneration.Generators
{
    public class CustomExecutionCs : WriterBase
    {
        #region Fields
        readonly BOAProcedureCommentParser BOAProcedureCommentParser;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomExecutionCs" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public CustomExecutionCs(WriterContext context, CustomExecution model)
            : base(context)
        {
            Model                     = model;
            BOAProcedureCommentParser = new BOAProcedureCommentParser();
            UpdateModelEmptyValues(model);
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets a value indicating whether [parameter is contract].
        /// </summary>
        public bool ParameterIsContract
        {
            get { return ProcedureParameters.Count(p => !Names.GenericColumns.Contains(p.Name)) > 3; }
        }

        /// <summary>
        ///     Gets the procedure information return columns.
        /// </summary>
        public IReadOnlyList<DataReaderColumn> ProcedureInfoReturnColumns => Model.ProcedureInfo.TryToGetReturnColumnNames(Model.Database);

        /// <summary>
        ///     Gets the procedure parameters.
        /// </summary>
        public IList<IProcedureParameter> ProcedureParameters => Model.ProcedureInfo.Parameters;
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the type of the generic response method return.
        /// </summary>
        /// <value>
        ///     The type of the generic response method return.
        /// </value>
        string GenericResponseMethodReturnType
        {
            get
            {
                if (Model.ExecutionType == ExecutionType.ExecuteReaderForOneColumn)
                {
                    var returnColumn = ProcedureInfoReturnColumns.First();
                    return "IList<{0}>".FormatCode(SqlDataType.GetDotNetType(returnColumn.DataTypeName, returnColumn.AllowDBNull));
                }

                if (Model.ExecutionType == ExecutionType.ExecuteNonQuery)
                {
                    return "int";
                }

                if (Model.ReturnValueType == null)
                {
                    return "int";
                }

                if (Model.GenericResponseMethodReturnType == "decimal?")
                {
                    return Model.GenericResponseMethodReturnType;
                }

                if (Model.GenericResponseMethodReturnType == "bool")
                {
                    return Model.GenericResponseMethodReturnType;
                }

                if (Model.ExecutionType == ExecutionType.ExecuteScalar)
                {
                    return "int";
                }

                if (!Model.ReturnOnlyOneRecord && Model.ExecutionType == ExecutionType.ExecuteReader)
                {
                    return "List<" + Model.GenericResponseMethodReturnType + ">";
                }

                return Model.GenericResponseMethodReturnType;
            }
        }

        /// <summary>
        ///     Gets the model.
        /// </summary>
        CustomExecution Model { get; }

        /// <summary>
        ///     Gets the procedure parameters for c sharp method.
        /// </summary>
        IList<IProcedureParameter> ProcedureParametersForCSharpMethod
        {
            get { return ProcedureParameters.Where(p => !Names.GenericColumns.Contains(p.Name)).ToList(); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Generates this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.MissingMemberException">PropertyNotFound: " + ci.ColumnName</exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public string Generate()
        {
            Padding = PaddingForMethodDeclaration;
            if (Model.Comment != null)
            {
                WriteLine("/// <summary>");
                WriteLine("///" + PaddingForComment + Model.Comment);
                WriteLine("/// </summary>");
            }

            WriteLine("public GenericResponse<{1}> {0}({2})", Model.DotNetMethodName, GenericResponseMethodReturnType, GetMethodParametersPart());

            WriteLine("{");

            Padding++;

            if (Model.ExecutionType == ExecutionType.ExecuteReader)
            {
                WriteLine("var returnObject = CreateResponse<{0}>();", GenericResponseMethodReturnType);
                WriteLine();
            }

            if (Model.ExecutionType == ExecutionType.ExecuteReaderForOneColumn)
            {
                WriteLine("var returnObject = CreateResponse<{0}>();", GenericResponseMethodReturnType);
                WriteLine();
            }

            WriteLine("var command = DBLayer.GetDBCommand(Databases.{0}, \"{1}\");", Model.DatabaseEnumName, Model.ProcedureFullName);
            WriteLine();

            if (ProcedureParameters.Any())
            {
                WriteLine("// Parameters");

                foreach (var procedureParameter in ProcedureParameters)
                {
                    var sqlDatabaseTypeName = SqlDataType.GetSqlDbType(procedureParameter.SqlDataType);

                    var parameterValue = procedureParameter.Name.AsMethodParameter();
                    if (ParameterIsContract)
                    {
                        parameterValue = "contract." + procedureParameter.Name;

                        if (procedureParameter.SqlDataTypeIsVarChar() && Model.ReturnValueType.PropertyTypeIsGuid(procedureParameter.Name))
                        {
                            parameterValue = $"contract.{procedureParameter.Name} == Guid.Empty ? null : contract.{procedureParameter.Name}.ToString()";
                        }
                    }

                    if (procedureParameter.Name == Names.UpdateUserName)
                    {
                        parameterValue = Names.UpdateUserNameValue;
                    }

                    if (procedureParameter.Name == Names.UpdateHostName)
                    {
                        parameterValue = Names.UpdateHostNameValue;
                    }

                    if (procedureParameter.Name == Names.UpdateHostIP)
                    {
                        parameterValue = Names.UpdateHostIPValue;
                    }

                    if (procedureParameter.Name == Names.UpdateSystemDate)
                    {
                        parameterValue = Names.UpdateSystemDateValue;
                    }

                    if (procedureParameter.Name == Names.UserName)
                    {
                        parameterValue = Names.UserNameValue;
                    }

                    if (procedureParameter.Name == Names.HostName)
                    {
                        parameterValue = Names.HostNameValue;
                    }

                    if (procedureParameter.Name == Names.HostIP)
                    {
                        parameterValue = Names.HostIPValue;
                    }

                    if (procedureParameter.Name == Names.SystemDate)
                    {
                        parameterValue = Names.SystemDateValue;
                    }

                    if (procedureParameter.Name == Names.ChannelId)
                    {
                        parameterValue = Names.ChannelIdValue;
                    }

                    WriteLine("DBLayer.AddInParameter(command, \"{0}\", SqlDbType.{1}, {2});",
                              procedureParameter.Name, sqlDatabaseTypeName, parameterValue);
                }

                WriteLine();
            }

            if (Model.ExecutionType == ExecutionType.ExecuteScalar)
            {
                WriteLine("return DBLayer.ExecuteScalar<{0}>(command);", GenericResponseMethodReturnType);
            }
            else if (Model.ExecutionType == ExecutionType.ExecuteNonQuery)
            {
                WriteLine("return DBLayer.ExecuteNonQuery(command);");

                //WriteLine("#region returnObject.Value = DBLayer.ExecuteNonQuery(command);");
                //WriteLine("var sp = DBLayer.ExecuteNonQuery(command);");
                //WriteLine("if (!sp.Success)");
                //WriteLine("{");
                //Padding++;
                //WriteLine("return returnObject.Add(sp);");
                //Padding--;
                //WriteLine("}");
                //WriteLine();
                //WriteLine("returnObject.Value = sp.Value;");
                //WriteLine("#endregion");
                //WriteLine();
                //WriteLine("return returnObject;");
            }
            else if (Model.ExecutionType == ExecutionType.ExecuteReader)
            {
                WriteLine("var sp = DBLayer.ExecuteReader(command);");
                WriteLine("if (!sp.Success)");
                WriteLine("{");
                Padding++;
                WriteLine("return returnObject.Add(sp);");
                Padding--;
                WriteLine("}");
                WriteLine();
                WriteLine("var reader = sp.Value;");
                WriteLine();

                if (Model.ReturnOnlyOneRecord)
                {
                    WriteLine("{0} dataContract = null;", GenericResponseMethodReturnType);
                }
                else
                {
                    WriteLine("var listOfDataContract = new {0}();", GenericResponseMethodReturnType);
                }

                
                
                WriteLine("while (reader.Read())");
                WriteLine("{");
                Padding++;

                if (!Model.ReturnOnlyOneRecord)
                {
                    Write("var ");
                }

                WriteLine("dataContract = new {0}();", Model.GenericResponseMethodReturnType);

                var columns = ProcedureInfoReturnColumns;

                foreach (var ci in columns)
                {
                    var readerMethod = SqlReaderMethods.GetStringValue.ToString();

                    var prop = Model.ReturnValueType.Properties.FirstOrDefault(p => p.Name == ci.ColumnName);
                    if (prop != null)
                    {
                        readerMethod = SqlDataType.GetSqlReaderMethod(prop.PropertyType).ToString();
                    }

                    if (readerMethod == SqlReaderMethods.GetGUIDValue.ToString() && Model.ReturnValueType.PropertyTypeIsGuid(ci.ColumnName))
                    {
                        WriteLine("dataContract." + ci.ColumnName + " = string.IsNullOrWhiteSpace(reader[" + '"' + ci.ColumnName + '"' + "] +" + '"' + '"' + ") ? Guid.Empty : Guid.Parse(reader[" + '"' + ci.ColumnName + '"' + "].ToString());");
                        continue;
                    }

                    WriteLine(@"dataContract.{0} = SQLDBHelper.{1}(reader[{2}]);", ci.ColumnName, readerMethod, '"' + ci.ColumnName + '"');
                }

                if (Model.ReturnOnlyOneRecord)
                {
                    WriteLine("break;");
                }
                else
                {
                    WriteLine("listOfDataContract.Add(dataContract);");
                }

                Padding--;
                WriteLine("}");

                WriteLine("reader.Close();");
                WriteLine();

                if (Model.ReturnOnlyOneRecord)
                {
                    WriteLine("returnObject.Value = dataContract;");
                }
                else
                {
                    WriteLine("returnObject.Value = listOfDataContract;");
                }

                WriteLine();

                WriteLine("return returnObject;");
            }

            else if (Model.ExecutionType == ExecutionType.ExecuteReaderForOneColumn)
            {
                WriteLine("var sp = DBLayer.ExecuteReader(command);");
                WriteLine("if (!sp.Success)");
                WriteLine("{");
                Padding++;
                WriteLine("return returnObject.Add(sp);");
                Padding--;
                WriteLine("}");
                WriteLine();

                WriteLine("var list = new {0}();", GenericResponseMethodReturnType.RemoveFromStart("I"));
                WriteLine();
                WriteLine("var reader = sp.Value;");
                WriteLine("while (reader.Read())");
                WriteLine("{");
                Padding++;

                var ci = ProcedureInfoReturnColumns.First();

                var readerMethod = SqlDataType.GetSqlReaderMethod(ci.DataType).ToString();

                WriteLine(@"list.Add(SQLDBHelper.{0}(reader[{1}]));", readerMethod, '"' + ci.ColumnName + '"');

                Padding--;
                WriteLine("}");

                WriteLine("reader.Close();");
                WriteLine();

                WriteLine("returnObject.Value = list;");
                WriteLine();

                WriteLine("return returnObject;");
            }
            else
            {
                throw new NotImplementedException(Model.ExecutionType.ToString());
            }

            Padding--;
            WriteLine("}");

            return GeneratedString;
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Gets the method parameters part.
        /// </summary>
        /// <returns></returns>
        string GetMethodParametersPart()
        {
            var sb = new StringBuilder();

            if (ParameterIsContract)
            {
                sb.AppendFormat("{0} contract", Model.GenericResponseMethodReturnType);
            }
            else if (ProcedureParametersForCSharpMethod.Any())
            {
                var last = ProcedureParametersForCSharpMethod.Last().Name;
                foreach (var procedureParameter in ProcedureParametersForCSharpMethod)
                {
                    var dotNetType = procedureParameter.GetDotNetType();

                    sb.AppendFormat("{0} {1}", dotNetType, procedureParameter.Name.AsMethodParameter());

                    if (procedureParameter.Name != last)
                    {
                        sb.Append(", ");
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Gets the procedure definition script.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">ProcedureNotFound: " + schemaName + "." + procedureName</exception>
        string GetProcedureDefinitionScript()
        {
            var dal = new DataAccess(Model.Database);

            var schemaName    = Model.ProcedureFullName.Split('.')[0];
            var procedureName = Model.ProcedureFullName.Split('.')[1];

            var definition = dal.GetProcedureDefinition(schemaName, procedureName);

            if (definition.IsNullOrEmpty())
            {
                throw new NotImplementedException("ProcedureNotFound: " + schemaName + "." + procedureName);
            }

            return definition;
        }

        /// <summary>
        ///     Updates the model empty values.
        /// </summary>
        /// <param name="model">The model.</param>
        void UpdateModelEmptyValues(CustomExecution model)
        {
            if (model.DatabaseEnumName == null)
            {
                model.DatabaseEnumName = Context.Naming.DatabaseEnumName;
            }

            if (model.ProcedureFullName == null)
            {
                model.ProcedureFullName = Context.Config.DatabaseTargetSchemaForProcedureNames + "." + model.SqlProcedureName;
            }

            if (model.Database == null)
            {
                model.Database = new DataAccess(Context.Config.GetDatabase()).Database;
            }

            if (model.ProcedureDefinitionScript == null)
            {
                model.ProcedureDefinitionScript = GetProcedureDefinitionScript();
            }

            if (model.ProcedureInfo == null)
            {
                model.ProcedureInfo = Parser.ParseProcedure(model.ProcedureDefinitionScript);
            }

            if (model.Comment == null)
            {
                model.Comment = BOAProcedureCommentParser.GetCommentForDotNet(Model.ProcedureInfo.Comment);
            }
        }
        #endregion
    }

    static class Extensions
    {
        #region Methods
        internal static bool PropertyTypeIsGuid(this ITypeDefinition typeDefinition, string propertyName)
        {
            return typeDefinition.Properties.FirstOrDefault(p => p.Name == propertyName)?.PropertyType == typeof(Guid);
        }

        internal static bool SqlDataTypeIsVarChar(this IProcedureParameter procedureParameter)
        {
            return procedureParameter.SqlDataType == SqlDataType.VARCHAR;
        }
        #endregion
    }
}