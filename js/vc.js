const apiClientFactory = require('./lib/serviceClient');
const directAuth = require('./lib/directAuth');

const userId = process.env.SophtronApiUserId;
const accessKey = process.env.SophtronApiUserSecret;

const directClient = apiClientFactory(directAuth, userId, accessKey);

process.on('unhandledRejection', error => {
    console.log('error: ');
    console.log(error);
});

(async function(){

  console.log('Getting customers')
  const customers = await directClient.getCustomers()
  const customer = customers[0]
  const customerId =  customer.CustomerID
  const memberId = customer.MemberIDs[0]
  console.log('Getting VC')
  const path = `api/vc/customers/${customerId}/members/${memberId}/identity`
  // const path = `api/vc/customers/${customerId}/members/${memberId}/accounts`
  const res = await directClient.getVC(path)
    .catch(err => console.log(err.response.data))
  const decoded =  Buffer.from(res.data.vc.split('.')[1], 'base64');
  console.log(JSON.parse(decoded.toString()).vc.credentialSubject)
})()

