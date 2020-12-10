function generateList(data){
    var regions = [];
    $.each( data, function(key, val) {
        var content = "<b>"+key+"</b>  -  " + val.author;
        $("#regionList").append('<li id="' + key + '">' + content + '</li>');
    });
}

$(document).ready(function(){
    $.getJSON("web/customRegions.json", generateList);
});