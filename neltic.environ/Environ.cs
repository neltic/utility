namespace neltic.environ
{
    using neltic.utility.builder.csharp;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;

    public static partial class Environ
    {
        public static string SchoolContext { get; set; }
        public static string SchoolContextProvider { get; set; }
        public static string SiraContext { get; set; }
        public static string SiraContextProvider { get; set; }
        public static string OnlineSiraContext { get; set; }
        public static string OnlineSiraContextProvider { get; set; }
        public static string ScriptPath { get; set; }
        public static string LabelRelativePath { get; set; }
        public static string LabelAbsolutePath { get; set; }
        public static string LabelRelativeScriptPath { get; set; }
        public static string TempPath { get; set; }
        public static long ClockValue { get; set; }
        public static string SMTP { get; set; }
        public static string Domain { get; set; }
        public static string EmailDomain { get; set; }
        public static string EmailUri { get; set; }
        public static int TokenExpiresAfter { get; set; }
        public static int ScannerTokenExpiresAfter { get; set; }
        public static int DefaultSystemUser { get; set; }
        public static int ExhibitionID { get; set; }
        public static string ExhibitionValue { get; set; }
        public static bool EncryptPassword { get; set; }
        public static string PhysicalPath { get; set; }
        public static string PartialToken { get; set; }
        public static string[] Claims { get; set; }
        public static string ColorException { get; set; }
        public static int[] ValidCombinationChartsID { get; set; }
        public static int CombinationGroupID { get; set; }
        public static bool IncludeCombinationCharts { get; set; }
        public static int MiniGraphicWidth { get; set; }
        public static int MiniGraphicHeight { get; set; }
        public static string PresentationImageTitle { get; set; }
        public static string AdviceEmailUri { get; set; }
        public static string AuditorCode { get; set; }
        public static string CompanyCode { get; set; }
        public static double OtherValue { get; set; }
        public static double NegativeValue { get; set; }
        public static DateTime Start { get; set; }
        public static DateTime End { get; set; }
        public static string InvalidDate { get; set; }
        public static string NotAndArray { get; set; }
        public static char[] ValidArray { get; set; }
        public static bool[] ValidBooleanArray { get; set; }
        public static char CharValue { get; set; }
        public static char Delimiter { get; set; }
        public static bool ValidBoolean { get; set; }

        private static char[] __DELIMITER_ARRAY__ = { ';', '|', '\n' };

        static  Environ()
        {
            Reload(); 
        }

        public static void Reload()
        {
            ConfigurationManager.RefreshSection("appSettings");
            SchoolContext = (ConfigurationManager.ConnectionStrings["SchoolContext"]).ConnectionString;
            SchoolContextProvider = (ConfigurationManager.ConnectionStrings["SchoolContext"]).ProviderName;
            SiraContext = (ConfigurationManager.ConnectionStrings["SiraContext"]).ConnectionString;
            SiraContextProvider = (ConfigurationManager.ConnectionStrings["SiraContext"]).ProviderName;
            OnlineSiraContext = (ConfigurationManager.ConnectionStrings["OnlineSiraContext"]).ConnectionString;
            OnlineSiraContextProvider = (ConfigurationManager.ConnectionStrings["OnlineSiraContext"]).ProviderName;
            ScriptPath = (ConfigurationManager.AppSettings["ScriptPath"]);
            LabelRelativePath = (ConfigurationManager.AppSettings["LabelRelativePath"]);
            LabelAbsolutePath = (ConfigurationManager.AppSettings["LabelAbsolutePath"]);
            LabelRelativeScriptPath = (ConfigurationManager.AppSettings["LabelRelativeScriptPath"]);
            TempPath = (ConfigurationManager.AppSettings["TempPath"]);
            ClockValue = Convert.ToInt64(ConfigurationManager.AppSettings["ClockValue"]);
            SMTP = (ConfigurationManager.AppSettings["SMTP"]);
            Domain = (ConfigurationManager.AppSettings["Domain"]);
            EmailDomain = (ConfigurationManager.AppSettings["EmailDomain"]);
            EmailUri = (ConfigurationManager.AppSettings["EmailUri"]);
            TokenExpiresAfter = Convert.ToInt32(ConfigurationManager.AppSettings["TokenExpiresAfter"]);
            ScannerTokenExpiresAfter = Convert.ToInt32(ConfigurationManager.AppSettings["ScannerTokenExpiresAfter"]);
            DefaultSystemUser = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultSystemUser"]);
            ExhibitionID = Convert.ToInt32(ConfigurationManager.AppSettings["ExhibitionID"]);
            ExhibitionValue = (ConfigurationManager.AppSettings["ExhibitionValue"]);
            EncryptPassword = (ConfigurationManager.AppSettings["EncryptPassword"]).Equals("true", StringComparison.InvariantCultureIgnoreCase);
            PhysicalPath = (ConfigurationManager.AppSettings["PhysicalPath"]);
            PartialToken = (ConfigurationManager.AppSettings["PartialToken"]);
            Claims = (ConfigurationManager.AppSettings["Claims"]).Split(__DELIMITER_ARRAY__).Select(v => (v)).ToArray();
            ColorException = (ConfigurationManager.AppSettings["ColorException"]);
            ValidCombinationChartsID = (ConfigurationManager.AppSettings["ValidCombinationChartsID"]).Split(__DELIMITER_ARRAY__).Select(v => Convert.ToInt32(v)).ToArray();
            CombinationGroupID = Convert.ToInt32(ConfigurationManager.AppSettings["CombinationGroupID"]);
            IncludeCombinationCharts = (ConfigurationManager.AppSettings["IncludeCombinationCharts"]).Equals("true", StringComparison.InvariantCultureIgnoreCase);
            MiniGraphicWidth = Convert.ToInt32(ConfigurationManager.AppSettings["MiniGraphicWidth"]);
            MiniGraphicHeight = Convert.ToInt32(ConfigurationManager.AppSettings["MiniGraphicHeight"]);
            PresentationImageTitle = (ConfigurationManager.AppSettings["PresentationImageTitle"]);
            AdviceEmailUri = (ConfigurationManager.AppSettings["AdviceEmailUri"]);
            AuditorCode = (ConfigurationManager.AppSettings["AuditorCode"]);
            CompanyCode = (ConfigurationManager.AppSettings["CompanyCode"]);
            OtherValue = Convert.ToDouble(ConfigurationManager.AppSettings["OtherValue"]);
            NegativeValue = Convert.ToDouble(ConfigurationManager.AppSettings["NegativeValue"]);
            Start = DateTime.ParseExact((ConfigurationManager.AppSettings["Start"]),"yyyy-MM-dd",CultureInfo.CurrentCulture);
            End = DateTime.ParseExact((ConfigurationManager.AppSettings["End"]),"yyyy-MM-dd",CultureInfo.CurrentCulture);
            InvalidDate = (ConfigurationManager.AppSettings["InvalidDate"]);
            NotAndArray = (ConfigurationManager.AppSettings["NotAndArray"]);
            ValidArray = (ConfigurationManager.AppSettings["ValidArray"]).Split(__DELIMITER_ARRAY__).Select(v => Convert.ToChar(v)).ToArray();
            ValidBooleanArray = (ConfigurationManager.AppSettings["ValidBooleanArray"]).Split(__DELIMITER_ARRAY__).Select(v => (v).Equals("true", StringComparison.InvariantCultureIgnoreCase)).ToArray();
            CharValue = Convert.ToChar(ConfigurationManager.AppSettings["CharValue"]);
            Delimiter = Convert.ToChar(ConfigurationManager.AppSettings["Delimiter"]);
            ValidBoolean = (ConfigurationManager.AppSettings["ValidBoolean"]).Equals("true", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
