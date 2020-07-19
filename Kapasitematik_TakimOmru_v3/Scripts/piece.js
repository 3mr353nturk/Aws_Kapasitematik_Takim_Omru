'use strict';
//var html = '<div class="progress progress-sm"><div class="progress-bar kt-bg-primary" role="progressbar" style="width: 25%;" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100"</div></div>'; 

var chartReg = {};
var KTDatatablee = function () {
    $(function () {
        // Declare a proxy to reference the hub.
        var noti = $.connection.myHub;
        //debugger;
        // Create a function that the hub can call to broadcast messages.
        noti.client.updatemessage = function () {
            alert('uyarı');
            Kategorii();
        };
        // Start the connection.
        $.connection.hub.start().done(function () {
            //console.log("connection started")
            //notifications.onconn();
            Kategorii();
        }).fail(function (s) {
            console.log('hata');
            alert(s);
        });
    });
    var datatable;
    $(function () {
        // Declare a proxy to reference the hub.
        var notis = $.connection.myHub;

        //debugger;
        // Create a function that the hub can call to broadcast messages.
        notis.client.griddetay = function (value) {

            alert('uyarı');
            datatable.reload();


        };

        // Start the connection.
        $.connection.hub.start().done(function () {

            //console.log("connection started")
            //notifications.onconn();

        }).fail(function (s) {
            console.log('hata');
            alert(s);
        });
    });
    $(function () {
        // Declare a proxy to reference the hub.
        var piece = $.connection.myHub;

        //debugger;
        // Create a function that the hub can call to broadcast messages.
        piece.client.updategridpiece = function (value) {

            alert('uyarı');
            datatable.reload();


        };

        // Start the connection.
        $.connection.hub.start().done(function () {

            //console.log("connection started")
            //notifications.onconn();

        }).fail(function (s) {
            console.log('hata');
            alert(s);
        });
    });
    $(function () {
        // Declare a proxy to reference the hub.
        var notis = $.connection.myHub;

        //debugger;
        // Create a function that the hub can call to broadcast messages.
        notis.client.gridmachine = function () {

            alert('uyarı');
            datatable.reload();


        };

        // Start the connection.
        $.connection.hub.start().done(function () {

            //console.log("connection started")
            //notifications.onconn();

        }).fail(function (s) {
            console.log('hata');
            alert(s);
        });
    });
    function Kategorii() {
        datatable = $('.machine').KTDatatable({
            data: {
                type: 'remote',
                method: 'GET',
                source: {
                    read: {
                        url: 'MachineListe',//signalr url KategoriListe
                        contentType: 'application/html ; charset:utf-8',
                        type: 'GET',
                        dataType: 'json',
                        success: function (result) {
                            var a2 = JSON.stringify(result)
                            //datatable.reload();
                            console.log(a2);
                        }
                    },
                },
                pageSize: 10, // display 20 records per page
                serverPaging: false,
                serverFiltering: false,
                serverSorting: true,
            },
            colReorder: {
                realtime: true
            },
            // layout definition
            layout: {
                scroll: true,
                height: 600,
                footer: false,
            },
            // column sorting
            sortable: true,
            pagination: false,
            detail: {
                title: '',
                content: machineDetail,
            },
            search: {
                input: $('#generalSearch'),
            },
            // columns definition
            columns: [
                {
                    field: 'MachineID',
                    title: '',
                    width: 5,
                    sortable: false,
                }, {
                    field: 'MachineName',
                    title: 'Makina Adı',
                    width: 100,
                    sortable: 'asc',
                }, {
                    field: 'MachineModel',
                    title: 'Makina Modeli',
                    width: 900,
                    sortable: 'asc',
                },
                //{
                //    field: 'CreatedDate',
                //    //field: 'createdDateString',
                //    title: 'Eklenme Tarihi',
                //    width: 120,
                //    sortable: 'asc',
                //},
                //{
                //    field: 'Adet',
                //    title: 'Adedi',
                //    width: 120,
                //},
                //{
                //    field: '',
                //    title: '',
                //    width: 900,
                //    sortable: false,
                //    overflow: 'visible',
                //    autoHide: false,
                //    template: function not(row) {
                //        return '\
                //                \
                //    <a data-toggle="modal" data-target="#takimanotekle" data-nottakim_id='+ row.PieceId + ' aria-haspopup="true" aria-expanded="false" href="" id="parca" class="btn btn-sm btn-clean btn-icon btn-icon-md test takimnotekle" title="Not Ekle">\
                //        <i class="la la-pencil"></i>\
                //    </a>\
                //';
                //    },
                //}
            ],
        });
    };
    $(document).on('click', '.takimnotekle', function () {
        var nottakimid = $(this).data("nottakim_id");
        $("#notetakim").val(nottakimid)
        $("#notetakim").hide()
        $("#typetakim").hide()
    });
    $('#kt_form_status').on('change', function () {
        datatable.search($(this).val().toLowerCase(), 'PieceName');
    });
    $('#kt_form_status,#kt_form_type').selectpicker();
    $('#datatable_asc').on('click', function () {
        datatable.sort('PieceName', 'asc');
    });
    $('#datatable_desc').on('click', function () {
        datatable.sort('PieceName', 'desc');
    });
    function machineDetail(e) {
        $('<div/>').attr('class', 'datatable_' + e.data.MachineID).appendTo(e.detailCell).KTDatatable({
            data: {
                type: 'remote',
                source: {
                    read: {
                        url: 'KategoriListe?machineId=' + e.data.MachineID,

                    },
                },
                pageSize: 10,
                responsive: true,
                serverPaging: false,
                serverFiltering: true,
                serverSorting: true,
            },
            detail: {
                title: '',
                content: subTableInit,
            },
            search: {
                input: $('#generalSearchh'),
            },
            sortable: true,
            // layout definition
            layout: {
                scroll: true,
                height: 600,
                footer: false,
                // enable/disable datatable spinner.
                spinner: {
                    type: 1,
                    theme: 'default',
                },
            },
            // columns definition
            columns: [
                {
                    field: 'PieceId',
                    title: '',
                    width: 5,
                    sortable: false,
                }, {
                    field: 'PieceName',
                    title: 'Takım Adı',
                    width: 100,
                    sortable: 'asc',
                }, {
                    field: 'CreatedDate',
                    //field: 'createdDateString',
                    title: 'Eklenme Tarihi',
                    width: 120,
                    sortable: 'asc',
                },
                //{
                //    field: 'Adet',
                //    title: 'Adedi',
                //    width: 120,
                //},
                {
                    field: '',
                    title: '',
                    width: 900,
                    sortable: false,
                    overflow: 'visible',
                    autoHide: false,
                    template: function not(row) {
                        return '\
                                \
		                  <a data-toggle="modal" data-target="#takimanotekle" data-nottakim_id='+ row.PieceId + ' aria-haspopup="true" aria-expanded="false" href="" id="parca" class="btn btn-sm btn-clean btn-icon btn-icon-md test takimnotekle" title="Not Ekle">\
		                      <i class="la la-pencil"></i>\
		                  </a>\
		              ';
                    },
                }
            ],
        });
    };
    //$(document).on('click', '.takimnotekle', function () {
    //    var nottakimid = $(this).data("nottakim_id");
    //    $("#notetakim").val(nottakimid)
    //    $("#notetakim").hide()
    //    $("#typetakim").hide()
    //});
    $('#ilk,#son').on('change', function () {
        var firstdatee = $('#ilk').val();
        var lastdatee = $('#son').val();
        var filterDatess = { firstdatee: firstdatee, lastdatee: lastdatee };
        datatable.search(filterDatess, 'filterDatess');
    });
    $('#kt_form_status').on('change', function () {
        datatable.search($(this).val().toLowerCase(), 'PieceName');
    });
    $('#kt_form_status,#kt_form_type').selectpicker();
    $('#datatable_asc').on('click', function () {
        datatable.sort('PieceName', 'asc');
    });
    $('#datatable_desc').on('click', function () {
        datatable.sort('PieceName', 'desc');
    });
    $('#ilk,#son').on('change', function () {
        var firstdatee = $('#ilk').val();
        var lastdatee = $('#son').val();
        var filterDatess = { firstdatee: firstdatee, lastdatee: lastdatee };
        dataa.search(filterDatess, 'filterDatess');
    });
    function subTableInit(e) {
        $('<div/>').attr('class', 'childd_' + e.data.PieceId).appendTo(e.detailCell).KTDatatable({
            data: {
                type: 'remote',
                source: {
                    read: {
                        url: 'AltKategori?pieceId=' + e.data.PieceId,

                    },
                },
                pageSize: 10,
                responsive: true,
                serverPaging: false,
                serverFiltering: true,
                serverSorting: true,
            },
            detail: {
                title: '',
                content: subTableInitt,
            },
            search: {
                input: $('#generalSearchh'),
            },
            sortable: true,
            // layout definition
            layout: {
                scroll: true,
                height: 1000,
                footer: false,
                // enable/disable datatable spinner.
                spinner: {
                    type: 1,
                    theme: 'default',
                },
            },
            // columns definition
            columns: [
                {
                    field: 'SubPieceId',
                    title: 'ID',
                    sortable: false,
                }, {
                    field: 'subPieceName',
                    title: 'Parça Adı',
                }, {
                    field: 'pieceCount',
                    title: 'Kullanılan Parça',
                }, {
                    field: 'toolLife',
                    title: 'Takım Ömrü',
                },
                {
                    field: 'createdDate',
                    field: 'createdDateString',
                    title: 'Eklenme Tarihi',
                }, {
                    field: 'kalan',
                    title: 'Kalan Parça Sayısı',
                },
                {
                    field: 'Actions',
                    width: 100,
                    title: '\
		                  <div style="font-size=15px" class="dropdown">\
		                      <a href="javascript:;" class="btn btn-default btn-sm btn-icon-sm" data-toggle="dropdown">\
		                          <i class="la la-download">Excel</i>\
		                      </a>\
		                      <div class="dropdown-menu dropdown-menu-right">\
		                          <a class="dropdown-item" id="import"  data-toggle="modal" data-target="#exampleModalll" aria-haspopup="true" aria-expanded="false" href="#"><i class="la la-file-excel-o"></i> Excel İçe Aktar</a>\
		                          <a class="dropdown-item" id="export"  data-toggle="modal" data-target="#parcaexportmodal" aria-haspopup="true" aria-expanded="false" href="#"><i class="la la-file-excel-o"></i> Excel Dışa Aktar </a>\
		                         \
		                      </div>\
		                  \
		              ',
                    sortable: false,
                }, {
                    field: 'subpiece',
                    width: 110,
                    title: '',
                    sortable: false,
                    overflow: 'visible',
                    autoHide: false,
                    template: function myFunctionthree(row) {
                        return '\
		                  <a data-toggle="modal" data-target="#parcaguncelle" data-subpiece_id='+ row.SubPieceId + ' data-subpiece_name=' + row.subPieceName + ' data-subpiece_toollife=' + row.toolLife + ' aria-haspopup="true" aria-expanded="false" href="" id="parca" class="btn btn-sm btn-clean btn-icon btn-icon-md test" title="Parça Düzenle">\
		                      <i class="la la-edit"></i>\
		                  </a>\
		                  <a data-toggle="modal" data-target="#parcasil" data-subpiecesil_id='+ row.SubPieceId + ' aria-haspopup="true" aria-expanded="false" href="#" class="btn btn-sm btn-clean btn-icon btn-icon-md parcsil" title="Parça Sil">\
		                      <i class="la la-trash"></i>\
		                  </a>\
                             <a data-toggle="modal" data-target="#parcayanotekle" data-notparca_id='+ row.SubPieceId + ' aria-haspopup="true" aria-expanded="false" href="#" class="btn btn-sm btn-clean btn-icon btn-icon-md parcanotekle" title="Not Ekle">\
		                      <i class="la la-pencil"></i>\
		                  </a>\
		              ';
                    },
                }],
        });


    };
    $(document).on('click', '.test', function () {
        var parname = $(this).data("subpiece_name");
        $("#subpiecename").val(parname);
        var partotal = $(this).data("subpiece_toollife");
        $("#ToolLife").val(partotal);
        //var id = $(this).data("subpiece_id");
    });
    $(document).on('click', '.parcsil', function () {
        var parid = $(this).data("subpiecesil_id");
        $("#parc").val(parid)
        $("#parcasil").hide()
    });
    $(document).on('click', '.parcanotekle', function () {
        var notparcaid = $(this).data("notparca_id");
        $("#noteparca").val(notparcaid)
        $("#noteparca").hide()
        $("#typeparca").hide()
    });
    $('#ilk,#son').on('change', function () {
        var firstdate = $('#ilk').val();
        var lastdate = $('#son').val();
        var filterDates = { firstdate: firstdate, lastdate: lastdate };
        dataa.search(filterDates, 'filterDates');
    });
    $(document).on('click', '.test', function () {
        var id = $(this).data("subpiece_id");
        $("#parcaduz").val(id);
        //$("#parcaduz").hide();
        var parname = $(this).data("subpiece_name");
        $("#subpiecename").val(parname);
        var partotal = $(this).data("subpiece_toollife");
        $("#ToolLife").val(partotal);
    });
    document.getElementById("import").addEventListener("click", myFunction)
    function myFunction() {
        $("#importt").hide();
        $("#importt").val(e.data.PieceId);
    }
    document.getElementById("export").addEventListener("click", myFunctiontwo)
    function myFunctiontwo() {
        $("#exportt").hide();
        $("#exportt").val(e.data.PieceId);
    };

    function subTableInitt(value) {



        var html = "";
        //function DetayListe() {
        $('<div/>').attr('id', 'child_data_ajax_' + value.data.SubPieceId).appendTo(value.detailCell).html("");

        var id = value.data.SubPieceId;

        var myvar = '<div id="chartdiv"></div>';
        $.ajax({
            type: "GET",
            //contentType: "application/html; charset=utf-8",
            url: "/Home/DetayListe?SubPieceId=" + id,
            data: "",
            dataType: "html",
            success: function (dataa) {
                console.log(dataa)
                var a3 = JSON.parse(dataa)
                var cart = [];
                var total = 0;
                var toolLife = 0;
                var html = '<div class="progress"> </div >'
                value.detailCell.append(html)

                $.each(a3, function (index, value) {
                    if ($(value.target).closest('tr').siblings().find('.progress').length < 2) {
                        var pieceCount = value.PieceCount;
                        total += value.PieceCount;
                        toolLife = value.ToolLife;
                        var createdDate = value.CreatedDate;
                        var percentile = toolLife / 3;
                        var progressHtml = '<div class="progress-bar bg-success tooltip-top" id="bar" style = "width:' + pieceCount + '%;border:outset"  data-toggle="tooltip" data-placement="top" data-original-title="Kullanılan Parça Sayısı:  ' + pieceCount + '\nEklenme Tarihi:  ' + createdDate + '">'
                            + '</div >';
                        $('.progress').append(progressHtml)
                        $('div').tooltip()

                    }
                    var element = {}
                    element.date = createdDate;
                    element.deger = pieceCount;
                    cart.push(element);
                    if (total >= (toolLife / 3) && total <= ((toolLife / 3) * 2))
                        $('#bar' + index).removeClass("bg-success").addClass("bg-warning")
                    else if (total >= ((toolLife / 3) * 2) && total <= toolLife)
                        $('#bar' + index).removeClass("bg-success").addClass("bg-danger")
                })
                if ($('#chartdiv').length > 0) {
                    $('#chartdiv').remove();
                }

                value.detailCell.append(myvar);
                am4core.useTheme(am4themes_animated);
                // Themes end


                var chartHelper = {

                    createChart: function createChart(chartdiv, charttype) {
                        // Check if the chart instance exists
                        chartHelper.disposeChart(chartdiv);
                        // Create new chart
                        chartReg[chartdiv] = am4core.create(chartdiv, charttype);
                        return chartReg[chartdiv];
                    },

                    disposeChart: function maybeDisposeChart(chartdiv) {
                        if (chartReg[chartdiv]) {
                            chartReg[chartdiv].dispose();
                            delete chartReg[chartdiv];
                        }
                    }

                }


                //Create chart instance
                var chart = am4core.create("chartdiv", am4charts.XYChart);

                var chart = chartHelper.createChart('chartdiv', am4charts.XYChart);



                // Add data
                chart.data = cart;


                // Enable export
                chart.exporting.menu = new am4core.ExportMenu();
                chart.exporting.menu.align = "right";
                chart.exporting.menu.verticalAlign = "top";

                chart.exporting.menu.items = [
                    {
                        "label": "...",
                        "menu": [
                            { "type": "png", "label": "PNG" },

                            { "type": "xlsx", "label": "EXCEL" },
                            { "type": "pdf", "label": "PDF" },
                            { "label": "YAZDIR", "type": "print" }
                        ]
                    }
                ]



                // Create axes

                var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
                categoryAxis.dataFields.category = "date";
                categoryAxis.renderer.grid.template.location = 0;
                categoryAxis.renderer.minGridDistance = 30;

                categoryAxis.renderer.labels.template.adapter.add("dy", function (dy, target) {
                    if (target.dataItem && target.dataItem.index & 2 == 2) {
                        return dy + 25;
                    }
                    return dy;
                });

                var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
                valueAxis.min = 0;
                valueAxis.max = toolLife;


                // Create series
                var series = chart.series.push(new am4charts.ColumnSeries());
                series.dataFields.valueY = "deger";
                series.dataFields.categoryX = "date";
                series.name = "Deger";

                series.columns.template.tooltipText = "Eklenilen Parça Sayısı: [bold]{valueY}[/]";
                series.columns.template.fillOpacity = .8;
                series.columns.template.width = am4core.percent(35);

                var columnTemplate = series.columns.template;
                columnTemplate.strokeWidth = 2;
                columnTemplate.strokeOpacity = 1;
            }

        });
        //}
        //}



    };

    // }//subtableinit parantez

    return {
        // Public functions
        init: function () {
            // init dmeo
            Kategorii();

        },
    };
    //function signalr() {
    //    // Declare a proxy to reference the hub.
    //    var noti = $.connection.myHub;
    //    //debugger;
    //    // Create a function that the hub can call to broadcast messages.
    //    noti.client.griddetay = function () {
    //        alert('uyarı');
    //        subTableInitt();
    //    };
    //    // Start the connection.
    //    $.connection.hub.start().done(function (a) {
    //        //console.log("connection started")
    //        //notifications.onconn();
    //        subTableInitt(a)
    //    }).fail(function (s) {
    //        console.log('hata');
    //        alert(s);
    //    });
    //}

    jQuery(document).ready(function () {
        KTDatatablee.init();
    });
    // end am4core.ready()
    //    var url = '/Home/Kategori',
    //        $('table').html();
    //    var thead = '<table><thead><tr><td>Id</td><td>KategoriAd</td></tr></thead>';
    //    $('table').append(thead);
    //    var deger;
    //    $.getJSON(url, function (data) {
    //        for (var item in data.Result) {
    //            deger = '<tbody><tr><td>' + data.Result[item].PieceId + '</td><td>' + data.Result[item].Name + '</td></tr></tbody>';
    //        }
    //        $('table').append(deger).append('</table>');
    //    }
    //}
    //function barOlustur(value) {
    //    var html = "";
    //    //var newprogress = (parseFloat(value.Count) / 3).toFixed(0);
    //    var oneThirdNumber = (parseFloat(value.TotalCount) / 3).toFixed(0); // toplam sayının 3 te 1 i
    //    var twoThirdNumber = (oneThirdNumber * 2).toFixed(0); // toplam sayının 3 te 2 si
    //    var threeThirdNumber = (oneThirdNumber * 3).toFixed(0); // toplam sayı
    //    var name = value.Name;
    //    var count = value.Count;
    //    //var degisken = count - oneThirdNumber; //kalan 7
    //    //var degisken2 = twoThirdNumber - count;
    //    var totalTime = value.TotalCount; //toplam değer
    //    var sonuc;
    //    var sayı1 = value.Count;
    //    var sayı2 = value.TotalCount;
    //    sonuc = sayı1 % sayı2;
    //    html = '<div id="progress-bar" class="progress-sm" role="progressbar" style="width: ' + 0 + '%" aria-valuenow="' + 0 + '" aria-valuemin="0" aria-valuemax="' + totalTime + '"></div>';
    //    var interval = setInterval(function () {
    //        //i progress barın koşula göre yeşil gelmesi için birer birer arttırdığım değişken
    //        // i+=4;
    //        if (sonuc > oneThirdNumber) {
    //            //toplam sayının 3'te 1'i  i değişkeninden küçük olduğu sürece progress barın rengini yeşil getirecek. burdaki koşula göre yeşili getiren yer burası
    //            $("#progress-bar")
    //                .css("width", oneThirdNumber + "%").addClass("progress-bar progress-bar-success")
    //                .attr("aria-valuenow", sonuc);
    //        }
    //    },1000)
    //        return html;
    //}
}