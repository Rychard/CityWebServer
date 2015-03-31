define(['knockout', 'text!./city-name.html'], function (ko, htmlString) {
    function CityNameViewModel(params) {
        var self = this;

        self.cityname = ko.observable("My City Name");

    }

    return {
        viewModel: CityNameViewModel,
        template: htmlString
    }
});



