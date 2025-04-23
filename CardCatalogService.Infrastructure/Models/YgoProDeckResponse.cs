using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardCatalogService.Infrastructure.Models
{
    public class YgoProDeckResponse
    {
        public List<ExternalCardDto> Data { get; set; } = new();
    }
}
