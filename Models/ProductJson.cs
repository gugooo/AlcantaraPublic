﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alcantara.Models
{
    public class ProductJson
    {
        public string id { get; set; }
        public int count { get; set; }
        public IEnumerable<string> atrs { get; set; }
    }
}
