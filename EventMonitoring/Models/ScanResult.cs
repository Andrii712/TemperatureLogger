using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EventMonitoring.Models
{
    public class ScanResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordID { get; set; }
        [Required]
        public DateTime EventTime { get; set; }
        [Required]
        public int AccountID { get; set; }
        [MaxLength(150)]
        public string FullName { get; set; }
        [MaxLength(50)]
        public string Position { get; set; }
        [Required]
        public byte DirectionID { get; set; }
        [MaxLength(10)]
        public string Direction { get; set; }
        [Required]
        public int DoorModuleID { get; set; }
        [MaxLength(25)]
        public string DoorModuleName { get; set; }
        [Required]
        public int ZoneID { get; set; }
        [MaxLength(50)]
        public string ZoneName { get; set; }
        [Required]
        public int RelayOut { get; set; }
        [Required]
        public double LastScanTemp { get; set; }

        public override string ToString()
        {
            return $"{EventTime} {FullName} {Position} {Direction}" +
                $" {DoorModuleName} {ZoneName} {LastScanTemp}";
        }
    }
}
