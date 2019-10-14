![alt text](https://app.trustev.com/assets/img/apple-icon-144.png)


#Trustev .NET Library
- If you are not familiar with Trustev, start with our [Developer Portal](http://www.trustev.com/developers).
- Check out our [API Documentation](https://app.trustev.com/help).
- If you would like to get some Test API keys to begin integrating, please contact us on our [support team](https://trustev.zendesk.com/hc/en-gb/requests/new?ticket_form_id=66326)

##Requirements
- .NET version 4.0 and later

##Installation
####NuGet
- Simply install via the Nuget Package Manager or install via the Package Manager Console with the following command 

			PM> Install-Package Trustev

####Others
- You could also download our solution, build it and simply include the .dll files as you need them.
- Our library can also be used as an example to inspire your own Integration to the Trustev Platform.

##Integration Tests
Included are async and sync tests to test the integration of the .NET Library against the Trustev API - This will show examples of how to use the various endpoints of the Trustev API.
Simply include your Test API Keys in the app.config files of either the Tests folder (Async) or TestsNet40 folder (sync).

## Usage
   The Trustev API has been designed to allow users complete control over what information they are sending to us while still ensuring that Integration can be done in a couple of simple steps.
   
### Simple Trustev Integration
This is simple version of the Trustev Integration and involves 4 simple steps:
```c#

// 1. Set-Up the Trustev API Client with your user credentials, and include your location details 
// - i.e. US (Enums.BaseUrl.US) or EU (Enums.BaseUrl.EU)
ApiClient.SetUp(userName, password, secret, baseURL);


// 2. Create your case.
// You will need two bits of information for this step
// 		SessionId : This is the SessionId that you have recieved from the Trustev JavaScript 
//					and transferred server-side
// 		CaseNumber : This is a number that you use to uniquely identify this case. It must
//					 be unique.
Case kase = new Case(sessionId, caseNumber);

// Now add any further information you have. The more you give us, the more accurate 
// our decisions.
kase.Customer = new Customer()
{
	FirstName = "John",
    LastName = "Doe",
}


// 3. Post this Case to the Trustev API
Case returnCase = ApiClient.PostCase(kase);


// 4. You can now get your Decision from Trustev base on the case you have given us!
Decision decision = ApiClient.GetDecision(returnCase.Id);


// Now it is up to you what you do with our decision

```

#### Optional Integration Steps
As mentioned earlier, we also provide detailed API endpoints for updating specific parts of your Case. These steps can be used where use cases require. See below for some examples.

##### Example : Adding a Customer

```c#

// 1. Set-Up the Trustev API Client with your user credentials, and include your location details 
// - i.e. US (Enums.BaseUrl.US) or EU (Enums.BaseUrl.EU)
ApiClient.SetUp(userName, password, secret, baseURL);


// 2. Create your case.
// You will need two bits of information for this step
// 		SessionId : This is the SessionId that you have received from the trustev JavaScript 
//					and transferred server-side
// 		CaseNumber : This is a number that you use to uniquely identify this case. It must
//					 be unique.
Case kase = new Case(sessionId, caseNumber);

// 3. Post this Case to the Trustev API
Case returnCase = ApiClient.PostCase(kase);


// 4. You may now want to add a Customer to Case you have already added.
//    First let's create the customer.
Customer customer = new Customer()
{
	FirstName = "John",
    LastName = "Doe",
}

//    Now we can go ahead and add the Customer to the Case we added earlier.
Customer returnCustomer = ApiClient.PostCustomer(returnCase.Id, customer);


// 5. You can now continue as normal and get the Decision on this case, including
//    the new customer you have added
Decision decision = ApiClient.GetDecision(returnCase.Id);


// Now it's up to you what you do with our decision

```

##### Example : Updating a Transaction

```c#

// 1. Set-Up the Trustev API Client with your user credentials, and include your location details
// - i.e. US (Enums.BaseUrl.US) or EU (Enums.BaseUrl.EU)
ApiClient.SetUp(userName, password, secret, baseURL);


// 2. Create your case.
// You will need two bits of information for this step
// 		SessionId : This is the SessionId that you have recieved from the Trustev JavaScript 
//					and transferred server-side
// 		CaseNumber : This is a number that you use to uniquely identify this case. It must
//					 be unique.
Case kase = new Case(sessionId, caseNumber);
kase.Transaction = new Transaction()
{
    Currency = "USD",
    TotalTransactionValue = 10
};

// 3. Post this Case to the Trustev API
Case returnCase = ApiClient.PostCase(kase);


// 4. Now, say the value of this Transaction changes,
//	  We provide the functionality to update the Transaction you have already added
//	  Just rebuild the Transaction again with the new information
Transaction transaction = new Transaction()
{
    Currency = "USD",
    TotalTransactionValue = 2000
};

//    Now we can go ahead and add the Transaction to the Case we added earlier.
Transaction returnTransaction = ApiClient.UpdateTransaction(returnCase.Id, transaction);


// 5. You can now continue as normal and get the Decision on this Case including
//    the updated Transaction you have added
Decision decision = ApiClient.GetDecision(returnCase.Id);


// Now its up to you what you do with our decision

```

We provide similar functions i.e. Post, Update and Get for every Sub Entity of the Case Object.
For more examples of these, check out the unit tests.


### Multiple Site Credentials Trustev Integration - Valid as of version update 1.1.1
This is simple version of the Trustev Integration and involves 4 simple steps:
```c#

// 1. Set-Up the Trustev API Client with your user credentials, and include your location details 
// - i.e. US (Enums.BaseUrl.US) or EU (Enums.BaseUrl.EU)
ApiClient.SetUp(userName, password, secret, baseURL);


// 2. Create your case.
// You will need two bits of information for this step
//		userName : This is optional - The user name, needed to support multiple credentials,
//      if only one set of credentials is used this parameter is not needed
//
// 		SessionId : This is the SessionId that you have received from the Trustev JavaScript 
//					and transferred server-side
// 		CaseNumber : This is a number that you use to uniquely identify this case. It must
//					 be unique.
Case kase = new Case(sessionId, caseNumber);

Case kase = new Case(sessionId, caseNumber, userName);

// Now add any further information you have. The more you give us, the more accurate 
// our decisions.
kase.Customer = new Customer()
{
	FirstName = "John",
    LastName = "Doe",
}


// 3. Post this Case to the Trustev API

Case returnCase = ApiClient.PostCase(kase);

//	userName is Optional: The user name, needed to support multiple credentials, 
//  if only one set of credentials is used this parameter is not needed

Case returnCase = ApiClient.PostCase(kase, userName);


// 4. You can now get your Decision from Trustev base on the case you have given us!

Decision decision = ApiClient.GetDecision(returnCase.Id);

//	userName is Optional: The user name, needed to support multiple credentials,
//  if only one set of credentials is used this parameter is not needed

Decision decision = ApiClient.GetDecision(returnCase.Id, userName);


// Now it is up to you what you do with our decision

```
