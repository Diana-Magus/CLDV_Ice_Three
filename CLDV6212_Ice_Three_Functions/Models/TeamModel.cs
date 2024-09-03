using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDV6212_Ice_Three_Functions.Models
{
    public class TeamModel : ITableEntity
    {
        [Key]
        public int TeamID { get; set; }

        public string? TeamName { get; set; }

        public int? TeamScore { get; set; }

        public string? PartitionKey { get; set; }

        public string? RowKey { get; set; }

        public ETag ETag { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

    }
}
