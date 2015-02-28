    ymaps.ready(init);
    var myMap;

    var dataControl = {
        //https://api.foursquare.com/v2/venues/explore?ll=37.794354,56.00793&client_id=5AD112QB4J1IYH30KTIJ3ZY5HFWI4Z4TA5JHGDDJU1ZV4C1H&client_secret=KEA2JSNL3315JH4ZUY3XKD3F5C3G2OZGOSFOMD5RW30XAZST&v=20150228
        foursquare: {
            client_id: "5AD112QB4J1IYH30KTIJ3ZY5HFWI4Z4TA5JHGDDJU1ZV4C1H",
            client_secret: "KEA2JSNL3315JH4ZUY3XKD3F5C3G2OZGOSFOMD5RW30XAZST",
            getPlaces: function (coordinates) {
                $.getJSON('https://api.foursquare.com/v2/venues/explore?ll=' + coordinates[0] + ','
                    + coordinates[1] + "&radius=5000&limit=100" +
                    '&client_id=' + dataControl.foursquare.client_id
                    + '&client_secret=' + dataControl.foursquare.client_secret + '&v=20150202', {}, function (data) {
                        try {
                            var items = data.response.groups[0].items;
                            console.log(items);

                            for (var i = 0; i < items.length; i++) {
                                //console.log(items[i]);
                                var item = items[i].venue;
                                var address = item.location.address;
                                if (address == undefined) {
                                    address = "Не указан";
                                }
                                var rating = item.rating;
                                if (rating == undefined) {
                                    rating = "Не указан";
                                }

                                var myPlacemark = new ymaps.Placemark([item.location.lat, item.location.lng], {
                                    hintContent: item.name,
                                    balloonContent: "<strong>" + item.name + "</strong></br>" +
                                        address + "</br>" +
                                        "Рейтинг: " + rating + "</br>" +
                                        ""
                                }, {
                                    preset: "islands#grayCircleDotIcon"
                                });
                                //console.log(myPlacemark);
                                myMap.geoObjects.add(myPlacemark);
                            }
                        } catch (ex) {
                            console.log(ex);
                        }
                    });
            }
        },
        //http://tts.voicetech.yandex.net/generate?text=%22%D0%9F%D1%80%D0%B8%D0%B2%D0%B5%D1%82%20%D0%BC%D0%B8%D1%80%22&format=mp3&emotion=good&lang=ru-RU&speaker=jane&key=c88bd5f0-c337-4b83-b2d0-b6e492cdd58c
        speech: {
            speechKey: "c88bd5f0-c337-4b83-b2d0-b6e492cdd58c",
            getSpeechLink: function (text) {
                return "http://tts.voicetech.yandex.net/generate?text=" + text + "&format=mp3&emotion=good&lang=ru-RU&speaker=jane&key=" + dataControl.speech.speechKey;
            }
        },
        loading: {
            maxLoadingCount: 3,
            loadingCount: 0,
            startLoading: function () {
                if (dataControl.loading.loadingCount > 0) {

                } else {
                    $("#loadingWrap").show();
                }
                dataControl.loading.loadingCount++;

                $("#loading").css('width', (dataControl.loading.loadingCount / dataControl.loading.maxLoadingCount * 100) +
                '%');

                if (dataControl.loading.loadingCount >= dataControl.loading.maxLoadingCount) {
                    dataControl.loading.stopLoading();
                }
            },
            stopLoading: function () {
                //$("#loadingWrap").hide();
            },
            stopLoadingOld: function () {
                if (dataControl.loading.loadingCount > 1) {

                } else {
                    console.log("loadingWrap hide");
                    $("#loadingWrap").hide();
                }
                dataControl.loading.loadingCount--;

                $("#loading").css('width', (dataControl.loading.loadingCount / dataControl.loading.maxLoadingCount * 100) +
                '%');
            }
        },
        allItems: [],
        currentArea: undefined,
        currentLines: [],
        userCoordinates: [0, 0],
        mapClick: function (e) {
            if (dataControl.currentArea !== undefined) {
                myMap.geoObjects.remove(dataControl.currentArea);
            }
            dataControl.currentLines.map(function (item) {
                myMap.geoObjects.remove(item);
            });
            dataControl.currentLines = [];

            // Получение координат щелчка
            var coords = e.get('coords');
            dataControl.currentArea = new ymaps.GeoObject({
                geometry: {
                    type: "Circle",
                    coordinates: [coords[0], coords[1]],
                    radius: 5000
                }
            });
            dataControl.foursquare.getPlaces([coords[0], coords[1]]);

            console.log(dataControl.currentArea);
            //dataControl.currentArea.add('click', f);
            myMap.geoObjects.add(dataControl.currentArea);

            dataControl.updateSumData([coords[0], coords[1]]);
        },
        getUserPosition: function (callback) {
            ymaps.geolocation.get().then(function (res) {

                dataControl.userCoordinates[0] = res.geoObjects.position[0];
                dataControl.userCoordinates[1] = res.geoObjects.position[1];

                var myPlacemark = new ymaps.Placemark([res.geoObjects.position[0], res.geoObjects.position[1]], {
                    hintContent: "Ваше местонахождение",
                    balloonContent: "Ваше местонахождение"
                }, {
                    preset: "islands#geolocationIcon"
                });

                myMap.geoObjects.add(myPlacemark);

                callback();
            }, function (e) {
                console.log(e);
                callback();
            });
        },
        getChildrenObjects: function () {
            dataControl.loading.startLoading();

            childPlaceItemTable = client.getTable('ChildPlaceItem');
            childPlaceItemTable.read().then(function (childItems) {
                listItems = $.map(childItems, function (item) {
                    try {
                        var position = [item.Y.replace(',', '.'), item.X.replace(',', '.')];
                        var myPlacemark = new ymaps.Placemark(position, {
                            hintContent: item.Name,
                            balloonContent: "<b>" + item.Name + "</b><br/>" +
                                item.Address + "<br/>Сумма контрактов - " + Math.round(item.ContractSum) + " руб.<br/>" +
                                "<audio controls><source src=\"" + dataControl.speech.getSpeechLink(item.Name) + "\" type=\"audio/mpeg\"></audio>"
                        });

                        dataControl.allItems.push({
                            name: item.Name,
                            sum: Math.round(item.ContractSum),
                            address: item.Address,
                            position: position
                        });

                        myMap.geoObjects.add(myPlacemark);

                        var myPolyline = new ymaps.GeoObject({
                            geometry: {
                                type: "LineString",
                                coordinates: [position, dataControl.userCoordinates]
                            }
                        });
                        myMap.geoObjects.add(myPolyline);
                    } catch (ex) {
                        console.dir(ex);
                    };
                });

                dataControl.updateSumData(position);
            });
        },
        getHospitalObjects: function () {
            dataControl.loading.startLoading();

            hospitalItemTable = client.getTable('HospitalAdultItem');

            hospitalItemTable.read().then(function (hospitalItems) {
                listItems = $.map(hospitalItems, function (item) {

                    var position = [item.Y.replace(',', '.'), item.X.replace(',', '.')];

                    var myPlacemark = new ymaps.Placemark(position, {
                        hintContent: item.FullName,
                        balloonContent: "<b>" + item.FullName + "</b><br/>" + item.Address + "<br/>Сумма контрактов - " + Math.round(item.ContractSum) + " руб.<br/>" +
                        "<audio controls><source src=\"" + dataControl.speech.getSpeechLink(item.FullName) + "\" type=\"audio/mpeg\"></audio>"
                    }, {
                        preset: 'islands#darkGreenIcon'
                    });

                    dataControl.allItems.push({
                        name: item.FullName,
                        sum: Math.round(item.ContractSum),
                        address: item.Address,
                        position: position
                    });

                    myMap.geoObjects.add(myPlacemark);

                    var myPolyline = new ymaps.GeoObject({
                        geometry: {
                            type: "LineString",
                            coordinates: [position, dataControl.userCoordinates]
                        }
                    });
                    myMap.geoObjects.add(myPolyline);
                });

                dataControl.updateSumData(position);
            });
        },
        getMinFinObjects: function () {
            dataControl.loading.startLoading();

            minFinItemTable = client.getTable('MinFinItem');

            minFinItemTable.read().then(function (minFinItems) {
                listItems = $.map(minFinItems, function (item) {

                    var position = [item.X.replace(',', '.'), item.Y.replace(',', '.')];

                    var myPlacemark = new ymaps.Placemark(position, {
                        hintContent: item.Name,
                        balloonContent: "<b>" + item.Name + "</b><br/>" + item.Address + "<br/>Сумма контрактов - " + Math.round(item.ContractSum) + " руб.<br/>" +
                        "<audio controls><source src=\"" + dataControl.speech.getSpeechLink(item.Name) + "\" type=\"audio/mpeg\"></audio>"
                    }, {
                        preset: 'islands#redIcon'
                    });

                    dataControl.allItems.push({
                        name: item.Name,
                        sum: Math.round(item.ContractSum),
                        address: item.Address,
                        position: position
                    });

                    myMap.geoObjects.add(myPlacemark);

                    var myPolyline = new ymaps.GeoObject({
                        geometry: {
                            type: "LineString",
                            coordinates: [position, dataControl.userCoordinates]
                        }
                    });
                    myMap.geoObjects.add(myPolyline);
                });

                dataControl.updateSumData(position);
            });
        },
        updateSumData: function (currentPlace) {
            var maink = 0;
            var objects_count = 0;
            var sum_count = 0;

            try {
                for (var k in dataControl.allItems) {
                    if (ymaps.coordSystem.geo.getDistance(dataControl.allItems[k].position, currentPlace) < 5001) {
                        maink = maink + calcCoef(dataControl.allItems[k].sum, ymaps.coordSystem.geo.getDistance(dataControl.allItems[k].position, currentPlace));
                        objects_count++;
                        sum_count = sum_count + Math.round(dataControl.allItems[k].sum);

                        var myPolyline = new ymaps.GeoObject({
                            geometry: {
                                type: "LineString",
                                coordinates: [currentPlace, dataControl.allItems[k].position]
                            },
                            options: {
                                fillColor: "#FF0000",
                                strokeColor: '#ff0000',
                                strokeWidth: 5
                            }
                        }, {
                            fillColor: "#FF0000",
                            strokeColor: '#ff0000',
                            strokeWidth: 5
                        });
                        myMap.geoObjects.add(myPolyline);
                        dataControl.currentLines.push(myPolyline);
                    }
                }
            } catch (ex) {
                console.dir(ex);
            }

            $("#objects_count").html(objects_count);
            $("#sum_count").html(sum_count);
            $("#maink").html(maink);
        }
    };

    function init(){     
        myMap = new ymaps.Map("map", {
            center: [55.76, 37.64],
            zoom: 7
        });

        dataControl.getUserPosition(function () {

            dataControl.getChildrenObjects();
            dataControl.getHospitalObjects();
            dataControl.getMinFinObjects();

            console.log("add map events");
            myMap.events.add('click', dataControl.mapClick);
        });
        
    }



    function calcCoef(sum, distance) {
                var k = 0;
                if ((distance != null) && (distance > 0)) {
                    k = sum / distance;
                }
                return Math.round(k);
            }

    