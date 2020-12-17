function generateList(data){
    var regions = [];
    $.each( data, function(key, val) {
        var authorString = val.authors.join(", ");

        var content = '<b><a href="' + val.url + '">' + key + '</a></b>  -  ' + authorString;
        $("#regionList").append('<li id="' + key + '">' + content + '</li>');
    });
}

$(document).ready(function(){
    $.getJSON("web/customRegions.json", generateList);
});