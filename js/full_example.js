const apiClientFactory = require('./lib/serviceClient');
const directAuth = require('./lib/directAuth');

var client = apiClientFactory(directAuth);
const userId = process.env.SophtronApiUserId;

process.on('unhandledRejection', error => {
    console.log('error: ');
    console.log(error);
});

async function mfa(jobId){
    console.log('checking mfa');
    var job = await client.getJobInformationByID(jobId);
    if(!job){
        console.log('Unable to find job');
        return 'failure';
    }
    if(job.SuccessFlag === true){
        console.log('mfa: SuccessFlag: true')
        return ('sucess');
    }
    if(job.SuccessFlag === false){
        console.log('mfa: SuccessFlag: false')
        if(job.LastStatus !== 'Completed'){
            return ('failure');
        }
    }
    if(job.LastStatus === 'AccountsReady'){
        console.log('mfa: AccountsReady')
        //this indicates accounts information is ready, if the job is not only retriving accounts, it is retriving transactions now
        return ('AccountsReady');
    }
    if (job.SecurityQuestion)
    {
        console.log('mfa: SecurityQuestion')
        //security questions example:  '["What was the first name of your first manager?","What is your mother&#39;s middle name","What is your cat&#39;s name"]'
        //require user to input security answer. Pay attention to the format.
        var securityAns = "[\"managername\",\"mothername\",\"catname\"]";
        await client.updateJobSecurityAnswer(jobId, securityAns);
    }
    else if (job.TokenMethod)
    {
        console.log('mfa: TokenMethod')
        //token method example: '[":  Email me   Mail me verification code at s*********@gmail.com", ":  Call me  Send verification code by voice call to ***-***-3800"]'
        //require user to choose.
        var tokenChoice = ":  Email me   Mail me verification code at s*********@gmail.com";
        await client.updateJobTokenChoice(jobID, tokenChoice);
    }
    else if (job.TokenSentFlag == true)
    {
        console.log('mfa: TokenSentFlag')
        //require user to fill in token he received.
        var tokenInput = "123";
        await client.updateJobTokenInput(jobID, tokenInput);
    }
    else if (job.TokenRead != null)
    {
        console.log('mfa: TokenRead')
        //answer the phone to read the content of 'tokenread'
        var phoneVerified = true;
        await client.updateJobTokenPhoneVerify(jobID, phoneVerified);
    }
    else if (job.CaptchaImage != null)
    {
        console.log('mfa: CaptchaImage')
        //input captcha code
        var captchaInput = "12345";
        await client.updateJobCaptchaInput(jobID, captchaInput);
    }
    console.log('mfa: waiting')
}

async function handleMfa(jobId){
    var job;
    while(!job){
        job = await mfa(jobId);
        //check again after 5 seconds
        await new Promise(resolve => setTimeout(resolve, 5000))
    }
    return job;
}

async function entry(){
    var institutionName = "sophtron bank";
    var inst = (await client.getInstitutionsByName(institutionName))[0];//choose target bank 
    console.log(inst);
    var institutionId = inst.InstitutionID; 
    var username = "bankloginname"; //input your bank login name
    var password = "bankloginpwd"; //input your bank login password
    var pin = '';
    console.log('Creating user institution')
    var jobTracker = await client.createUserInstitution(userId, institutionId, username, password, pin);  
    console.log(jobTracker);
    var mfaRet = await handleMfa(jobTracker.JobID);
    if(mfaRet === 'failure'){
        console.log('Job failed');
        // IF MFA Failed -> Retry?
        return;
    }

    // IF MFA Succeed -> Get Account:
    console.log(`mfa: ${mfaRet}, getting user accounts`);
    var accounts = await client.getUserInstitutionAccounts(jobTracker.UserInstitutionID);
    if(accounts && accounts.length > 0)
    {
        // Refresh Account:
        console.log('refreshing user accounts');
        var jobId = (await client.refreshUserInstitutionAccount(accounts[0].AccountID)).JobID;
        console.log(jobId);
        await handleMfa(jobId);
        // IF MFA Succeed -> Get Transactions:
        console.log('getting transactions');
        var transactionsHistory = await client.getTransactionsByTransactionDate(accounts[0].AccountID, new Date(new Date().getTime() + 30 * 24 * 60 * 60 * 1000), new Date());
        console.log(transactionsHistory);
    }
}

entry().then(ret => {
    console.log('done');
})