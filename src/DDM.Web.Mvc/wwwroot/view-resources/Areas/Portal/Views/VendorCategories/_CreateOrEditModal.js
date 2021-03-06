﻿(function ($) {
    app.modals.CreateOrEditVendorCategoryModal = function () {

        var _vendorCategoriesService = abp.services.app.vendorCategories;

        var _modalManager;
        var _$vendorCategoryInformationForm = null;

		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$vendorCategoryInformationForm = _modalManager.getModal().find('form[name=VendorCategoryInformationsForm]');
            _$vendorCategoryInformationForm.validate();
        };

		  

        this.save = function () {
            if (!_$vendorCategoryInformationForm.valid()) {
                return;
            }

            var vendorCategory = _$vendorCategoryInformationForm.serializeFormToObject();
			
			 _modalManager.setBusy(true);
			 _vendorCategoriesService.createOrEdit(
				vendorCategory
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditVendorCategoryModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);