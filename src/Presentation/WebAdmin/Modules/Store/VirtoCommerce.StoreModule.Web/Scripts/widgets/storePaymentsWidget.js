﻿angular.module('virtoCommerce.storeModule.widgets')
.controller('storePaymentsWidgetController', ['$scope', 'bladeNavigationService', function ($scope, bladeNavigationService) {
    var blade = $scope.widget.blade;
    
    $scope.openBlade = function () {
        var newBlade = {
            id: "storeChildBlade",
            currentEntities: blade.currentEntity.paymentGateways,
            title: blade.title,
            subtitle: 'Manage store payments',
            controller: 'storePaymentsListController',
            template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/blades/store-payments-list.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };
}]);