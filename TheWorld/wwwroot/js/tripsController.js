// tripsController.js

(function () {

    "use strict";

	// Getting the existing module

    angular.module("app-trips")
        .controller("tripsController", tripsController);

    function tripsController($http) {

        var vm = this;
        vm.trips = [];


            //[{

   //         name: "US Trip",
			//created: new Date()
   //     },
   //         {
   //         name: "SA Trip",
   //         created: new Date()
   //     },
   //         {
			//	name: "World Trip",
			//	created: new Date()
   //         }];

        // object for getting new trip using input 
        vm.newTrip = {};
        vm.errorMessage = "";

        // busy flag
        vm.isBusy = true;

        $http.get("/api/trips")
            .then(function (response) {
                //Success
                angular.copy(response.data, vm.trips);
            }, function () {
                //Failure
                vm.errorMessage = "Failed to load data: " + Error;
            })
            .finally(function () {
              vm.isBusy = false;
            });
        // pushing data to the database 
        vm.addTrip = function () {
            //vm.trips.push({
            //    name: vm.newTrip.name, created: new Date()
            
            //});
            //// clear the form 
            //vm.newTrip = {};

            vm.isBusy = true;
            vm.errorMessage = "";
            $http.post("/api/trips", vm.newTrip)
                .then(function (response) {
                    //success
                    vm.trips.push(response.data);
                    vm.newTrip = {};
                }, function () {
                    // failure
                    vm.errorMessage = " Failed to save new trip";
                })
                .finally(function () {
                    vm.isBusy = false;
                });
        };


    }
})();