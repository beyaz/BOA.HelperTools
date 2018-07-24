using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BOA.CodeGeneration.Common;
using BOA.Common.Helpers;
using BOA.DatabaseAccess;
using BOAPlugins.Models;
using BOAPlugins.SearchProcedure;
using BOAPlugins.VSIntegration;
using WhiteStone.Helpers;

namespace BOAPlugins.FormApplicationGenerator
{
    /// <summary>
    ///     Interaction logic for MainForm.xaml
    /// </summary>
    public partial class View
    {
        #region Static Fields
        public static readonly DependencyProperty FormDataFieldsProperty = DependencyProperty.Register("FormDataFields", typeof(ObservableCollection<FieldInfo>), typeof(View), new PropertyMetadata(default(ObservableCollection<FieldInfo>)));
        public static readonly DependencyProperty FormNameProperty       = DependencyProperty.Register("FormName", typeof(string), typeof(View), new PropertyMetadata(default(string), OnFormNameChanged));

        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register(
                                                                                                         "ConnectionString", typeof(string), typeof(View), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ListFormSearchFieldsProperty = DependencyProperty.Register("ListFormSearchFields", typeof(ObservableCollection<FieldInfo>), typeof(View), new PropertyMetadata(default(ObservableCollection<FieldInfo>)));
        #endregion

        #region Fields
        Model _model;
        #endregion

        #region Constructors
        public View()
        {
            Connections = DatabaseConnectionStrings.Connections;

            InitializeComponent();

            KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    Close();
                }
            };

