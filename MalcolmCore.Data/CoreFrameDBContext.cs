using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Data
{
    public class CoreFrameDBContext: DbContext
    {
        public CoreFrameDBContext(DbContextOptions<CoreFrameDBContext> options) : base(options) 
        {

        }

        public DbSet<useinfo> useinfo { get; set; }
    }
}
