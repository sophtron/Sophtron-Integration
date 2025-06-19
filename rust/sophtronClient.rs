extern crate ring;
extern crate dotenv;
extern crate data_encoding;

use ring::{digest, hmac};
use data_encoding::BASE64;

pub struct SophtronClient {
  ClientId: String,
  Secret: String
}

impl SophtronClient {
  pub fn new() -> Self {
        let ret = SophtronClient {
            ClientId: match dotenv::var("SophtronApiClientId"){
              Ok(clientId) => clientId,
              Err(error) => {
                  println!("{error}");
                  String::from("not found env SophtronApiClientId")
              }
            },
            Secret: match dotenv::var("SophtronApiClientSecret"){
              Ok(secret_key) => secret_key,
              Err(error) => {
                  println!("{error}");
                  String::from("not found env SophtronApiClientSecret")
              }
            }
        };
        // Self::GenerateAuthToken(&ret, "/example".to_string(), "get".to_string());
        return ret
    }
  
  fn GenerateAuthToken(&self, url: String, httpMethod: String) -> String{
    let authPath = url[(url.find("/").unwrap())..].to_lowercase();
    let payload = httpMethod.to_uppercase() + "\n" + &authPath;
    let secret_key = BASE64.decode(self.Secret.as_ref()).unwrap();
    let signed_key = hmac::Key::new(hmac::HMAC_SHA256, &secret_key);
    let signature = hmac::sign(&signed_key, payload.as_bytes());
    let b64Sig = BASE64.encode(signature.as_ref());
    let ret = "FIApiAUTH:".to_owned() + &self.ClientId + ":" + &b64Sig + ":" + &authPath;
    // println!("Auth token: {}", ret);
    return ret;
  }

  //TODO: add http requests
}