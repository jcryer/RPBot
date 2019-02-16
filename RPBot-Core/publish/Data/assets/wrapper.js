$(document).ready(function() {
    console.log( "ready!" );
	$('#iframe').attr('src', "reeeee");

		
	$.get( "http://51.15.222.156/logs/loglist.csv", function( data ) {
  		$( ".result" ).html(data);
  		alert(data);
		var responseSplit = data.split(",");
		for (var i = 0; i < responseSplit.length; i++) {
			console.log(responseSplit[i]);
		}
	});
});