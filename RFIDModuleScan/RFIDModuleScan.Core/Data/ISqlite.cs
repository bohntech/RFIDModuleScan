﻿//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace RFIDModuleScan.Core
{
    public interface ISqlite
    {
        SQLiteConnection GetConnection();
    }
}
