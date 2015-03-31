requirejs.config({
    baseUrl: 'lib',
    paths: {
        app: '../app',
        jquery: 'jquery-2.1.3.min',
        'knockout': 'knockout-3.3.0',
        'knockoutmapping': 'knockout.mapping-latest',
        'bootstrap': 'bootstrap.min',
        'highchart': 'highcharts'
    }
});

define(['knockout', 'domready!'], function (ko) {
    //window.alert(ko);
    ko.components.register('city-name', {
        template: '<a class="navbar-brand" href="/"><span data-bind="text: name"></span></a>',
        viewModel: function() {
            var self = this;
            self.name =ko.observable("City Name");
        }
    });
    ko.components.register('district-info', { require: 'app/district-info/district-info' });

    ko.applyBindings();
});