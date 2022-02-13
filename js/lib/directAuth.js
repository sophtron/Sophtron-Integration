 const crypto = require('crypto');
 //direct
 const userId = process.env.SophtronApiUserId;
 const accessKey = process.env.SophtronApiUserSecret;
 // console.log(userId);
 // console.log(accessKey);
 function buildAuthCode(httpMethod, url) {
     var authPath = url.substring(url.lastIndexOf('/')).toLowerCase();
     var integrationKey = Buffer.from(accessKey, 'base64');
     var plainKey = httpMethod.toUpperCase() + '\n' + authPath;
     var b64Sig = crypto.createHmac('sha256', integrationKey).update(plainKey).digest("base64");
     var authString = 'FIApiAUTH:' + userId + ':' + b64Sig + ':' + authPath;
     return Promise.resolve(authString);
 }
 
 module.exports = buildAuthCode;