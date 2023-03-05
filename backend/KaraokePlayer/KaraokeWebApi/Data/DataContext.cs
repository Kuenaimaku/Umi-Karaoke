using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using KaraokeWebApi.Models;

namespace KaraokeWebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        { 
        
        }
        public DbSet<Song> Songs { get; set; }
        public DbSet<QueueItem> Queue {  get; set; }
    }
}
