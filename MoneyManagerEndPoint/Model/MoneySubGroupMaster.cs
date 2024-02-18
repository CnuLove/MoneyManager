using System.ComponentModel.DataAnnotations;

namespace MoneyManagerEndPoint.Model
{
    public class MoneySubGroupMaster
    {
        [Key]
        public int sublegerid {  get; set; }
        public string sublegername { get; set; }

        public int grouplegerid { get; set; }

    }
}
