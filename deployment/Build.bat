@echo off
set /p id="Enter ID: "

nuget pack Cashier.nuspec -Version %id%
nuget pack Cashier.Stripe.nuspec -Version %id%