            FormDataFields = new ObservableCollection<FieldInfo>
            {
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.Int32,
                    Name          = "AccountNumber",
                    ComponentName = ComponentName.BAccountComponent
                },
                new FieldInfo
                {
                    TypeName = DotNetTypeName.String,
                    Name     = "CardNumber"
                },
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.DateTime,
                    Name          = "TranDate",
                    ComponentName = ComponentName.BDateTimePicker
                },
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.Decimal,
                    Name          = "Amount",
                    ComponentName = ComponentName.BInputNumeric
                },
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.Int32,
                    Name          = "GenderCode",
                    ComponentName = ComponentName.BParameterComponent
                }
            };

            ListFormSearchFields = new ObservableCollection<FieldInfo>
            {
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.Int32,
                    Name          = "AccountNumber",
                    ComponentName = ComponentName.BAccountComponent
                },
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.String,
                    Name          = "CardNumber",
                    ComponentName = ComponentName.BInput
                },
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.DateTime,
                    Name          = "BeginDate",
                    ComponentName = ComponentName.BDateTimePicker
                },
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.DateTime,
                    Name          = "EndDate",
                    ComponentName = ComponentName.BDateTimePicker
                },
                new FieldInfo
                {
                    TypeName      = DotNetTypeName.Int32,
                    Name          = "BranchId",
                    ComponentName = ComponentName.BBranchComponent
                }
            };

            selectSqlRichTextBox.SetText(@"

-- drop table #tmp

	---------------Örnek Kullanım -----------

SELECT T.* INTO #tmp FROM 
(
		 SELECT vaTranDate         AS TransactionDate,
		 	    vaSourceAmount     AS SourceAmount,
		 	    vaDestAmount       AS DestinationAmount,
		 	    vaSourceCurrency   AS SourceCurrencyCode,
		 	    vaDestCurrency     AS DestinationCurrencyCode,
		 	    vaAuthCode         AS ProvisionNumber,
		 	    vaCATIndicator     AS CATIndicator
		 
		   FROM kkVISAAcq  WITH(NOLOCK)
LEFT OUTER JOIN iccVISAAcq WITH(NOLOCK) ON vaAcqNo = cvaAcqNo 
LEFT OUTER JOIN kkVISATCR4 WITH(NOLOCK) ON vaAcqNo = vtIssAcqNo and vtIssAcq = 'A' 
          WHERE vaAcqNo = 1204 
		    AND vaAccountNo='4051480000011126 ' 
			AND vaFilmLocator = 06003026237

) AS T


	
	SELECT c.name AS ColumnName , ty.name AS DataType 
	  FROM tempdb.sys.columns  c 
INNER JOIN sys.types  ty ON c.user_type_id = ty.user_type_id
     WHERE [object_id] = OBJECT_ID(N'tempdb..#tmp')
	

");
        }
        #endregion

        #region Public Properties
        public IReadOnlyList<DatabaseConnectionInfo> Connections { get; set; }

        public string ConnectionString
        {
            get { return (string) GetValue(ConnectionStringProperty); }
            set { SetValue(ConnectionStringProperty, value); }
        }

        public ObservableCollection<FieldInfo> FormDataFields
        {
            get { return (ObservableCollection<FieldInfo>) GetValue(FormDataFieldsProperty); }
            set { SetValue(FormDataFieldsProperty, value); }
        }

        public string FormName
        {
            get { return (string) GetValue(FormNameProperty); }
            set { SetValue(FormNameProperty, value); }
        }

        public ObservableCollection<FieldInfo> ListFormSearchFields
        {
            get { return (ObservableCollection<FieldInfo>) GetValue(ListFormSearchFieldsProperty); }
            set { SetValue(ListFormSearchFieldsProperty, value); }
        }

        public IVisualStudioLayer VisualStudio { get; set; }
        #endregion

        #region Properties
        string SolutionFilePath
        {
            get { return VisualStudio.GetSolutionFilePath(); }
        }
        #endregion

        #region Methods
        static FieldInfo ConvertFrom(ColumnInfo x)
        {
            var fi       = new FieldInfo();
            var dataType = x.DataType.ToUpperEN();

            var dotNetType = SqlDataType.GetDotNetType(dataType, false);
            if (dotNetType == Names.DotNetStringName)
            {
                fi.TypeName = DotNetTypeName.String;
            }
            else if (dotNetType == Names.DotNetDecimal)
            {
                fi.TypeName = DotNetTypeName.Decimal;
            }
            else if (dotNetType == Names.DotNetInt32 ||
                     dotNetType == Names.DotNetInt16)
            {
                fi.TypeName = DotNetTypeName.Int32;
            }
            else if (dotNetType == Names.DotNetDateTime)
            {
                fi.TypeName = DotNetTypeName.DateTime;
            }
            else if (dotNetType == Names.DotNetBool)
            {
                fi.TypeName = DotNetTypeName.Boolean;
            }

            fi.Name          = x.ColumnName;
            fi.ComponentName = null;

            return fi;
        }

        static void OnFormNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view        = (View) d;
            if (view._model == null)
            {
                return;
            }

            var cachedModel = view.GetCachedModel(view._model);
            if (cachedModel != null)
            {
                view._model = cachedModel;

                view.FormDataFields       = new ObservableCollection<FieldInfo>(cachedModel.FormDataClassFields);
                view.ListFormSearchFields = new ObservableCollection<FieldInfo>(cachedModel.ListFormSearchFields);
            }
        }

        void AutoGenerateFieldsFromSql(object sender, RoutedEventArgs e)
        {
            if (ConnectionString.IsNullOrWhiteSpace())
            {
                MessageBox.Show("ConnectionString seçilmeli");
                return;
            }

            var sqlCommandText = selectSqlRichTextBox.GetText();

            if (sqlCommandText.IsNullOrWhiteSpace())
            {
                MessageBox.Show("Select sql yazılmalı");
                return;
            }

            var sql = new SqlDatabase(ConnectionString)
            {
                CommandText = sqlCommandText
            };


            try
            {
                var columnInfos = sql.ExecuteReader().ToList<ColumnInfo>().ToList();

                FormDataFields = new ObservableCollection<FieldInfo>(columnInfos.ConvertAll(ConvertFrom));

                NamingHelper.InitializeFieldComponentTypes(FormDataFields.ToList());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                return;
            }

            Dispatcher.BeginInvoke((Action) (() => _tabControl.SelectedIndex = 1));
        }

        void GenerateCode(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FormName))
                {
                    MessageBox.Show(Label.FormName + " must have value.");
                    return;
                }

                if (FormName.EndsWith("Form") || FormName.EndsWith("ListForm"))
                {
                    MessageBox.Show("Suffix Form veya ListForm olamaz.");
                    return;
                }

                _model = new Model(SolutionFilePath, FormName)
                {
                    FormDataClassFields  = FormDataFields.ToList(),
                    ListFormSearchFields = ListFormSearchFields.ToList()
                };


                new FileExporter(_model).ExportFiles();

                VisualStudio.UpdateStatusbarText("Files generated successfully. Include files into project.");

                SaveModelToCache(_model);

                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        Model GetCachedModel(Model model)
        {
            
            var cacheFilePath = GetCacheFilePath(model);
            if (File.Exists(cacheFilePath))
            {
                return JsonHelper.Deserialize<Model>(File.ReadAllText(cacheFilePath));
            }

            return null;
        }

        static string GetCacheFilePath(Model model)
        {
            return  model.TypesProjectFolder + model.FormName + ".json";
        }

        void SaveModelToCache(Model model)
        {
            var cacheFilePath = GetCacheFilePath(model);

            Util.WriteFileIfContentNotEqual(cacheFilePath, JsonHelper.Serialize(model));
        }
        #endregion

        class ColumnInfo
        {
            #region Public Properties
            public string ColumnName { get; set; }
            public string DataType   { get; set; }
            #endregion
        }
    }
}