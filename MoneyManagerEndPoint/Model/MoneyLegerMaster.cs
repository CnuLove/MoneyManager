using System.ComponentModel.DataAnnotations;

namespace MoneyManagerEndPoint.Model
{
    public class MoneyLegerMaster
    {
        [Key]
        public int legertypeid {  get; set; }

        public string legertypename { get; set; }
    }
}
