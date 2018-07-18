namespace BOAPlugins.ExportingModel
{
    public class MessagingExporterResult
    {
        #region Public Properties
        public string ErrorMessage   { get; set; }
        public string GeneratedCode  { get; set; }
        public string TargetFilePath { get; set; }
        #endregion
    }
}