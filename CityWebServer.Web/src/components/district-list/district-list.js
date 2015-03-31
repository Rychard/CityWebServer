define(['knockout', 'knockout-mapping', 'text!./district-list.html'], function (ko, komapping, templateMarkup) {

  function DistrictList(params) {
    var self = this;
    
    self.endpoint = "../CityInfo";
    self.initialized = ko.observable(false);
    self.lastDate = ko.observable();
    self.data = ko.observable();
    
    self.refresh = function() {
        $.getJSON(self.endpoint, function (data) {
            console.log(data);
            self.data(komapping.mapping.fromJS(data));
            if (self.lastDate() !== self.data().Time()) {
                // updateChart(self.data(), chart);
                self.lastDate(self.data().Time());
            } else {
                console.log("Same Date: " + self.lastDate());
            }
        }).fail(function() { 
            console.log("Request failed.  Is the server running?");
        });
    };

    // Update the data once every second.
    self.intervalReference = window.setInterval(function () {
        self.refresh();
    }, 2000); // Every two seconds.    
  }

  // This runs when the component is torn down. Put here any logic necessary to clean up,
  // for example cancelling setTimeouts or disposing Knockout subscriptions/computeds.
  DistrictList.prototype.dispose = function() { 
    window.clearinterval(self.intervalReference);
  };
  
  return { viewModel: DistrictList, template: templateMarkup };

});
