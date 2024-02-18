using System.ComponentModel.DataAnnotations;

namespace MoneyManagerEndPoint.Model
{
    public class MoneyGroupMaster
    {
        [Key]
        public int grouplegerid { get; set; }
        public string grouplegername { get; set; }
    }
}
