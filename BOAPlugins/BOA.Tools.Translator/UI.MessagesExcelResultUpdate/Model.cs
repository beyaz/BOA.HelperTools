using System;
using System.Collections.Generic;
using WhiteStone.Common;

namespace BOA.Tools.Translator.UI.MessagesExcelResultUpdate
{
    [Serializable]
    public class Model : ContractBase
    {
        #region Public Properties
        public string FileName { get; set; }

        public List<Item> list { get; set; }
        #endregion

        #region int? GroupId
        int? _groupId;

        public int? GroupId
        {
            get { return _groupId; }
            set
            {
                if (_groupId != value)
                {
                    _groupId = value;
                    OnPropertyChanged("GroupId");
                }
            }
        }
        #endregion
    }

    [Serializable]
    public class Item : ContractBase
    {
        #region int ExcelRowIndex
        int _excelRowIndex;

        public int ExcelRowIndex
        {
            get { return _excelRowIndex; }
            set
            {
                if (_excelRowIndex != value)
                {
                    _excelRowIndex = value;
                    OnPropertyChanged("ExcelRowIndex");
                }
            }
        }
        #endregion

        #region string NameTR
        string _nameTR;

        public string NameTR
        {
            get { return _nameTR; }
            set
            {
                if (_nameTR != value)
                {
                    _nameTR = value;
                    OnPropertyChanged("NameTR");
                }
            }
        }
        #endregion

        #region string NameEN
        string _nameEN;

        public string NameEN
        {
            get { return _nameEN; }
            set
            {
                if (_nameEN != value)
                {
                    _nameEN = value;
                    OnPropertyChanged("NameEN");
                }
            }
        }
        #endregion
    }
}