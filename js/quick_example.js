const apiClientFactory = require('./lib/serviceClient');
const directAuth = require('./lib/directAuth');
const oauth = require('./lib/oauth');

var directClient = apiClientFactory(directAuth);
var oauthClient = apiClientFactory(oauth);

process.on('unhandledRejection', error => {
    console.log('error: ');
    console.log(error);
});

console.log('Pinging Api with direct auth')
directClient.ping().then(data => console.log('direct auth ping: ' + data));

//using oauth
console.log('Pinging Api with oauth')
oauthClient.ping().then(data => console.log('oauth ping: ' + data));