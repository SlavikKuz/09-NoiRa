using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerHearingLib
{
    public class HearingModel
    {
        public string status { get; set; }
        public List<Prediction> predictions { get; set; }

        public class Prediction
        {
            public string label_id { get; set; }
            public string label { get; set; }
            public double probability { get; set; }
        }
    }
}
