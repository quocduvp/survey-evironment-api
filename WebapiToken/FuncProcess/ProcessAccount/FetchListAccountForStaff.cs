using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebapiToken.Models;
using WebapiToken.Models.AccountModel;

namespace WebapiToken.FuncProcess
{
    public class FetchListAccountForStaff
    {
        private static DBS db = new DBS();
        public static async Task<int> getTotalRecord()
        {
            var total = (from a in db.accounts where a.role_id == 1 && a.status == true select a).Count();
            return total;
        }

        public static async Task<int> getTotalPage(int page_size)
        {
            int totalPage = (int)(Math.Ceiling((double)await getTotalRecord() / page_size));
            return totalPage;
        }

        //list account for student
        public static async Task<IQueryable<Lists>> getListAccount(int skip_row, int page_size)
        {
            // lấy danh sách account trên csdl
            var accounts = (from a in db.accounts
                            from b in db.roles
                            where a.role_id == b.id && a.role_id == 1 && a.status == true
                            orderby a.id ascending
                            select new Lists
                            {
                                id = a.id,
                                role_id = a.role_id,
                                card_id = a.card_id,
                                username = a.username,
                                section = a.section,
                                status = a.status,
                                date_join = a.date_join,
                                create_at = a.create_at,
                                role = new RoleAccount
                                {
                                    id = b.id,
                                    role_name = b.role_name,
                                    create_at = b.create_at
                                }
                            }).Skip(skip_row).Take(page_size);
            return accounts;
        }

        //get all account , support panigation
        public static async Task<AllAccounts> getAllAccount(int page, int page_size)
        {
            //record của từng trang
            var skip_row = (page - 1) * page_size;
            //lấy về api list account
            var AllAccount = new AllAccounts
            {
                lists = await getListAccount(skip_row, page_size),
                total = await getTotalRecord(),
                total_page = await getTotalPage(page_size),
                page_size = page_size,
                page = page
            };
            return AllAccount;
        }
    }
}