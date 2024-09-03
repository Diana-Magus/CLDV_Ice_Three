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
    public class HintModel : ITableEntity
    {

        

        [Key]
        public int HintID { get; set; }

        public int TeamID { get; set; } // FK to team (needed)

        public string? HintName { get; set; }

        public string? HintText { get; set; }

        public string? HintAnswer { get; set; }

        public string? HintImageUrl { get; set; }

        public bool? IsFound { get; set; }



        public string? PartitionKey { get; set; }

        public string? RowKey { get; set; }

        public ETag ETag { get; set; }

        public DateTimeOffset? Timestamp { get; set; }


        [Required(ErrorMessage = "Please select a treasure")]
        public int TreasureID { get; set; }
    }
}
