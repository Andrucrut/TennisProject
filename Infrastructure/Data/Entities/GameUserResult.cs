﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Entities
{
    public class GameUserResult
    {
        public long Id { get; set; }
        public long GameResultId { get; set; }
        public long UserId { get; set; }
    }
}
