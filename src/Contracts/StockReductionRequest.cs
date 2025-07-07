using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public class StockReductionRequest
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}
