var PIO_VURL = "";
var PH_VURL = "";
var PIO_IMG = "";
var PH_IMG = "";
var PIO_ENDED = false;
var PH_ENDED = false;
var timer, timeSpent = [];
var phtimer, phtimeSpent = [];
$(document).ready(function () {
    //$('.site-bg-overlay').css('background-color', 'rgba(41, 174, 160, 0.9)');  
    //$('.site-bg-img').css('background-image', 'url(assets/image/bg.jpg)');

    //Get Site bg-image
    var bgImage = $(".site-bg-img").data("bgimage");
    if (bgImage !== "" && bgImage != null) {
        $("body").attr("class", "is-site-bg-img");
        $(".site-bg-img").css("background-image", "url(" + $(".site-bg-img").data("bgimage") + ")");
    }
    
    //Get Site bg-audio
    var bgAudio = $(".site-bg-audio").data("bgaudio");
    if (bgAudio !== "" && bgAudio != null) {
        $("body").addClass("is-loaded is-audio-on");
        $("<audio id='audioPlayer' loop></audio>").attr({
            'src': 'assets/audio/audio.mp3',
            'type': 'audio/mpeg',
            'volume': 0.4,
            'autoplay': 'autoplay'
        }).appendTo("body");
    }


    //Get Passitcode Image
    var PIO_IMG = $(".site-pio-img").data("pioimage");
    if (PIO_IMG !== "" && PIO_IMG != null) {
        $("#codeplayer").hide();
        $("#piocodead").attr('src', JSON.parse(PIO_IMG));
        $("#passItOnCode").prop("disabled", false);
    }

    //Get Passitcode Video
    var piovideo = $(".site-pio-video").data("piovurl");
    if (piovideo !== "" && piovideo != null) {
        $("#piocodead").hide();
        PIO_VURL = JSON.parse($(".site-pio-video").data("piovurl"));
        /*var src = "https://www.youtube.com/embed/" + PIO_VURL + "?autoplay=1&enablejsapi=1";
        $("#codeiframe").attr('src', src);*/

    }

    //Get Phone Image - Default
    var PH_IMG = $(".site-phone-img").data("phoneimage");
    if (PH_IMG !== "" && PH_IMG != null) {
        $("#phoneplayer").hide();
        $("#phonead").attr('src', JSON.parse(PH_IMG));
    }

    //Get Phone Video - Default
    var phvideo = $(".site-phone-video").data("phonevurl");
    if (phvideo !== "" && phvideo != null) {
        $("#phonead").hide();
        PH_VURL = JSON.parse($(".site-phone-video").data("phonevurl"));
    }


    $("input[id=passItOnCode]").keyup(function () {
        if ($(this).val().length === 10) {
            //alert(charLimit);
            $.get("api/passiton/?passitCode=" + $('#passItOnCode').val(),
            function (data) {
                if (data.AdType === "PhoneNo-Image") {
                    $("#phoneplayer").hide();
                    $("#phonead").attr('src', data.ImageUrl);
                    $("#phoneNumber").prop("disabled", false);
                    $("#phoneNumber").prop("placeholder", "Enter Mobile Number...");

                }
                else if (data.AdType === "PhoneNo-Video") {
                    $("#phonead").hide();
                    phoneplayer.cueVideoById({
                        videoId: data.VideoUrl,
                        startSeconds: 1,
                        endSeconds: 0,
                        suggestedQuality: 'medium'
                    });
                    PH_ENDED = false;
                    phtimeSpent = [];
                    //phoneplayer.addEventListener('onReady', 'onPlayerReady');
                    phoneplayer.addEventListener('onStateChange', 'onPhPlayerStateChange');

                }
                console.log("Data: " + data.AdType + "\nStatus: " + status);
            });

            $('#passNumber').trigger("click");
        }
    });

    $("#reward").on("click", function () {
        if ($("#phoneNumber").val().length === 10) {
            bootbox.confirm("Ready to recieve your reward?", function (result) {
                if (result) {
                    var dialog = bootbox.dialog({
                        message: '<p class="text-center"><i class="fa fa-refresh fa-spin"></i> Please wait while we process your request...</p>',
                        closeButton: false
                    });
                    //Make The API Call
                    $.post("api/passiton/?passitCode=" + $('#passItOnCode').val(),
                        {
                            MobileNo: $("#phoneNumber").val()
                        },
                        function(data, status) {
                            console.log("Data: " + data + "\nStatus: " + status);
                            dialog.modal('hide'); //close the custom dialog

                            bootbox.alert({
                                message: data,
                                backdrop: true
                            });

                        //Do Re-Initialization
                        $("input[id=passItOnCode]").clearFields();
                        $("input[id=phoneNumber]").clearFields();
                        
                        if (PIO_IMG !== "" && PIO_IMG != null) {
                            $("#codeplayer").hide();
                            $("#piocodead").attr('src', JSON.parse(PIO_IMG));
                            $("#passItOnCode").prop("disabled", false);
                        }

                        $("#phoneNumber").prop("disabled", true);

                        if (PH_IMG !== "" && PH_IMG != null) {
                            $("#phoneplayer").hide();
                            $("#phonead").attr('src', JSON.parse(PH_IMG));
                        }

                        if (PIO_VURL !== "" && PIO_VURL != null) {

                            $("#passItOnCode").prop("disabled", true);
                            codeplayer.cueVideoById({
                                videoId: PIO_VURL,
                                startSeconds: 1,
                                endSeconds: 0,
                                suggestedQuality: 'medium'
                            });

                            PIO_ENDED = false;
                            timeSpent = [];
                        }

                        if (PH_VURL !== "" && PH_VURL != null) {
                            phoneplayer.cueVideoById({
                                videoId: PH_VURL,
                                startSeconds: 1,
                                endSeconds: 0,
                                suggestedQuality: 'medium'
                            });
                            phoneplayer.removeEventListener('onStateChange', 'onPhPlayerStateChange');
                            //phoneplayer.removeEventListener('onReady', 'onPlayerReady');
                        }

                        $('#passCode').trigger("click");
                    });
                }
            });

            $('body').css('padding-right', '0');
        }
    });

});


