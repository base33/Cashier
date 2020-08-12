
class cashierController {
    constructor($scope, cashierService) {
        this.$scope = $scope
        this.cashierService = cashierService

        this.pageSize = 10
        this.take = 0

        this.items = []

        this.load(this.take, this.pageSize)
    }

    load(skip, take) {
        this.cashierService.getPaged(skip, take).then(r => {
            this.items = r.data;
        })
    }    
}

class cashierService {
    constructor($http) {
        this.$http = $http;
    }

    getPaged(skip, take) {
        return this.$http.get(`/umbraco/backoffice/api/cashierBackofficeApi/getPaged?take=${take}&skip=${skip}`);
    }
}

angular.module("umbraco").service("cashierService", ["$http", function ($http) {
    return new cashierService($http);
}]);
angular.module("umbraco").controller("cashierController", ["$scope", "cashierService", function ($scope, cashierService) {
    return new cashierController($scope, cashierService);
}]);