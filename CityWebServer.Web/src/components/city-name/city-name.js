define(['knockout', 'text!./city-name.html'], function(ko, templateMarkup) {

  function CityName(params) {
    this.name = ko.observable("My City Name");
  }

  // This runs when the component is torn down. Put here any logic necessary to clean up,
  // for example cancelling setTimeouts or disposing Knockout subscriptions/computeds.
  CityName.prototype.dispose = function() { };
  
  return { viewModel: CityName, template: templateMarkup };

});