var yid = $(".mbYTP_wrapper").attr("id");

var tag = document.createElement('script');
tag.id = 'iframe-demo';
tag.src = 'https://www.youtube.com/iframe_api';
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

var player;
var codeplayer;
var phoneplayer;
function onYouTubeIframeAPIReady() {
    player = new YT.Player(yid,
    {
        videoId: _bg_video_youtube_url,
        playerVars: {
            modestbranding: 1,
            iv_load_policy: 3,
            cc_load_policy: 0,
            title: " ",
            autoplay: 1,
            loop: 1,
            suggestedQuality: _bg_video_youtube_quality,
            //playlist: _bg_video_youtube_url,
            controls: 0,
            end: 0,
            rel: 0
            //showinfo: 0,
        },
        events: {
            'onReady': function (e) {
                e.target.playVideo();              
            },
            'onStateChange': function (e) {
                if (e.data === YT.PlayerState.PLAYING) {
                    $(".mbYTP_wrapper").css('opacity', '100');
                }

                if (e.data === YT.PlayerState.ENDED) {
                    player.playVideo();
                }
            }
        }
    });

    codeplayer = new YT.Player('codeiframe',
    {
        height: '360',
        width: '440',
        videoId: PIO_VURL,
        playerVars: {
            modestbranding: 1,
            iv_load_policy: 3,
            cc_load_policy: 0,
            //autoplay: 1,
            suggestedQuality: 'medium',
            fs: 0,
            end: 0,
            rel: 0,
            showinfo: 0
        },
        events: {
            //'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
        }
    });

    phoneplayer = new YT.Player('phoneiframe',
    {
        height: '360',
        width: '440',
        videoId: PH_VURL,
        playerVars: {
            modestbranding: 1,
            iv_load_policy: 3,
            cc_load_policy: 0,
            suggestedQuality: 'medium',
            fs: 0,
            end: 0,
            rel: 0,
            showinfo: 0
        }
    });
}

//PIOCODE Video Events
function onPlayerReady(event) {
    event.target.playVideo();
}


function onPlayerStateChange(event) {
    if (event.data === YT.PlayerState.PLAYING) {
        if (!timeSpent.length) {
            for (var i = 0, l = parseInt(player.getDuration()) ; i < l; i++)
                timeSpent.push(false);
        }
        timer = setInterval(record, 100);
    }
    else if (event.data === YT.PlayerState.ENDED) {
        clearInterval(timer);
        $("#passItOnCode").prop("placeholder", "Enter Code Here...");
        PIO_ENDED = true;
    }
    else if (event.data === YT.PlayerState.CUED) {
        clearInterval(timer);
        $("#passItOnCode").prop("disabled", true);
        $("#passItOnCode").prop("placeholder", "Enter Code Here...");
    }
    else {
        clearInterval(timer);
        $("#passItOnCode").prop("placeholder", "Enter Code Here...");
    }
}


function record() {
    if (!PIO_ENDED) {
        timeSpent[parseInt(player.getCurrentTime())] = true;
        showPercentage();
    }
}

function showPercentage() {
    var percent = 0;
    var timeleft = 6;
    for (var i = 0, l = timeSpent.length; i < l; i++) {
        if (timeSpent[i]) {
            percent++;
            if (percent !== timeleft) {
                $("#passItOnCode").prop("placeholder", "Active in: " + (timeleft - percent) + "s");

            } else {
                PIO_ENDED = true;
                $("#passItOnCode").prop("disabled", false);
                $("#passItOnCode").prop("placeholder", "Enter Code Here...");
            }
        }    
        
    }
}

//Function for Phone Video
function onPhPlayerStateChange(event) {
    if (event.data === YT.PlayerState.PLAYING) {
        var run = false;
        if (!phtimeSpent.length) {
            for (var i = 0, l = parseInt(player.getDuration()); i < l; i++)
                phtimeSpent.push(false);

            run = true;
        }
        if(run)
            phtimer = setInterval(pHrecord, 100);
    }
    else if (event.data === YT.PlayerState.ENDED) {
        clearInterval(phtimer);
        $("#phoneNumber").prop("placeholder", "Enter Mobile Number...");
        PH_ENDED = true;
    }
    else if (event.data === YT.PlayerState.CUED) {
        clearInterval(phtimer);
        $("#phoneNumber").prop("disabled", true);
        $("#phoneNumber").prop("placeholder", "Enter Mobile Number...");
    }
    else {
        clearInterval(phtimer);
        $("#phoneNumber").prop("placeholder", "Enter Mobile Number...");
    }
}


function pHrecord() {
    if (!PH_ENDED) {
        phtimeSpent[parseInt(player.getCurrentTime())] = true;
        pHshowPercentage();
    }
}

function pHshowPercentage() {
    var percent = 0;
    var timeleft = 6;
    for (var i = 0, l = phtimeSpent.length; i < l; i++) {
        if (phtimeSpent[i]) {
            percent++;
            if (percent !== timeleft) {
                $("#phoneNumber").prop("placeholder", "Active in: " + (timeleft - percent) + "s");

            } else {
                PH_ENDED = true;
                $("#phoneNumber").prop("disabled", false);
                $("#phoneNumber").prop("placeholder", "Enter Mobile Number...");
            }
        }

    }
}