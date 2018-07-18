using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.Common.Helpers;
using WhiteStone.Helpers;

namespace BOA.CodeGeneration.Input
{
    public enum Databases
    {
        DbKuveyt2,
        Boa,
        BoaLog,
        BoaReference,
        Divit,
        BoaStats,
        BoaWeb,
        DWH,
        DW,
        BoaCatalog,
        VeribranchProd,
        SMSinteractive,
        SMSBulk,
        BanksoftCC,
        BDSKuveyt,
        WEBFile,
        BanksoftATM,
        BoaERP,
        Budget,
        IMKB,
        CallCenterAgentStatus,
        CallCenterCallLog,
        CallCenterI3_IC,
        CallCenterOutbound,
        CallCenterDailer,
        Fraud,
        BoaCRM,
        AnaliticData,
        SSIS,
        BoaStatsAtlas,
        KTSiteCMS,
        DBArchiveI3_IC,
        Forex,
        BOADWH,
        KTAccessories,
        Deniz,
        Sedef,
        BOAINC,
        BOACloud,
        CallCenterI3_IC_30,
        CallAnalysis,
        BOAReadOnly,
        Mi4bizDB,
        KTTemp,
        AnalyticDB,
        OldBudget,
        PPSReport,
        BOAQueue,
        KARTAL,
        BoaCatalogDev,
        Oracle,
        DBArchive_DBKuveyt2,
        Abacus,
        ContinuumDB,
        SocialMedia,
        APlus,
        BoaERP_BI,
        BoaArchiveCold,
        ActivityLog
    }
}