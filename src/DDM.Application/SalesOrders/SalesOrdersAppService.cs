﻿using DDM.Customers;
using DDM.Machines;
using DDM.Materials;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using DDM.SalesOrders.Dtos;
using DDM.Dto;
using Abp.Application.Services.Dto;
using DDM.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;


namespace DDM.SalesOrders
{
    [AbpAuthorize(AppPermissions.Pages_SalesOrders)]
    public class SalesOrdersAppService : DDMAppServiceBase, ISalesOrdersAppService
    {
        private readonly IRepository<SalesOrder> _salesOrderRepository;
        private readonly IRepository<Customer, int> _lookup_customerRepository;
        private readonly IRepository<Machine, int> _lookup_machineRepository;
        private readonly IRepository<Material, int> _lookup_materialRepository;

        public SalesOrdersAppService(IRepository<SalesOrder> salesOrderRepository, 
            IRepository<Customer, int> lookup_customerRepository,
            IRepository<Machine, int> lookup_machineRepository,
            IRepository<Material, int> lookup_materialRepository)
        {
            _salesOrderRepository = salesOrderRepository;
            _lookup_customerRepository = lookup_customerRepository;
            _lookup_machineRepository = lookup_machineRepository;
            _lookup_materialRepository = lookup_materialRepository;
        }

        public async Task<PagedResultDto<GetSalesOrderForViewDto>> GetAll(GetAllSalesOrdersInput input)
        {

            var filteredSalesOrders = _salesOrderRepository.GetAll()
                        .Include(e => e.CustomerFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Number.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NumberFilter), e => e.Number == input.NumberFilter)
                        .WhereIf(input.MinDateFilter != null, e => e.Date >= input.MinDateFilter)
                        .WhereIf(input.MaxDateFilter != null, e => e.Date <= input.MaxDateFilter)
                        .WhereIf(input.MinDueDateFilter != null, e => e.DueDate >= input.MinDueDateFilter)
                        .WhereIf(input.MaxDueDateFilter != null, e => e.DueDate <= input.MaxDueDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name == input.CustomerNameFilter);

            var pagedAndFilteredSalesOrders = filteredSalesOrders
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var salesOrders = from o in pagedAndFilteredSalesOrders
                              join o1 in _lookup_customerRepository.GetAll() on o.CustomerId equals o1.Id into j1
                              from s1 in j1.DefaultIfEmpty()

                              select new GetSalesOrderForViewDto()
                              {
                                  SalesOrder = new SalesOrderDto
                                  {
                                      Number = o.Number,
                                      Date = o.Date,
                                      DueDate = o.DueDate,
                                      Id = o.Id
                                  },
                                  CustomerName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                              };

            var totalCount = await filteredSalesOrders.CountAsync();

            return new PagedResultDto<GetSalesOrderForViewDto>(
                totalCount,
                await salesOrders.ToListAsync()
            );
        }

        public async Task<GetSalesOrderForViewDto> GetSalesOrderForView(int id)
        {
            var salesOrder = await _salesOrderRepository.GetAsync(id);

            var output = new GetSalesOrderForViewDto { SalesOrder = ObjectMapper.Map<SalesOrderDto>(salesOrder) };

            if (output.SalesOrder.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.SalesOrder.CustomerId);
                output.CustomerName = _lookupCustomer?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_SalesOrders_Edit)]
        public async Task<GetSalesOrderForEditOutput> GetSalesOrderForEdit(EntityDto input)
        {
            var salesOrder = await _salesOrderRepository.FirstOrDefaultAsync(input.Id);


            var output = new GetSalesOrderForEditOutput { SalesOrder = ObjectMapper.Map<CreateOrEditSalesOrderDto>(salesOrder) };


            if (output.SalesOrder.CustomerId != null)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.SalesOrder.CustomerId);
                output.CustomerName = _lookupCustomer?.Name?.ToString();
            }

            return output;

        }

        public async Task CreateOrEdit(CreateOrEditSalesOrderDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_SalesOrders_Create)]
        protected virtual async Task Create(CreateOrEditSalesOrderDto input)
        {
            var salesOrder = ObjectMapper.Map<SalesOrder>(input);

            await _salesOrderRepository.InsertAsync(salesOrder);
        }

        [AbpAuthorize(AppPermissions.Pages_SalesOrders_Edit)]
        protected virtual async Task Update(CreateOrEditSalesOrderDto input)
        {
            var salesOrder = await _salesOrderRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, salesOrder);
        }

        [AbpAuthorize(AppPermissions.Pages_SalesOrders_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _salesOrderRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_SalesOrders)]

        public async Task<List<SalesOrderCustomerLookupTableDto>> GetAllCustomerForTableDropdown()
        {
            return await _lookup_customerRepository.GetAll()
                .Select(customer => new SalesOrderCustomerLookupTableDto
                {
                    Id = customer.Id,
                    DisplayName = customer == null || customer.Name == null ? "" : customer.Name.ToString()
                }).ToListAsync();
        }

        public async Task<List<SalesOrderMachineLookupTableDto>> GetAllMachineForTableDropdown()
        {
            return await _lookup_machineRepository.GetAll()
                .Select(machine => new SalesOrderMachineLookupTableDto
                {
                    Id = machine.Id,
                    DisplayName = machine == null || machine.Name == null ? "" : machine.Name.ToString()
                }).ToListAsync();
        }

        public async Task<List<SalesOrderMaterialLookupTableDto>> GetAllMaterialForTableDropdown()
        {
            return await _lookup_materialRepository.GetAll()
                .Select(material => new SalesOrderMaterialLookupTableDto
                {
                    Id = material.Id,
                    DisplayName = material == null || material.Name == null ? "" : material.Name.ToString()
                }).ToListAsync();
        }

    }
}