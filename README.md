# BankSystem

## Automatically generated user (with 10,000 euro)
| Username        	| Password 
|-----------------	|----------
| test@test.com 	  | test123

# Getting started
### Set up recaptcha functionality
1. Register a [SendGrid](https://sendgrid.com/) account.
2. [Create a Key and a Secret](https://developers.google.com/recaptcha/intro).
3. Insert the Key and the secret in the following file:
    * *src\Web\BankSystem.Web\appsettings.json*

Example:
```
"ReCaptcha": {
    "SiteKey": "6L************************************FG",
    "SiteSecret": "6L************************************D1"
  },
```

### Set up email functionality (optional)
1. Register a [SendGrid](https://sendgrid.com/) account.
2. [Create an API key](https://sendgrid.com/docs/ui/account-and-settings/api-keys/#creating-an-api-key).
3. Insert the API key in the following file:
    * *src\Web\BankSystem.Web\appsettings.json*

Example:
```
"SendGrid": {
  "ApiKey": "SA.10*****************************************************DO-zfxp"
}
```

### Change project urls
> Note: Modify only protocol, domain name and port because the rest of the url is the same
Example:
```
"https://localhost:56013/api/ReceiveMoneyTransfers", - this is the url which is used by my machine when running the project, If it's different on your machine please change it as it follows:
"https://localhost:7815/api/ReceiveMoneyTransfers",

https://localhost:56013 - this is the part which you must change 
if the projects are not hosted on the same urls on your machine because the rest of the url is the same as mentioned above
```
1. Change CentralApiAddress in src\Web\BankSystem.Web\appsettings.json with the url which your central api is running on.

Example:
```
"BankConfiguration": {
    "CentralApiAddress": "https://localhost:5001/",
  },
```
2. Change Central Api ApplicationBuilderExtensions.cs in src\Web\Api\CentralApi\Infrastructure\ApplicationBuilderExtensions.cs seed method with the url which your bank is running on.
```
P.S Please modify only the properties down below
dbContext.AddRange(
                    new Bank
                    {
                        ApiAddress = "https://localhost:56013/api/ReceiveMoneyTransfers",
                        PaymentUrl = "https://localhost:56013/pay/{0}",
                        CardPaymentUrl = "https://localhost:56013/api/cardPayments",
                    }

```
3. Modify appsettings.json in src\DemoShop\DemoShop.Web\appsettings.json with the urls which your projects are running on. And for the DestinationBankAccountConfiguration - Set the properties with the appropriate bank in which you'd like your money to be send when somebody purchases an item.

Example:
```
"DestinationBankAccountConfiguration": {
    "DestinationBankName": "Bank system",
    "DestinationBankCountry": "Bulgaria",
    "DestinationBankSwiftCode": "ABC",
    "DestinationBankAccountUniqueId": "INSERT_DESTINATION_ACCOUNT_HERE",
    "RecipientName": "INSERT_RECIPIENT_NAME_HERE"
  },
  
  "DirectPaymentsConfiguration": {
    "SiteUrl": "https://localhost:7001/",
    "CentralApiPaymentUrl": "https://localhost:5001/pay/{0}"
  },
  "CardPaymentsConfiguration": {
    "CentralApiCardPaymentsUrl": "https://localhost:5001/api/CardPayments"
  },
```

# For more documentation - [click here](https://github.com/melikpehlivanov/BankSystem/wiki)
