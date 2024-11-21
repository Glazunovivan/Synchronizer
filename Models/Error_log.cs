using System.ComponentModel.DataAnnotations.Schema;

namespace Synchronizer.Models
{
    [Table("Error_log")]
    public class Error_log
    {
        [Column("_id")]
        public int Id { get; set; }

        [Column("script_name")]
        public string ScriptName { get; set; }

        [Column("error_message")]
        public string ErrorMessage { get; set; }

        [Column("error_time")]
        public DateTime ErrorTime { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("job_id")]
        public int? JobId { get; set; }

        [Column("stock_id")]
        public int StockId { get; set; }

        [Column("additional_data")]
        public string AdditionalData { get; set; }
    }
}
