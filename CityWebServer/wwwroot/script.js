var graphChoice = 'Default';
var dataDisplay = 'spline';
var querySelect = document.querySelectorAll('#graphOptions div');
var querySelect2 = document.querySelectorAll('#displayOptions div');
var endpoint = "./CityInfo";
var viewModel;
var chart;
var initialized = false;
var lastDate;
let history = [];

for(var i = 0; i < querySelect.length; i++){
	querySelect[i].addEventListener("click", updateGraphChoice);
}

for(var i = 0; i < querySelect2.length; i++){
	querySelect2[i].addEventListener("click", updateGraphChoice2);
}


function updateGraphChoice(e){
	
	graphChoice = e.currentTarget.innerHTML;
	chart.destroy();
	chart = initializeSplineChart(viewModel);
	updateChart(viewModel, chart);
	
}

function updateGraphChoice2(e){
	
	dataDisplay = e.currentTarget.innerHTML.toLowerCase();
	if(dataDisplay == 'timeline'){
	dataDisplay = 'spline'
}
	chart.destroy();
	chart = initializeSplineChart(viewModel);
	updateChart(viewModel, chart);
	
}

function initializeSplineChart() {
var t = graphChoice;
var d;
if(graphChoice == "Default"){
t = "Statistics"
}
if(dataDisplay == 'spline'){
d = "datetime";
}
if(dataDisplay == 'column' || dataDisplay == 'pie'){
d = "linear";
}
    var c = new Highcharts.Chart({
    chart: {
            renderTo: 'chart',
            defaultSeriesType: dataDisplay,
            events: { }
        },
        title: {
            text: t
        },
        xAxis: {
            type: d,
            tickPixelInterval: 150
        },
        yAxis: {
            minPadding: .02,
            maxPadding: .02,
            title: {
                text: 'Values',
                margin: 5
            }
        }
    });
    return c;
}


function addOrUpdateSeries(theChart, seriesName, value, valueName)
{
    var series;
    var matchFound = false;
	
    if(theChart.series.length > 0)
    {
        for(var s = 0; s < theChart.series.length; s++)
        {
            if(theChart.series[s].name == seriesName)
            {
                series = theChart.series[s];
                matchFound = true;
                s = theChart.series.length;
            }
        }
    }

    if(!matchFound && dataDisplay == 'spline')
    {
        var seriesOptions = {
            id: seriesName,
            name: seriesName,
            data: [{ name: valueName, y: value}]
        };
        series = theChart.addSeries(seriesOptions, false);
    } else if(!matchFound && dataDisplay == 'column')
    {
        var seriesOptions = {
            id: seriesName,
            name: valueName,
            data: [{ name: valueName, y: value}]
        };
        series = theChart.addSeries(seriesOptions, false);
    } else if(!matchFound && dataDisplay == 'pie')
    {
        var seriesOptions = {
            id: seriesName,
            name: seriesName,
            data: value
        };
        series = theChart.addSeries(seriesOptions, false);
    }
    else if(matchFound)
    {
	console.log(series);
	 var shift = series.data.length > 20;
	series.addPoint(value, true, shift);
    }
}

function updateChart(vm, chart)
{
    var pieData = [];
    var districts = vm.Districts();

    var dataPointX; 
    var seriesName;
    var dataPoint;

    for(var i = 0; i < districts.length; i++)
    {
        var district = districts[i];
	seriesName = district.DistrictName();

	if(graphChoice == "Population"){
		pointYPopulation = district.TotalPopulationCount();
	}
	if(graphChoice == "Tourists"){
		pointYPopulation = district.WeeklyTouristVisits();
	}
	if(graphChoice == "Households"){
		pointYPopulation = district.CurrentHouseholds();
	}
	if(graphChoice == "Jobs Occupied"){
		pointYPopulation = district.CurrentJobs();
	}
	if(graphChoice == "Household Vacancies"){
		pointYPopulation = district.AvailableHouseholds()-district.CurrentHouseholds();
	}
	if(graphChoice == "Job Vacancies"){
		pointYPopulation = district.AvailableJobs()-district.CurrentJobs();
	}
	if(graphChoice == "Household Fullness"){
		pointYPopulation = doMath(district.CurrentHouseholds(), district.AvailableHouseholds());
	}
	if(graphChoice == "Job Fullness"){
		pointYPopulation = doMath(district.CurrentJobs(), district.AvailableJobs());
	}
	if(graphChoice == "Tourist Saturation"){
		pointYPopulation = doMath(district.WeeklyTouristVisits(), district.TotalPopulationCount());
	}
	if(graphChoice == "Default"){
		graphChoice = 'Population';
		pointYPopulation = district.TotalPopulationCount();
	}
	if(dataDisplay == "column"){
		dataPointX = seriesName;
		dataPointY = pointYPopulation;
		addOrUpdateSeries(chart, seriesName, dataPointY, dataPointX);
	
	}
	if(dataDisplay == "spline"){
		dataPointX = formatDate(vm.Time());
		dataPointY = pointYPopulation;
		addOrUpdateSeries(chart, seriesName, dataPointY, dataPointX);
	}
	if(dataDisplay == 'pie'){
		dataPointX = seriesName;
		pieData.push({name: seriesName, y: pointYPopulation})
		dataPointY = pieData;
	}
	
	
   }

if(dataDisplay == 'pie'){
	addOrUpdateSeries(chart, graphChoice, dataPointY, dataPointX);
}
    chart.redraw();


}

