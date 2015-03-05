﻿angular.module('virtoCommerce.storeModule.blades')
.controller('storeSettingsListController', ['$scope', 'bladeNavigationService', function ($scope, bladeNavigationService) {
    $scope.openBlade = function (data) {
        var newBlade = {
            id: 'storeSetting',
            origEntity: data,
            title: $scope.blade.title,
            subtitle: 'Edit store setting',
            controller: 'storeSettingDetailController',
            template: 'Modules/Store/VirtoCommerce.StoreModule.Web/Scripts/blades/store-setting-detail.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, $scope.blade);
    }

    $scope.blade.onClose = function (closeCallback) {
        closeChildrenBlades();
        closeCallback();
    };

    function closeChildrenBlades() {
        angular.forEach($scope.blade.childrenBlades.slice(), function (child) {
            bladeNavigationService.closeBlade(child);
        });
    }

    function openAddEntityBlade() {
        var data = {
            isNew: true
        };
        $scope.openBlade(data);
    }

    $scope.bladeHeadIco = 'fa fa-archive';

    $scope.bladeToolbarCommands = [
        {
            name: "Add", icon: 'fa fa-plus',
            executeMethod: function () {
                openAddEntityBlade();
            },
            canExecuteMethod: function () {
                return true;
            }
        }
    ];

    $scope.blade.isLoading = false;
    $scope.$watch('blade.parentBlade.currentEntity.settings', function (currentEntities) {
        $scope.blade.currentEntities = currentEntities;
    });

    // open blade for new setting
    if (!_.some($scope.blade.currentEntities)) {
        openAddEntityBlade();
    }
}]);
