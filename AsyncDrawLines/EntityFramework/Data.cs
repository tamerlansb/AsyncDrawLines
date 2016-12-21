using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

namespace AsyncDrawLines
{
    public class Data
    {
        [Key]
        public int ID
        {
            get; set;
        }
        public byte[] blob
        {
            get; set;
        }
        public string keyword
        {
            get; set;
        }
        public DateTime date
        {
            set; get;
        }
    }
}
