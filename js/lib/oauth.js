const axios = require('axios')
const username = process.env.SophtronUserName;
const clientId = process.env.SophtronApiClientId;
const clientSecret = process.env.SophtronApiClientSecret;
const authPhraze = Buffer.from(`${clientId}:${clientSecret}`).toString('base64');

var token;
function getToken(){
    if(token && token.expires > new Date()){
        return Promise.resolve(token);
    }
    return axios.post('https://sophtron.com/oauth/token', `grant_type=password&username=${username}`, {
        headers: {
            "Content-Type": 'application/x-www-form-urlecoded',
            "Authorization": 'Basic ' + authPhraze
        },
    }).then(res => {
        //console.log(res.data);
        if(res.data.access_token){
            token = res.data;
            token.expires = new Date().getTime() + (token.exires_in) * 1000;
        }
        return token;
    }).catch(err => 
        {
            console.log(err);
        }
    );
}

module.exports = function buildAuth(){
    return getToken().then(t => `bearer ${token.access_token}`);
}