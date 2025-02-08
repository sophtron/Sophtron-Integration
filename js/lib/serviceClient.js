const crypto = require('crypto');
const axios =  require('axios');

const apiClientFactory = function(buildAuth, userId, accessKey){
    const apiBaseUrl = 'https://api.sophtron.com/api/';
    async function post(url, data){
        let conf = {headers: {Authorization: await buildAuth('post', url, userId, accessKey)}};
        console.log('Post: ' + url);
        let res = await axios.post(apiBaseUrl + url, data, conf);
        return res.data
    }

    async function get(url){
        let conf = {headers: {Authorization: await buildAuth('get', url, userId, accessKey)}};
        console.log('Get: ' + url);
        let res = await axios.get(apiBaseUrl + url, conf);
        return res.data
    }

    return {
        ping: function(){
            return get('Institution/HealthCheckAuth');
        },
        getIngrationKey: function(){
            return post('User/GetUserIntegrationKey', {Id: userId})
        },
        getInstitutionsByName: function(name){
            return post('Institution/GetInstitutionByName', {InstitutionName: name})
        },
        getUserInstitutionsByUser: function(){
            return post('UserInstitution/GetUserInstitutionsByUser', {UserID: userId});
        },
        getJobInformationByID: function(){
            return post( 'Job/GetJobInformationByID', {JobID: jobId});
        },
        createUserInstitution: function(userId, institutionId, username, password, pin){
            return post("UserInstitution/CreateUserInstitution", {
                UserID: userId,
                InstitutionID: institutionId,
                UserName: username,
                Password: password,
                PIN: pin
            });
        },
        retryAddingUserInstitution: function(userInstitutionId){
            return post("UserInstitution/RetryAddingUserInstitution", {
                UserInstitutionID: userInstitutionId
            });
        },
        getJobInformationByID: function(jobId){
            return post("Job/GetJobInformationByID",{ JobID: jobId });
        },
        updateJobSecurityAnswer: function(jobID, securityAnswer){
            return post("Job/UpdateJobSecurityAnswer", {
                    JobID: jobID,
                    SecurityAnswer: securityAnswer
                });
        },
        updateJobCaptchaInput: function(jobID, captchaInput){
            return post("Job/UpdateJobCaptcha", {
                    JobID: jobID,
                    CaptchaInput: captchaInput
                });
        },

        updateJobTokenChoice: function(jobID, tokenChoice){
            return post("Job/UpdateJobTokenInput", {
                    JobID: jobID,
                    TokenChoice: tokenChoice,
                    TokenInput: null,
                    VerifyPhoneFlag: null,
                });
        },
        updateJobTokenInput: function(jobID, tokenInput){
            return post("Job/UpdateJobTokenInput", {
                    JobID: jobID,
                    TokenChoice: null,
                    TokenInput: tokenInput,
                    VerifyPhoneFlag: null,
                });
        },
        updateJobTokenPhoneVerify: function(jobID, verifyPhoneFlag){
            return post("Job/UpdateJobTokenInput", {
                    JobID: jobID,
                    TokenChoice: null,
                    TokenInput: null,
                    VerifyPhoneFlag: verifyPhoneFlag,
                });
        },
        getUserInstitutionAccounts: function(userInstitutionId){
            return post("UserInstitution/GetUserInstitutionAccounts", { UserInstitutionID: userInstitutionId });
        },
        refreshUserInstitutionAccount: function(userInstitutionAccountId){
            return post("UserInstitutionAccount/RefreshUserInstitutionAccount", {AccountID: userInstitutionAccountId });
        },
        getTransactionsByTransactionDate: function(userInstitutionAccountId, startDate, endDate){
            return post("Transaction/GetTransactionsByTransactionDate", {
                    AccountID: userInstitutionAccountId,
                    StartDate: startDate,
                    EndDate: endDate
                });
        },
        getCustomers(){
          return get(`v2/customers`)
        },
        getVC: async function(path) {
          const res = await this.getIngrationKey();
          const headers = { 
            IntegrationKey: res.IntegrationKey,
            // Authorization: await buildAuth('get', path, userId, accessKey) 
          };
          const ret = await axios.get(`https://vc.sophtron.com/${path}`, {headers})
          return ret?.vc || ret;
        }
    }
};

module.exports = apiClientFactory;