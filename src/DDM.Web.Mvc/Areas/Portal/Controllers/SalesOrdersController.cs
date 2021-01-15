﻿using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using DDM.Web.Areas.Portal.Models.SalesOrders;
using DDM.Web.Controllers;
using DDM.Authorization;
using DDM.SalesOrders;
using DDM.SalesOrders.Dtos;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using System.Collections.Generic;
using DDM.SalesOrderLines;

namespace DDM.Web.Areas.Portal.Controllers
{
    [Area("Portal")]
    [AbpMvcAuthorize(AppPermissions.Pages_SalesOrders)]
    public class SalesOrdersController : DDMControllerBase
    {
        private readonly ISalesOrdersAppService _salesOrdersAppService;
      
        public SalesOrdersController(ISalesOrdersAppService salesOrdersAppService)
        {
            _salesOrdersAppService = salesOrdersAppService;
        }

        public ActionResult Index()
        {
            var model = new SalesOrdersViewModel
            {
                FilterText = ""
            };

            return View(model);
        }

        [AbpMvcAuthorize(AppPermissions.Pages_SalesOrders_Create, AppPermissions.Pages_SalesOrders_Edit)]
        public async Task<PartialViewResult> CreateOrEditModal(int? id)
        {
            GetSalesOrderForEditOutput getSalesOrderForEditOutput;

            if (id.HasValue)
            {
                getSalesOrderForEditOutput = await _salesOrdersAppService.GetSalesOrderForEdit(new EntityDto { Id = (int)id });


                //TODO : GET LINES BASED ON ORDER ID
                //getSalesOrderLineForEditOutput

                //getSalesOrderLineForEditOutput = new GetSalesOrderLineForEditOutput
                //{
                //    SalesOrderLines = new List<CreateOrEditSalesOrderLineDto>()
                //};

            }

            else
            {
                //Empty models
                var salesOrder = new CreateOrEditSalesOrderDto();
                var line1 = new SalesOrderLineDto
                {
                    Name="line 1",
                    Description = "desc line 1", 
                    SalesOrderId = 1,
                    MachineId = 1,
                    MaterialId = 1
                };

                var line2 = new SalesOrderLineDto
                {
                    Name = "line 2",
                    Description = "desc line 2", 
                    SalesOrderId = 1,
                    MachineId = 1,
                    MaterialId = 1

                };

                var salesOrderLines = new List<SalesOrderLineDto>();
                salesOrderLines.Add(line1);
                //salesOrderLines.Add(line2);

                salesOrder.SalesOrderLines = salesOrderLines;

                getSalesOrderForEditOutput = new GetSalesOrderForEditOutput
                {
                    SalesOrder = salesOrder
                };


                ////TODO : CREATE NEW LINE LIST WITH DEFAULT VALUES including material and machine selection list
                //var orderLine = new CreateOrEditSalesOrderLineDto();
                //orderLine.Id = 0;
                //orderLine.Name = "";
                //orderLine.Description = "";

                //orderLine.MachineList = await _salesOrderLinesAppService.GetAllMachineForTableDropdown();
                //orderLine.MaterialList = await _salesOrderLinesAppService.GetAllMaterialForTableDropdown();

                //getSalesOrderLineForEditOutput = new GetSalesOrderLineForEditOutput
                //{
                //    SalesOrderLines = new List<CreateOrEditSalesOrderLineDto>()
                //};

                //getSalesOrderLineForEditOutput.SalesOrderLines.Add(orderLine);

            }


            var viewModel = new CreateOrEditSalesOrderModalViewModel()
            {
                SalesOrder = getSalesOrderForEditOutput.SalesOrder,
                CustomerName = getSalesOrderForEditOutput.CustomerName,

                SalesOrderCustomerList = await _salesOrdersAppService.GetAllCustomerForTableDropdown(),
                SalesOrderMachineList = await _salesOrdersAppService.GetAllMachineForTableDropdown(),
                SalesOrderMaterialList = await _salesOrdersAppService.GetAllMaterialForTableDropdown()
            };

            return PartialView("_CreateOrEditModal", viewModel);
        }

        public async Task<PartialViewResult> ViewSalesOrderModal(int id)
        {
            var getSalesOrderForViewDto = await _salesOrdersAppService.GetSalesOrderForView(id);

            var model = new SalesOrderViewModel()
            {
                SalesOrder = getSalesOrderForViewDto.SalesOrder,
                CustomerName = getSalesOrderForViewDto.CustomerName

            };

            return PartialView("_ViewSalesOrderModal", model);
        }

    }
}