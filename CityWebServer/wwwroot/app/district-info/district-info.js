define(['knockout', 'text!./district-info.html'], function (ko, htmlString) {

    function DistrictInfoViewModel(params) {
        
        var endpoint = "../CityInfo";
        var viewModel;
        var chart;
        var initialized = false;
        var lastDate;

        // Update the data once every second.
        window.setInterval(function () {
            $.getJSON(endpoint, function (data) {
                if (initialized) {
                    ko.mapping.fromJS(data, viewModel);
                    if (lastDate !== viewModel.Time()) {
                        updateChart(viewModel, chart);
                        lastDate = viewModel.Time();
                    } else {
                        console.log("Same Date: " + lastDate);
                    }
                } else {
                    initialized = true;
                    viewModel = ko.mapping.fromJS(data);
                    ko.applyBindings(viewModel);
                    chart = initializeChart(viewModel);
                }
            });
        }, 2000); // Every two seconds.
    }

    return {
        viewModel: DistrictInfoViewModel,
        template: htmlString
    }
});



