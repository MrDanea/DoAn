﻿using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace System
{
    public partial class Document
    {
        public string StationName { get => GetString(nameof(StationName)); set => Push(nameof(StationName), value); }
        public string StationID { get => GetString(nameof(StationID)); set => Push(nameof(StationID), value); }
        public string StationAddress { get => GetString(nameof(StationAddress)); set => Push(nameof(StationAddress), value); }
        public List<string> StationTypeList { get => GetArray<List<string>>(nameof(StationTypeList)); set => Push(nameof(StationTypeList), value); }
        public string StationType { get => GetString(nameof(StationType)); set => Push(nameof(StationType), value); }
        public DocumentList StationData { get => GetArray<DocumentList>(nameof(StationData)); set => Push(nameof(StationData), value); }

    }
    public partial class DB
    {
        static public BsonData.Collection? Station => Main.GetCollection(nameof(Station));

    }
}
