using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebapiToken.Models.Panigation
{
    public class Page
    {
        public int page_size { get; set; }
        public int page { get; set; }
        public int _page_size
        {
            get
            {
                if (this.page_size <= 0)
                    return 1;
                else
                    return this.page_size;
            }
            set
            {
                this.page_size = value;
            }
        }
        public int _page
        {
            get
            {
                if (this.page <= 0)
                    return 1;
                else
                    return this.page;
            }
            set
            {
                this.page = value;
            }
        }
    }
}