define(['knockout', 'knockout-mapping', 'text!./district-list.html'], function (ko, komapping, templateMarkup) {

    function DistrictListViewModel(params) {
        var self = this;

        //self.endpoint = "../CityInfo";
        self.endpoint = "http://localhost:8080/CityInfo";

        self.lastDate = ko.observable();
        self.data = ko.observable('');

        self.name = ko.observable();
        self.districts = ko.observableArray([]);
        self.global = ko.observable();

        self.refresh = function () {
            $.getJSON(self.endpoint, function (response) {
                //console.log(response);

                komapping.fromJS(response.Districts, {}, self.districts);
                komapping.fromJS(response.GlobalDistrict, {}, self.global);

                self.data(response);
                console.log(self.data());

                //self.name(response.name);
                //self.districts(response.districts);
                //var model = ko.observable();
                //komapping.fromJS(response, {}, model);
                //self.data(model());

                //console.log(model());
                //if (self.lastDate() !== self.data().Time()) {
                //    // updateChart(self.data(), chart);
                //    self.lastDate(self.data().Time());
                //} else {
                //    console.log("Same Date: " + self.lastDate());
                //}
            }).fail(function () {
                console.log("Request failed.  Is the server running?");
            });
        };

        // Update the data once every second.
        self.intervalReference = window.setInterval(function () {
            self.refresh();
        }, 1000); // Every two seconds.    
    }

    // This runs when the component is torn down. Put here any logic necessary to clean up,
    // for example cancelling setTimeouts or disposing Knockout subscriptions/computeds.
    DistrictListViewModel.prototype.dispose = function () {
        window.clearinterval(self.intervalReference);
    };

    return { viewModel: DistrictListViewModel, template: templateMarkup };

});
