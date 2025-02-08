const crypto = require('crypto');
//direct

function buildAuthCode(httpMethod, url, userId, accessKey) {
  userId = userId || process.env.SophtronApiUserId;
  accessKey = accessKey || process.env.SophtronApiUserSecret;
  var authPath = url.substring(url.lastIndexOf('/')).toLowerCase();
  var integrationKey = Buffer.from(accessKey, 'base64');
  var plainKey = httpMethod.toUpperCase() + '\n' + authPath;
  var b64Sig = crypto.createHmac('sha256', integrationKey).update(plainKey).digest("base64");
  var authString = 'FIApiAUTH:' + userId + ':' + b64Sig + ':' + authPath;
  return Promise.resolve(authString);
}

module.exports = buildAuthCode;