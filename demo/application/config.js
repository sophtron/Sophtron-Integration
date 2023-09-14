const processEnv = {};
const envs = {...process.env, ...process.client_envs};
Object.keys(envs).forEach((k) => {
  processEnv[k.toUpperCase()] = envs[k];
});
const config = {
  LogLevel: 'debug',
  Port: '8088',
  Env: 'pre', // mocked
  Version: '',
  CryptoAlgorithm: 'aes-256-cbc',

  SophtronAuthServiceEndpoint: 'https://auth.sophtron-prod.com/api',
  
  SophtronApiServiceEndpoint: 'https://api.sophtron-prod.com/api',
  SophtronVCServiceEndpoint: 'https://vc.sophtron-prod.com/api/',

  SophtronApiUserId: 'd37f847e-86a3-4a88-8918-248d0f50b2f3',
  SophtronApiUserSecret: 'YTZkMTZmNDQtOGYzMy00NjA3LWFiODMtNzkyN2VmOGNmMGQy',

  MxClientId:'',
  MxApiSecret: '', 
  MxClientIdProd: '',
  MxApiSecretProd: '',

  AkoyaClientId:'',
  AkoyaApiSecret: '', 
  AkoyaClientIdProd: '',
  AkoyaApiSecretProd: '',

  FinicityPartnerId: '',
  FinicityAppKey: '',
  FinicitySecret: '',
  FinicityPartnerIdProd: '',
  FinicityAppKeyProd: '',
  FinicitySecretProd: '',
};

const arr = Object.keys(config);
for (let i = 0; i < arr.length; i++) {
  const key = arr[i];
  // if(key === 'SophtronApiUserId' || key === 'SophtronApiUserSecret'){
  //   continue;
  // }
  config[key] = processEnv[key.toUpperCase()] || config[key];
}
module.exports = config;