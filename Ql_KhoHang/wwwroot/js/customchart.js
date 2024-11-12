document.addEventListener("DOMContentLoaded", function () {
    $.ajax({
        type: "GET",
        url: "/Home/Chart",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccess,
    });

    function OnSuccess(data) {
        const myChart = document.getElementById('myChart');
        const pieChart = document.getElementById('pieChart');
        var _data = data;
        var _labels = _data[0];
        var _chartData = _data[1];
        var _labelsPie = _data[2];
        var _chartDataPie = _data[3];
        var colors = ['rgba(255, 99, 132, 0.2)', 'rgba(255, 159, 64, 0.2)', 'rgba(255, 205, 86, 0.2)', 'rgba(75, 192, 192, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(153, 102, 255, 0.2)', 'rgba(201, 203, 207, 0.2)'];
        var colorspie = ['rgb(255, 99, 132)', 'rgb(54, 162, 235)', 'rgb(255, 205, 86)'];


        new Chart(myChart,
            {
                type: 'bar',
                data: {
                    labels: _labels, // Thay đổi từ label thành labels
                    datasets: [{
                        backgroundColor: colors,
                        data: _chartData,
                        borderWidth: 1,
                    }]
                },
                options: {
                    plugins: {
                        legend: {
                            display: false,
                        }
                    }
                }
            });
        new Chart(pieChart,
            {
                type: 'pie',
                data: {
                    labels: _labelsPie, // Thay đổi từ label thành labels
                    datasets: [{
                        backgroundColor: colorspie,
                        data: _chartDataPie,
                        borderWidth: 1.5,
                        hoverOffset: 4
                    }]
                },
                options: {
                    plugins: {
                        legend: {
                            display: false,
                        }
                    },
                    responsive: true,
                    maintainAspectRatio: false
                }
            });
    }
});