function formatDate(t){
	t = Date.parse(t);
	return t;
}

function doMath(v1,v2){
	var r = 0;

	if(!isFinite((v1 / v2)*100) || isNaN((v1 / v2)*100)){
		r = 0;
	} else {
		r = ((v1 / v2)*100).toFixed(0);
		r = parseInt(r);
}
	return r;
}

function filterTable() {
  // Declare variables
  var input, filter, table, tr, td, i, txtValue;
  input = document.getElementById("searchInput");
  filter = input.value.toUpperCase();
  table = document.getElementById("mainDataTable");
  tr = table.getElementsByTagName("tr");

  // Loop through all table rows, and hide those who don't match the search query
  for (i = 0; i < tr.length; i++) {
    td = tr[i].getElementsByTagName("td")[0];
    if (td) {
      txtValue = td.textContent || td.innerText;
      if (txtValue.toUpperCase().indexOf(filter) > -1) {
        tr[i].style.display = "";
      } else {
        tr[i].style.display = "none";
      }
    }
  }
}

function sortTable(n) {
  var table, rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
  table = document.getElementById("mainDataTable");
  switching = true;
  //Set the sorting direction to ascending:
  dir = "asc"; 
  /*Make a loop that will continue until
  no switching has been done:*/
  while (switching) {
    //start by saying: no switching is done:
    switching = false;
    rows = table.rows;
    /*Loop through all table rows (except the
    first, which contains table headers):*/
    for (i = 1; i < (rows.length - 1); i++) {
      //start by saying there should be no switching:
      shouldSwitch = false;
      /*Get the two elements you want to compare,
      one from current row and one from the next:*/
      x = rows[i].getElementsByTagName("TD")[n];
      y = rows[i + 1].getElementsByTagName("TD")[n];
      /*check if the two rows should switch place,
      based on the direction, asc or desc:*/
      if (dir == "asc") {
        if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
          //if so, mark as a switch and break the loop:
          shouldSwitch= true;
          break;
        }
      } else if (dir == "desc") {
        if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
          //if so, mark as a switch and break the loop:
          shouldSwitch = true;
          break;
        }
      }
    }
    if (shouldSwitch) {
      /*If a switch has been marked, make the switch
      and mark that a switch has been done:*/
      rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
      switching = true;
      //Each time a switch is done, increase this count by 1:
      switchcount ++;      
    } else {
      /*If no switching has been done AND the direction is "asc",
      set the direction to "desc" and run the while loop again.*/
      if (switchcount == 0 && dir == "asc") {
        dir = "desc";
        switching = true;
      }
    }
  }
}

 
    
    // Update the data once every second.
    window.setInterval(function ()
    {
        $.getJSON(endpoint, function(data) {
            if (initialized) {
                ko.mapping.fromJS(data, viewModel);
                if (lastDate !== viewModel.Time()) {
                    updateChart(viewModel, chart);
                    lastDate = viewModel.Time();
                } else {
                    //console.log("Same Date: " + lastDate);
                }
            } else {
                initialized = true;
                viewModel = ko.mapping.fromJS(data);
                ko.applyBindings(viewModel);
		chart = initializeSplineChart(viewModel);
            }
        });
    }, 2000); // Every two seconds.

	function appendHistoryTable(){
		console.log(history)
	}
