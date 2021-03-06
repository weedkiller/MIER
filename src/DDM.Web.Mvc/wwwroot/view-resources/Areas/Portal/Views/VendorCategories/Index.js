﻿(function () {
    $(function () {

        var _$vendorCategoriesTable = $('#VendorCategoriesTable');
        var _vendorCategoriesService = abp.services.app.vendorCategories;
		
        $('.date-picker').datetimepicker({
            locale: abp.localization.currentLanguage.name,
            format: 'L'
        });

        var _permissions = {
            create: abp.auth.hasPermission('Pages.VendorCategories.Create'),
            edit: abp.auth.hasPermission('Pages.VendorCategories.Edit'),
            'delete': abp.auth.hasPermission('Pages.VendorCategories.Delete')
        };

         var _createOrEditModal = new app.ModalManager({
                    viewUrl: abp.appPath + 'Portal/VendorCategories/CreateOrEditModal',
                    scriptUrl: abp.appPath + 'view-resources/Areas/Portal/Views/VendorCategories/_CreateOrEditModal.js',
                    modalClass: 'CreateOrEditVendorCategoryModal'
                });
                   

		 var _viewVendorCategoryModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Portal/VendorCategories/ViewvendorCategoryModal',
            modalClass: 'ViewVendorCategoryModal'
        });

		
		

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
        }
        
        var getMaxDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT23:59:59Z"); 
        }

        var dataTable = _$vendorCategoriesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _vendorCategoriesService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#VendorCategoriesTableFilter').val(),
					nameFilter: $('#NameFilterId').val(),
					descriptionFilter: $('#DescriptionFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    className: 'control responsive',
                    orderable: false,
                    render: function () {
                        return '';
                    },
                    targets: 0
                },
                {
                    width: 120,
                    targets: 1,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text: '<i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
						{
                                text: app.localize('View'),
                                iconStyle: 'far fa-eye mr-2',
                                action: function (data) {
                                    _viewVendorCategoryModal.open({ id: data.record.vendorCategory.id });
                                }
                        },
						{
                            text: app.localize('Edit'),
                            iconStyle: 'far fa-edit mr-2',
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                            _createOrEditModal.open({ id: data.record.vendorCategory.id });                                
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            iconStyle: 'far fa-trash-alt mr-2',
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteVendorCategory(data.record.vendorCategory);
                            }
                        }]
                    }
                },
					{
						targets: 2,
						 data: "vendorCategory.name",
						 name: "name"   
					},
					{
						targets: 3,
						 data: "vendorCategory.description",
						 name: "description"   
					}
            ]
        });

        function getVendorCategories() {
            dataTable.ajax.reload();
        }

        function deleteVendorCategory(vendorCategory) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _vendorCategoriesService.delete({
                            id: vendorCategory.id
                        }).done(function () {
                            getVendorCategories(true);
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

		$('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideUp();
        });

        $('#CreateNewVendorCategoryButton').click(function () {
            _createOrEditModal.open();
        });        

		

        abp.event.on('app.createOrEditVendorCategoryModalSaved', function () {
            getVendorCategories();
        });

		$('#GetVendorCategoriesButton').click(function (e) {
            e.preventDefault();
            getVendorCategories();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getVendorCategories();
		  }
		});
		
		
		
    });
})();
