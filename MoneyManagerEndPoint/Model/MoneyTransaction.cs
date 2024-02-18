using System.ComponentModel.DataAnnotations;

namespace MoneyManagerEndPoint.Model
{
    public class MoneyTransaction
    {
        [Key] 
        public int transid {  get; set; }
        public int sublegerid { get;set; }
        public int legertypeid { get; set; }

        private float _amount;
        public float amount
        {
            get
            {
                return _amount;
            }
            set => _amount = legertypeid == 2 ? -(value) : (value);
        }

        public int active { get; set; } = 1;

        public float balance { get; set; }

        public  DateTime createddate { get; set; }
    }
}
