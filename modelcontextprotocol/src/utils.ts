import { createHmac } from "crypto";
import * as fs from "fs";
import * as path from "path";


export function getHmac(dataToEncrypt: string, key: string) {
  if (!dataToEncrypt || !key) {
    return "";
  }

  const hmac = createHmac('sha256', Buffer.from(key, 'base64'));
  hmac.update(dataToEncrypt);
  return hmac.digest('base64');
}

export function buildSophtronAuthCode(
  httpMethod: string,
  url: string,
  apiUserID: string,
  secret: string,
) {
  const authPath = url.substring(url.lastIndexOf("/")).toLowerCase();
  const text = httpMethod.toUpperCase() + "\n" + authPath;
  const b64Sig = getHmac(text, secret);
  return "FIApiAUTH:" + apiUserID + ":" + b64Sig + ":" + authPath;
}

// const logger = fs.createWriteStream(path.join('/home/ubuntu/dev/github/Sophtron-Integration/modelcontextprotocol/out', 'log.txt'), {
//   flags: 'w' 
// });
// export function logError(message: string, data: any = ""){
//   logger.write(`${message}\n`)
//   logger.write(JSON.stringify(data))
//   logger.write('\n')
// }

export function logError() {}