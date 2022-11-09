function initializeSplineChart() {
    var c = new Highcharts.Chart({
    chart: {
            renderTo: 'chart',
            defaultSeriesType: 'spline',
            events: { }
        },
        title: {
            text: 'Statistics'
        },
        xAxis: {
            type: 'datetime',
            tickPixelInterval: 150
        },
        yAxis: {
            minPadding: 0.2,
            maxPadding: 0.2,
            title: {
                text: 'Value',
                margin: 80
            }
        }
    });
    return c;
}

function initializePieChart() {
    var c = new Highcharts.Chart({
    chart: {
            renderTo: 'chart2',
            defaultSeriesType: 'pie',
            events: { }
        },
        title: {
            text: 'Statistics'
        },
        xAxis: {
            type: 'datetime',
            tickPixelInterval: 150
        },
        yAxis: {
            minPadding: 0.2,
            maxPadding: 0.2,
            title: {
                text: 'Value',
                margin: 80
            }
        }
    });
    return c;
}

function deleteUnusedSeries(chart, seriesArray)
{
    // seriesArray is an array that contains only the NAMES of the series that were updated this tick.

    //for (var j = 0; j < chart.series.length; j++) {
    //    var series = chart.series[j];
    //    var isUsed = false;
    //    for (var i = 0; i < seriesArray.length; i++) {
    //        var usedSeriesName = seriesArray[i];
    //        if (series.name == usedSeriesName) {
    //            isUsed = true;
    //        }
    //    }

    //    if (!isUsed) {
    //        //console.log("Removing: " + chart.series[j].name);
    //        chart.series[j].remove();
    //        j = -1;
    //    }

    //}
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
                s = theChart.series.length; // Stop looping
            }
        }
    }

    if(!matchFound)
    {
        //console.log("Adding series: " + seriesName);
        var seriesOptions = {
            id: seriesName,
            name: seriesName,
            data: [{ name: valueName, y: value}]
        };
        series = theChart.addSeries(seriesOptions, false);
    }
    else
    {
        var shift = series.data.length > 20;
        series.addPoint(value, true, shift);
    }
}

function updateChart(vm, chart)
{
    var updatedSeries = [];
    var districts = vm.Districts();
    for(var i = 0; i < districts.length; i++)
    {
        var district = districts[i];
        var districtName = district.DistrictName();
	
        var seriesName = districtName + " - Population";
	var seriesName2 = districtName + " - Jobs";
	var seriesName3 = districtName + " - Density";
        var population = district.TotalPopulationCount();
	var jobs = district.CurrentJobs();
	addOrUpdateSeries(chart, seriesName, population, vm.Time());
	addOrUpdateSeries(chart, seriesName2, jobs, vm.Time());
	updatedSeries.push(seriesName);
        deleteUnusedSeries(chart, updatedSeries);
    }
    chart.redraw();
}
