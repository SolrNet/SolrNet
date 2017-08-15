function ping(url) {
	var request = new ActiveXObject('MSXML2.XMLHTTP.3.0');
	request.open('GET', url, false);
	try {
		request.send();
		return request.status == 200;
	} catch (e) {
		return false;
	}
}
function pingUntilOK(url) {
	for (var i = 0; i < 10; i++)
		if (ping(url))
			return true;
	return false;
}
WScript.Echo('Waiting for Solr to start up...');
if (!pingUntilOK('http://localhost:8983/solr')) {
	WScript.Echo('Solr did not start up successfully. Please see the log.');
	WScript.Quit(1);
}

WScript.Echo('Waiting for web app to start up...');
if (!pingUntilOK('http://localhost:8082')) {
	WScript.Echo('Web app did not start up successfully. Please see the log.');
	WScript.Quit(1);
}
WScript.Quit(0);