﻿using System;

namespace Samsonite.Library.Bussness.WebApi.Models
{
    public class GetSparePartRequest : ApiRequest
    {
        public string Sku { get; set; }

        public int GroupID { get; set; }
    }
}
