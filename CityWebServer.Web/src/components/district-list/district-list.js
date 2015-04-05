define(['knockout', 'knockout-mapping', 'text!./district-list.html'], function (ko, komapping, templateMarkup) {

    function DistrictListViewModel(params) {
        var self = this;

        //self.endpoint = "../CityInfo";
        self.endpoint = "http://localhost:8080/CityInfo";

        self.lastDate = ko.observable();
        //self.data = ko.observable('');

        self.name = ko.observable();
        self.districts = ko.observableArray([]);
        self.global = ko.observable();

        self.refresh = function () {
            $.getJSON(self.endpoint, function (response) {
                var districtMapping = {
                    create: function (options) {
                        var innerModel = komapping.fromJS(options.data);

                        var householdsPercentage = '100%';
                        var current = innerModel.CurrentHouseholds();
                        var available = innerModel.AvailableHouseholds();
                        if (current > 0 && available > 0) {
                            householdsPercentage = (Math.round((current / available) * 100)) + '%';
                        }
                        innerModel.HouseholdsPercentage = householdsPercentage;

                        return innerModel;
                    }
                };

                komapping.fromJS(response.Districts, districtMapping, self.districts);
                komapping.fromJS(response.GlobalDistrict, districtMapping, self.global);
            }).fail(function () {
                console.log("Request failed.  Is the server running?");
            });
        };

        // Update the data once every second.
        self.intervalReference = window.setInterval(function () {
            self.refresh();
        }, 1000);
    }

    DistrictListViewModel.prototype.dispose = function () {
        window.clearinterval(self.intervalReference);
    };

    return { viewModel: DistrictListViewModel, template: templateMarkup };
});
