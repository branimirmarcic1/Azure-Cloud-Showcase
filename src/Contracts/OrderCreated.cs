﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public record OrderCreated(
    Guid OrderId,
    string ProductName,
    int Quantity
);