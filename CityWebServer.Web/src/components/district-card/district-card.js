define(['knockout', 'text!./district-card.html'], function(ko, templateMarkup) {

  function DistrictCard(params) {
    this.message = ko.observable('Hello from the district-card component!');
  }

  // This runs when the component is torn down. Put here any logic necessary to clean up,
  // for example cancelling setTimeouts or disposing Knockout subscriptions/computeds.
  DistrictCard.prototype.dispose = function() { };
  
  return { viewModel: DistrictCard, template: templateMarkup };

});
