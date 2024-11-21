using System.ComponentModel.DataAnnotations.Schema;

namespace Synchronizer.Models
{
    [Table("user_stock")]
    public class UserStock
    {
        [Column("_id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        /// <summary>
        /// Поле которое синхронизируем с гугл таблицей
        /// </summary>
        [Column("part_number")]
        public string PartNumber { get; set; }

        [Column("update_time")]
        public DateTime UpdateTime { get; set; }
    
    }
}
