﻿using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace KnotDetectorML.Model
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText;

        [LoadColumn(1)]
        [ColumnName("Label")]
        public bool Sentiment;
    }
}
