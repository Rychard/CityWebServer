function initializeChart() {
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
        console.log("Adding series: " + seriesName);
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
    var districts = vm.Districts();
    for(var i = 0; i < districts.length; i++) 
    {
        var district = districts[i];        
        var districtName = district.DistrictName();
        
        var seriesName = districtName + " - Population";
        var population = district.TotalPopulationCount();
        addOrUpdateSeries(chart, seriesName, population, vm.Time());
    }
    chart.redraw();
}