using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyManagerEndPoint.infra;
using MoneyManagerEndPoint.Model;
using System.Data;

namespace MoneyManagerEndPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {

        private readonly AppDbContext _appDbContext;
        public MasterController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [Route("Getgroupmaster")]
        [HttpGet]
        public ActionResult getgroupmaster()
        {
            try
            {
                var groupmaster = _appDbContext.Tblgrouplegermaster.ToList();
                return Ok(groupmaster);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Getsubgroupmaster")]
        [HttpGet]
        public ActionResult getsubgroupmaster(int id)
        {

            var subgroupmaster = id == 0 ? _appDbContext.TblSubgrouplegermaster : _appDbContext.TblSubgrouplegermaster.Where(data => data.grouplegerid == id);

            return Ok(subgroupmaster);
        }

        [Route("GetListofmaster")]
        [HttpGet]
        public ActionResult getlistofmaster(int id)
        {

            var subgroupmaster = id == 0 ? (from s in _appDbContext.TblSubgrouplegermaster
                                            join g in _appDbContext.Tblgrouplegermaster on s.grouplegerid equals g.grouplegerid
                                            select new
                                            {
                                                sublegerid = s.sublegerid,
                                                grouplegerid = s.grouplegerid,
                                                subgroupname = s.sublegername,
                                                grouplegername = g.grouplegername
                                            })
                : (from s in _appDbContext.TblSubgrouplegermaster
                   join g in _appDbContext.Tblgrouplegermaster on s.grouplegerid equals g.grouplegerid
                   where g.grouplegerid == id
                   select new
                   {
                       sublegerid = s.sublegerid,
                       grouplegerid = s.grouplegerid,
                       subgroupname = s.sublegername,
                       grouplegername = g.grouplegername
                   });

            return Ok(subgroupmaster);
        }


        [Route("Getlegermaster")]
        [HttpGet]
        public ActionResult getlegermaster()
        {
            var legertypemaster = _appDbContext.Tbllegertypemaster.ToList();

            return Ok(legertypemaster);
        }

        [Route("Postgroupmaster")]
        [HttpPost]
        public ActionResult postgroupmaster(MoneyGroupMaster mastermaster)
        {
            var success = _appDbContext.Tblgrouplegermaster.Add(mastermaster);
            _appDbContext.SaveChanges();

            return Ok();
        }

        [Route("Postsubgroup")]
        [HttpPost]
        public ActionResult postsubgroup(MoneySubGroupMaster mastersubmaster)
        {
            var success = _appDbContext.TblSubgrouplegermaster.Add(mastersubmaster);
            _appDbContext.SaveChanges();

            return Ok();
        }

        [Route("PostTrans")]
        [HttpPost]
        public ActionResult posttrans(MoneyTransaction transaction)
        {
            //var result = _appDbContext.Tbltransactions.GroupBy(o=>1)
            //      .Select(g => g.Sum(i => i.amount) );

            var total = _appDbContext.Tbltransactions.Select(o => o.amount).Sum();
            var _balance = total + transaction.amount;
            transaction.balance = _balance;
            transaction.active = 1;
            transaction.createddate= DateTime.Now;
            var success = _appDbContext.Tbltransactions.Add(transaction);
            _appDbContext.SaveChanges();
            return Ok();
        }

        [Route("GetTrans")]
        [HttpGet]
        public ActionResult gettrans()
        {
            try
            {
                var dataset = (from t in _appDbContext.Tbltransactions
                               join sub in _appDbContext.TblSubgrouplegermaster on t.sublegerid equals sub.sublegerid
                               join grp in _appDbContext.Tblgrouplegermaster on sub.grouplegerid equals grp.grouplegerid
                               join lgt in _appDbContext.Tbllegertypemaster on t.legertypeid equals lgt.legertypeid
                               where t.active == 1
                               select new
                               {
                                   Transid = t.transid,
                                   Legername = grp.grouplegername,
                                   SubLegername = sub.sublegername,
                                   Legertype = lgt.legertypename,
                                   Amount = (double)t.amount,
                                   ClosingBalance = (double)t.balance,
                                   Date = t.createddate
                               }).OrderByDescending(e => e.Transid)
                .ToList();
                return Ok(dataset);
            }
            catch (Exception ex)
            {
                return Ok();

            }



        }

        [HttpGet]
        [Route("getsumofgroup")]
        public ActionResult getsumofgroup()
        {

            var result = (from trans in _appDbContext.Tbltransactions
                          join sub in _appDbContext.TblSubgrouplegermaster on trans.sublegerid equals sub.sublegerid
                          join grup in _appDbContext.Tblgrouplegermaster on sub.grouplegerid equals grup.grouplegerid
                          where trans.active == 1 && trans.amount<0
                          select new
                          {
                              groupname = grup.grouplegername,
                              amt = (double)trans.amount
                          })
                         .GroupBy(g => g.groupname)
                         .Select(x => new
                         {
                             GroupName = x.Key,
                             Amount = x.Sum(d => d.amt)

                         }).OrderBy(o => o.Amount);
            return Ok(result);
        }

        [HttpGet]
        [Route("getsubgroupofsum")]
        public ActionResult getsubgroupofsum()
        {
            var subgroupsum = (from trans in _appDbContext.Tbltransactions
                               join sub in _appDbContext.TblSubgrouplegermaster on trans.sublegerid equals sub.sublegerid
                               where trans.active == 1 && trans.amount < 0
                               select new
                               {
                                   subgroupname = sub.sublegername,
                                   amt = (double)trans.amount
                               })
                             .GroupBy(g => g.subgroupname)
                             .Select(s => new
                             {
                                 Subgroupname = s.Key,
                                 Amount = s.Sum(a => a.amt)
                             }).OrderBy(o => o.Amount);

            return Ok(subgroupsum);
        }

        [HttpGet]
        [Route("getsummary")]
        public ActionResult gettotalincome()
        {

            var totalincome = (from trans in _appDbContext.Tbltransactions
                              where trans.active == 1 && trans.legertypeid == 1
                              select new
                              {
                                  Amount = trans.amount
                              }).Sum(a=>a.Amount);
            var totalout = (from trans in _appDbContext.Tbltransactions
                               where trans.active == 1 && trans.legertypeid == 2
                               select new
                               {
                                   Amount = trans.amount
                               }).Sum(a => a.Amount);
            var totalcurrent = (from trans in _appDbContext.Tbltransactions
                            where trans.active == 1 
                            select new
                            {
                                Amount = trans.amount
                            }).Sum(a => a.Amount);

            return Ok(new {income=totalincome,outcome=totalout,currentbalance=totalcurrent});
        }
    }
}
