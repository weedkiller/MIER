﻿using Abp.Application.Services.Dto;
using System;

namespace DDM.ProductionStatuses.Dtos
{
    public class GetAllProductionStatusesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string DescriptionFilter { get; set; }

    }
}