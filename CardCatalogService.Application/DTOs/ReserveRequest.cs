using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardCatalogService.Application.DTOs
{
    public class ReserveRequest
    {
        public int CardId { get; set; }
        public int UserId { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }
    }
}
