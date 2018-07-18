using System;
using BOA.Common.Helpers;

namespace BOA.CodeGeneration.Common
{
    public static class ServerNames
    {
        #region Public Properties
        public static string BfixAtlas => @"srvbfix\Atlas";

        public static string DevAtlas => @"srvdev\Atlas";

        public static string DevAtlasDE => @"srvdedb";

        public static string DevKKDB => "srvkkdbdev";

        public static string PrepAtlas => @"srvprep\Atlas";

        public static string PrepKKDB => @"SRVXPREP\SRVKKDB";
        #endregion

        #region Public Methods
        public static string GetServerNameOfConnectionString(string value)
        {
            if (value.ToUpperEN().Contains(DevAtlasDE.ToUpperEN()))
            {
                return DevAtlasDE;
            }

            if (value.ToUpperEN().Contains(PrepAtlas.ToUpperEN()))
            {
                return PrepAtlas;
            }

            if (value.ToUpperEN().Contains(DevAtlas.ToUpperEN()))
            {
                return DevAtlas;
            }

            if (value.ToUpperEN().Contains(DevKKDB.ToUpperEN()))
            {
                return DevKKDB;
            }

            if (value.ToUpperEN().Contains(BfixAtlas.ToUpperEN()))
            {
                return BfixAtlas;
            }

            throw new ArgumentException(value);
        }
        #endregion
    }
}