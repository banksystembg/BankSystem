# BankSystem 
**BankSystem** is an open-source web project where users can create bank accounts, transfer money, create payment cards, pay with them or directly through their account, etc.
It supports connecting multiple banks together through the _CentralApi_. This allows money to be securely transferred between **separate instances of BankSystem** running on different machines.

## Project links

We have set up a network of two BankSystem instances and one [DemoShop](https://github.com/banksystembg/BankSystem/wiki/DemoShop), connected through _CentralApi_.

|Project |Address
|-|-
|**DemoShop** |https://banksystem-demoshop.azurewebsites.net/
|**Bank system 1** (first instance) <br><br> **Transfer details:** <br> * Bank name - _Bank system_ <br> * Bank country - _Bulgaria_ <br> * Bank code - _ABC_ |https://banksystem1.azurewebsites.net/
|**Bank system 2** (second instance) <br><br> **Transfer details:** <br> * Bank name - _Bank system 2_ <br> * Bank country - _Germany_ <br> * Bank code - _CBA_ |https://banksystem2.azurewebsites.net/

All of these projects have a demo account already registered:

| Email                 | Password 
|-----------------	|----------
| test@test.com         | Test123$

## Documentation

### [Getting started](https://github.com/banksystembg/BankSystem/wiki/Getting-started)
>**This page contains important information on how to properly configure BankSystem and CentralApi**

BankSystem consists of two base components - the bank web application and the _CentralApi_, which securely connects banks running on separate machines together to process [transfers between different banks](https://github.com/banksystembg/BankSystem/wiki/Money-transfers#Global--worldwide-transfers), [card payments](https://github.com/banksystembg/BankSystem/wiki/Cards#Purchases) and [direct payments](https://github.com/banksystembg/BankSystem/wiki/Direct-payments).

### [Bank accounts](https://github.com/banksystembg/BankSystem/wiki/Bank-accounts)
Bank accounts hold information about their owner, balance, transactions, date of creation, etc.

### [Money transfers](https://github.com/banksystembg/BankSystem/wiki/Money-transfers)
BankSystem supports two types of money transfers - internal and global / worldwide.

### [Cards](https://github.com/banksystembg/BankSystem/wiki/Cards)
Cards are used for making purchases on other websites using the _CentralApi_.

### [Direct payments](https://github.com/banksystembg/BankSystem/wiki/Direct-payments)
Direct payments are a way to securely pay on websites directly through a bank account without the need to provide card details.

### [DemoShop](https://github.com/banksystembg/BankSystem/wiki/DemoShop)
DemoShop is an example web application implementing direct and card payments.

***

**Used technologies:**
* C#
* ASP.NET Core
* ASP.NET Core MVC
* ASP.NET Core Web API
* Entity Framework Core
* asymmetric encryption
* jQuery
* AJAX
* HTML
* CSS
* Bootstrap
