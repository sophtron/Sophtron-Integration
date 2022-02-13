# Sophtron Integration
Sample code of how to integrate with Sophtron data API. Currently support financial, utility, phone, internet and other vendor billing accounts.

Sophtron is a next generation data aggregation service that makes accessing data a breeze. No matter which sources the data are from, or which formats the data are of, Sophtron will provide you with a single API and uniform format. You only need to integrate with Sophtron once, and be assured that Sophtron API will take care of the rest. 

Armed with machine learning technologies, Sophtron's data aggregation system adjusts to data changes automatically. No matter whether the data source or format changes, our robust system will return the same format of data without any disruption. This means minimal down time for you and no maintenance cost to support angry customers' calls.

To integrate with Sophtron, follow our tutorial or sample code. Authentication methods include both direct API call and OAuth 2.0. Contact us at dev@sophtron.com if you have any questions.

Sophtron Banking and Billing API is free to developers up to 10,000 requests / month. More details at https://sophtron.com/Home/Pricing

## Example code
- js:
```bash
cd js
npm i
node quick_example.js
node full_example.js
```
- dotnet5: load with visualstudio and start debugging.
- both version provides direct auth and oauth method, with a `quick example` to access the authorized healthcheck endpoint
- js version of full bank auth (with mfa handling) example, please refer to [full_example](js/full_example.js)
- for sophtron widget integration, please refer to [Sophtron-Wiget-Loader](https://github.com/sophtron/sophtron-widget-loader) project
