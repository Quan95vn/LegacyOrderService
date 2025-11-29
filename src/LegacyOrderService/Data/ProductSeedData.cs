using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyOrderService.Data;

public static class ProductSeedData
{
    public static readonly Dictionary<string, double> Products = new()
    {
        ["Widget"] = 12.99,
        ["Gadget"] = 15.49,
        ["Doohickey"] = 8.75
    };
}
