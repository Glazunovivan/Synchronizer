using System.ComponentModel.DataAnnotations.Schema;

namespace Synchronizer.Models
{
    [Table("stock_creds")]
    public class StockCreds
    {

        [Column("_id")]
        public int Id { get; set; }

        /// <summary>
        /// Секунды
        /// </summary>
        [Column("sinc_switch")]
        public int SincSwitch { get;set; }

        [Column("update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Если 2, то синк с таблицей, Если 3 - то через локальный файл , Если 1 - то заглушка
        /// </summary>
        [Column("doc_types")]
        public int DocType { get; set; }

        /// <summary>
        /// ссылка на гугл таблицу
        /// </summary>
        [Column("stock_link")]
        public string StockLink { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        public UserStock UserStock { get; set; }

        [Column("exel_column")]
        public string ExelColumn { get; set; }
        
    }
}
